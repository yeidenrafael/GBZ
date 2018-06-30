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
            bundles.Add(new StyleBundle("~/bundles/homer/css").Include(
                      "~/Content/style.css", new CssRewriteUrlTransform()));
            // Animate.css
            bundles.Add(new StyleBundle("~/bundles/animate/css").Include(
                      "~/Vendor/animate.css/animate.min.css"));

            // Pe-icon-7-stroke
            bundles.Add(new StyleBundle("~/bundles/peicon7stroke/css").Include(
                      "~/Icons/pe-icon-7-stroke/css/pe-icon-7-stroke.css", new CssRewriteUrlTransform()));

            // Font Awesome icons style
            bundles.Add(new StyleBundle("~/bundles/font-awesome/css").Include(
                      "~/Vendor/fontawesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));


            // Bootstrap style
            bundles.Add(new StyleBundle("~/bundles/bootstrap/css")
                .Include("~/Content/bootstrap.min.css",new CssRewriteUrlTransform())
                );


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
