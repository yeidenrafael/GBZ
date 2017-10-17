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
            

            // Homer script
            bundles.Add(new ScriptBundle("~/bundles/homer/js").Include(
                      "~/Vendor/metisMenu/dist/metisMenu.min.js",
                      "~/Scripts/homer.js"));
            
            // Animate.css
            bundles.Add(new StyleBundle("~/bundles/animate/css").Include(
                      "~/Vendor/animate.css/animate.min.css"));

            // Pe-icon-7-stroke
            bundles.Add(new StyleBundle("~/bundles/peicon7stroke/css").Include(
                      "~/Icons/pe-icon-7-stroke/css/pe-icon-7-stroke.css", new CssRewriteUrlTransform()));

            // Font Awesome icons style
            bundles.Add(new StyleBundle("~/bundles/font-awesome/css").Include(
                      "~/Vendor/fontawesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));

            //footable
            bundles.Add(new StyleBundle("~/bundles/footable/css").Include(
                       "~/Vendor/fooTable/css/footable.bootstrap.min.css", new CssRewriteUrlTransform()));



            // Bootstrap style
            bundles.Add(new StyleBundle("~/bundles/bootstrap/css")
                .Include("~/Vendor/bootstrap/css/bootstrap.min.css",new CssRewriteUrlTransform())
                );

           

            // Bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap/js").Include(                     
                      "~/Vendor/bootstrap/js/bootstrap.min.js"));
            

            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery/js").Include(
                      "~/Vendor/jquery/dist/jquery.min.js"));


            // jQuery UI
            bundles.Add(new ScriptBundle("~/bundles/jqueryui/js").Include(
                      "~/Vendor/jquery-ui/jquery-ui.min.js"));

            // jQuery Validation
            bundles.Add(new ScriptBundle("~/bundles/validation/js").Include(
                      "~/Vendor/jquery-validation/jquery.validate.min.js"));


            // Grid footable
            bundles.Add(new ScriptBundle("~/bundles/footable/js").Include(                      
                      "~/Vendor/fooTable/js/footable.min.js"
                       ));
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
