using BusinessLayer.Core;
using Corely.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using logrm = Resources.Website.Logs;
using rm = Resources.Website.Home;

namespace ApartmentWeb.Controllers
{
    public class HomeController : Controller
    {
        public static readonly string Name = nameof(HomeController).Replace(nameof(Controller), "");

        /// <summary>
        /// Return base website
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Return application page
        /// </summary>
        /// <returns></returns>
        public ActionResult Apply()
        {
            BusinessLayer.Application application = new BusinessLayer.Application();
            return View(application);
        }

        /// <summary>
        /// Return page to download application
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadApplication()
        {
            return View();
        }

        /// <summary>
        /// Return tenant info page
        /// </summary>
        /// <returns></returns>
        public ActionResult TenantInfo()
        {
            return View();
        }

        /// <summary>
        /// Return maintenance request page
        /// </summary>
        /// <returns></returns>
        public ActionResult MaintenanceRequest()
        {
            BusinessLayer.MaintenanceRequest maintenanceRequest = new BusinessLayer.MaintenanceRequest();
            return View(maintenanceRequest);
        }

        /// <summary>
        /// Submit a maintenance request
        /// </summary>
        /// <param name="maintenanceRequest"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        public ActionResult SubmitMaintenanceRequest(BusinessLayer.MaintenanceRequest maintenanceRequest)
        {
            if (ModelState.IsValid)
            {
                Shared.Logger.WriteLog(logrm.creatingViewHtml, "", LogLevel.DEBUG);
                string htmlstr = RenderRazorViewToString(nameof(this.MaintenanceRequest), maintenanceRequest);
                Task.Run(() =>
                {
                    try
                    {
                        // Create PDF of application
                        Shared.Logger.WriteLog(logrm.convertingHtmlToPDF, "", LogLevel.DEBUG);
                        byte[] pdfbytes = PDF.ConvertHtmlToPDF(htmlstr,
                            Shared.Configuration.SiteDetails.CompanyName,
                            $"{maintenanceRequest.FirstName} {maintenanceRequest.LastName} Maintenance Request",
                            $"Maintenance request for {maintenanceRequest.RentalAddress} from {maintenanceRequest.FirstName} {maintenanceRequest.LastName}");
#if DEBUG
                        // Output sample PDF and HTML for debugging
                        System.IO.File.WriteAllText($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\maintsample.html", htmlstr);
                        if (System.IO.File.Exists($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\maintsample.pdf")) { System.IO.File.Delete($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\maintsample.pdf"); }
                        using (FileStream fs = new FileStream($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\maintsample.pdf", FileMode.Create, FileAccess.ReadWrite))
                        {
                            fs.Write(pdfbytes, 0, pdfbytes.Length);
                            fs.Flush();
                        }
#endif
                        // Send maintenance request in email
                        Shared.Logger.WriteLog(logrm.sendingMaintEmail, "", LogLevel.DEBUG);
                        using (MemoryStream pdfstream = new MemoryStream(pdfbytes))
                        {
                            Mail.SendMaintenanceRequest(Shared.Configuration.SiteDetails.MailSettings, maintenanceRequest, htmlstr, pdfstream);
                        }
                        Shared.Logger.WriteLog(logrm.finishedSendingMaintEmail, "", LogLevel.DEBUG);
                    }
                    catch (Exception ex)
                    {
                        Shared.Logger.WriteLog(logrm.failSubmitMaintReq, ex, LogLevel.ERROR);
                    }
                });
                ViewBag.maintsubmitted = "yes";
                return View(nameof(this.Index));
            }
            else
            {
                // Return view and errors
#if DEBUG
                List<KeyValuePair<string, ModelState>> Errors = ModelState.Where(m => m.Value.Errors.Count > 0).ToList();
#endif
                ViewBag.Errors = true;
                return View(nameof(MaintenanceRequest), maintenanceRequest);
            }
        }

        /// <summary>
        /// Return contact us page
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactUs()
        {
            return View();
        }

        /// <summary>
        /// Submit an application
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        public ActionResult SubmitApplication(BusinessLayer.Application application)
        {
            Shared.Logger.WriteLog(logrm.checkingModel, "", LogLevel.DEBUG);
            if (ModelState.IsValid)
            {
                Shared.Logger.WriteLog(logrm.creatingViewHtml, "", LogLevel.DEBUG);
                string htmlstr = RenderRazorViewToString(nameof(this.Apply), application);
                Task.Run(() =>
                {
                    try
                    {
                        // Create PDF of application
                        Shared.Logger.WriteLog(logrm.convertingHtmlToPDF, "", LogLevel.DEBUG);
                        byte[] pdfbytes = PDF.ConvertHtmlToPDF(htmlstr, 
                            Shared.Configuration.SiteDetails.CompanyName, 
                            $"{application.PersonalInfo.FirstName} {application.PersonalInfo.LastName} Application",
                            $"Application for {application.RentalAddress} from {application.PersonalInfo.FirstName} {application.PersonalInfo.LastName}; Co-Applicants : {application.OtherApplicants}");
#if DEBUG
                        // Output sample PDF and HTML for debugging
                        System.IO.File.WriteAllText($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\sample.html", htmlstr);
                        if (System.IO.File.Exists($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\sample.pdf")) { System.IO.File.Delete($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\sample.pdf"); }
                        using(FileStream fs = new FileStream($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\sample.pdf", FileMode.Create, FileAccess.ReadWrite))
                        {
                            fs.Write(pdfbytes, 0, pdfbytes.Length);
                            fs.Flush();
                        }
#endif
                        // Send application in email
                        Shared.Logger.WriteLog(logrm.sendingEmail, "", LogLevel.DEBUG);
                        using (MemoryStream pdfstream = new MemoryStream(pdfbytes))
                        {
                            Mail.SendApplication(Shared.Configuration.SiteDetails.MailSettings, application, htmlstr, pdfstream);
                        }
                        Shared.Logger.WriteLog(logrm.finishedSendingEmail, "", LogLevel.DEBUG);
                    }
                    catch (Exception ex)
                    {
                        Shared.Logger.WriteLog(logrm.failSubmitApp, ex, LogLevel.ERROR);
                    }
                });
                ViewBag.submitted = "yes";
                return View(nameof(this.Index));
            }
            else
            {
                // Return view and errors
#if DEBUG
                List<KeyValuePair<string, ModelState>> Errors = ModelState.Where(m => m.Value.Errors.Count > 0).ToList();
#endif
                ViewBag.Errors = true;
                return View(nameof(Apply), application);
            }
        }

        /// <summary>
        /// Reload service configuration
        /// </summary>
        public ActionResult ReloadConfig()
        {
            Shared.LoadConfiguration();
            return Content(rm.HOME_CONFIG_RELOADED);
        }

        /// <summary>
        /// Render view as HTML string
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                ViewBag.inlinecss = "Yes";
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}