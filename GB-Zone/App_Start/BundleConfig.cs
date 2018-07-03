using System.Web;
using System.ComponentModel.Composition;
using System.Web.Optimization;

namespace GrinGlobal.Zone
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            // Homer style
            var bundlesContent = new StyleBundle("~/Content/css")
                                .Include("~/Content/style.css", new CssRewriteUrlTransform())
                                .Include("~/Content/bootstrap.min.css", new CssRewriteUrlTransform());

            var bundlesVendor = new StyleBundle("~/Vendor/css")
                                 .Include("~/Vendor/animate.css/animate.min.css", new CssRewriteUrlTransform())
                                 .Include("~/Vendor/pe-icon-7-stroke/css/pe-icon-7-stroke.css")
                                 .Include("~/Vendor/fontawesome/css/font-awesome.min.css", new CssRewriteUrlTransform());

            bundles.Add(bundlesContent);
            bundles.Add(bundlesVendor);

            // Homer script
            bundles.Add(new ScriptBundle("~/bundles/homer/js").Include(
                        "~/Scripts/metisMenu.js",
                      "~/Scripts/homer.js"));

            // Bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap/js").Include(
                      "~/Scripts/bootstrap.min.js"));
            

            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery/js").Include(
                      "~/Scripts/jquery-{version}.js"));

           
            // jQuery Validation
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.validate.js"));

            //microsof ajax

           
                bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                    "~/Scripts/jquery.unobtrusive-ajax.js"));


            //GBZone scripts            
            bundles.Add(new ScriptBundle("~/bundles/GBZone/js").Include(
                      "~/Scripts/gbzone.js"));
            //GBZone scripts            
            bundles.Add(new ScriptBundle("~/bundles/jqueryMask/js").Include(
                      "~/vendor/jqueryMask/jquery.mask.js",
                      "~/vendor/jqueryMask/jquery.mask.min.js"));
        }

    }
}
