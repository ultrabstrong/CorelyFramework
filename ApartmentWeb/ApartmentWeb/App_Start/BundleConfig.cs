using System.Web;
using System.Web.Optimization;

namespace ApartmentWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Modernizr SCRIPT
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            // Modal SCRIPT
            bundles.Add(new ScriptBundle("~/bundles/modal").Include(
                       "~/Scripts/custom/modalmanager.js"));
            // JQuery SCRIPT
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            // JQuery Validate SCRIPT
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/custom/requireif*",
                        "~/Scripts/custom/visibletoggle*",
                        "~/Scripts/custom/validationErrorStyle*",
                        "~/Scripts/jquery-ui-{version}.js"));
            // Bootstrap SCRIPT
            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/umd/popper.min.js",
                      "~/Scripts/bootstrap.js"));
            // Bootstrap STYLE
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/custom.css",
                      "~/Content/themes/base/jquery-ui.min.css"));
            // Home SCRIPT
            bundles.Add(new ScriptBundle("~/bundles/home").Include(
                      "~/Scripts/bootstrap-select.min.js"));
            // Home STYLE
            bundles.Add(new StyleBundle("~/Content/home").Include(
                      "~/Content/bootstrap-select.min.css"));
        }
    }
}
