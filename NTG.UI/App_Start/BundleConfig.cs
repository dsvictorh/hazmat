using System.Web;
using System.Web.Optimization;

namespace NTG.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new LessBundle("~/bundles/less/ntg").Include(
                "~/assets/less/ntg.less"));

            bundles.Add(new LessBundle("~/bundles/less/hazmat-ntg").Include(
                "~/assets/less/hazmat-ntg.less"));

            bundles.Add(new LessBundle("~/bundles/less/hazmat").Include(
                "~/assets/less/hazmat.less"));

            bundles.Add(new StyleBundle("~/bundles/css/hazmat/lib").Include(
                "~/assets/lib/angular-datepicker/angular-datepicker.css",
                "~/assets/lib/jquery/jquery-ui.css"));


            bundles.Add(new ScriptBundle("~/bundles/js/ntg").Include(
                "~/assets/lib/jquery/jquery.js",
                "~/assets/lib/angular/angular.js",
                "~/assets/js/ntg/app.js",
                "~/assets/js/app-global.js",
                "~/assets/js/directives/global-input-fields.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/hazmat").Include(
                "~/assets/lib/jquery/jquery.js",
                "~/assets/lib/jquery/jquery-ui.js",
                "~/assets/lib/jquery/jquery-ui-touch-punch.js",
                "~/assets/lib/moment/moment.js",
                "~/assets/lib/moment/moment-timezone.js",
                "~/assets/lib/angular/angular.js",
                "~/assets/lib/angular/angular-sanitize.js",
                "~/assets/lib/angular/angular-wysiwyg.js",
                "~/assets/lib/angular-datepicker/angular-datepicker.js",
                "~/assets/js/hazmat/app.js",
                "~/assets/js/app-global.js",
                "~/assets/js/hazmat/services/page-service.js",
                "~/assets/js/directives/global-input-fields.js",
                "~/assets/js/directives/hazmat-input-fields.js",
                "~/assets/js/directives/expandable-menu-item.js"));

            /*bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));*/
        }
    }
}