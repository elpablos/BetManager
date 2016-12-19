using System.Web;
using System.Web.Optimization;

namespace BetManager.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                 "~/Scripts/jquery.dataTables.js",
                 "~/Scripts/dataTables.bootstrap.js",
                 "~/Scripts/DataTables-1.10.12/extensions/Responsive/js/dataTables.responsive.js"
                 ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/moment.js",
                      "~/Scripts/moment-with-locales.js",
                      "~/Scripts/bootstrap-datetimepicker.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/metisMenu").Include(
                        "~/Scripts/metisMenu.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/morris").Include(
                        "~/Scripts/raphael.js",
                        "~/Scripts/morris.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/sb").Include(
                        "~/Scripts/sb-admin-2.js"
                ));

            // CSS

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-datetimepicker.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                 "~/Content/jquery.dataTables.css",
                 "~/Content/dataTables.bootstrap.css",
                 "~/Content/DataTables-1.10.12/extensions/Responsive/css/responsive.dataTables.css",
                 "~/Content/DataTables-1.10.12/extensions/Responsive/css/responsive.bootstrap.css"
                 ));

            bundles.Add(new StyleBundle("~/Content/font-awesome").Include(
                      "~/Content/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/metisMenu").Include(
                      "~/Content/metisMenu.css"));

            bundles.Add(new StyleBundle("~/Content/morris").Include(
                      "~/Content/morris.css"));

            bundles.Add(new StyleBundle("~/Content/sb").Include(
                      "~/Content/sb-admin-2.css"));
        }
    }
}
