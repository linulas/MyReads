using System.Web;
using System.Web.Optimization;

namespace MyReads
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/googleBooks").Include(
                        "~/Scripts/googleBooks.js"));
            bundles.Add(new ScriptBundle("~/bundles/animations").Include(
                        "~/Scripts/animatelo.min.js",
                        "~/Scripts/agency.min.js",
                        "~/Scripts/sortable.min.js",
                        "~/Scripts/jquery-easing.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/rating").Include(
                        "~/Scripts/star-rating.js",
                        "~/Scripts/rating.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/affix.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/star-rating.min.css",
                      "~/Content/agency.min.css",
                      "~/Content/site.css"));
        }
    }
}
