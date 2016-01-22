using System.Web;
using System.Web.Optimization;

namespace MvcSA
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-migrate-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/select2").Include(
                        "~/Content/select2.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/flick/jquery-ui-flick.css"));

            // Bootstrap's files
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js"));
            bundles.Add(new StyleBundle("~/Content/bootstrap-css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/bootstrap-responsive.css"));

            // jqGrid
            bundles.Add(new StyleBundle("~/Content/jqueryjqgrid").Include(
                        "~/Content/jquery.jqGrid/ui.jqgrid.css"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryjqgrid").Include(
                        "~/Scripts/jquery.jqGrid.js",
                        "~/Scripts/i18n/grid.locale-en.js"));

            // form validator
            bundles.Add(new StyleBundle("~/Content/form-validator").Include(
                        "~/Content/form-validator.css"));
            bundles.Add(new ScriptBundle("~/bundles/form-validator").Include(
                        "~/Scripts/form-validator/jquery.form-validator.min.js",
                        "~/Scripts/form-validator/file.js"));

            // jTable
            bundles.Add(new StyleBundle("~/Content/jTable").Include(
                        "~/Scripts/jtable/themes/metro/blue/jtable.css"));
            bundles.Add(new ScriptBundle("~/bundles/jTable").Include(
                        "~/Scripts/jtable/jquery.jtable.js"));

            // Select2
            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                        "~/Scripts/select2.js"));

            // Moment
            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                        "~/Scripts/moment.js"));

            //Validate
            bundles.Add(new ScriptBundle("~/bundles/validate").Include("~/Scripts/jqValidate.js"));
            bundles.Add(new StyleBundle("~/Content/validate").Include("~/Content/formvalidator/css/formvalidator.css"));
        }
    }
}