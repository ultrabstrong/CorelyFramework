using BusinessLayer.Core;
using KelleyUtilities.Helpers;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        /// <summary>
        /// Desktop path
        /// </summary>
        static string DesktopLoc { get; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";

        /// <summary>
        /// Entry point for console test
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                var reg = new Regex(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$");
                var email = "ultrabstrong@gmail.com";
                Console.WriteLine(reg.IsMatch(email));
                //TestHTMLtoPDF();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught:{Environment.NewLine}{ex}");
            }
            Console.WriteLine("Program finished. Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// Test converting html to pdf
        /// </summary>
        static void TestHTMLtoPDF()
        {
            string filepath = $"{DesktopLoc}sample.html";
            string outpath = DesktopLoc + FilePathHelper.GetFileNameNoExt(filepath) + ".pdf";
            string html = File.ReadAllText(filepath);
            // Convert with SelectPDF
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            converter.Options.PdfDocumentInformation.Author = "ApexProperties";
            converter.Options.PdfDocumentInformation.CreationDate = DateTime.Now;
            converter.Options.PdfDocumentInformation.Title = "Application";
            converter.Options.PdfDocumentInformation.Subject = "Subect";
            PdfDocument doc = converter.ConvertHtmlString(html);
            byte[] pdfBytes = doc.Save();
            doc.Close();
            // close pdf document
            if (File.Exists(outpath)) { File.Delete(outpath); }
            using (FileStream fs = new FileStream(outpath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(pdfBytes, 0, pdfBytes.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// Test sending mail
        /// </summary>
        static void TestMail()
        {
            MailSettings settings = new MailSettings()
            {
                SMTPServer = "sm04.internetmailserver.net",
                SMTPUsername = "Application@rentinbozeman.net",
                SMTPPw = "snowC@pp3dPeeks",
                SMTPPort = 25,
                SMTPTo = "rentinbozeman@gmail.com"
            };
            // Initialize SMTP client
            SmtpClient client = null;
            try
            {
                // Build client
                client = new SmtpClient(settings.SMTPServer);
                client.Port = 25;
                client.Credentials = new NetworkCredential(settings.SMTPUsername, settings.SMTPPw);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Create message
                MailMessage message = new MailMessage()
                {
                    Subject = $"test subject",
                    From = new MailAddress(settings.SMTPUsername)
                };

                // Set body to html content
                message.Body = $"test body";
                message.IsBodyHtml = false;

                // Add recipients
                message.To.Add(settings.SMTPTo);

                // Send message
                client.Send(message);
            }
            // Dispose client
            finally { if (client != null) client.Dispose(); }
        }
    }
}
