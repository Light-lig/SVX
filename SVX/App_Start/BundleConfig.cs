using System.Web;
using System.Web.Optimization;

namespace SVX
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                    "~/assets/vendor_components/jquery/dist/jquery.min.js",
                           "~/assets/vendor_components/jquery/dist/jquery.min.js",
                           "~/assets/vendor_components/popper/dist/popper.min.js",
                         "~/assets/vendor_components/bootstrap/dist/js/bootstrap.min.js",
                          "~/assets/vendor_components/jquery-slimscroll/jquery.slimscroll.min.js",
                           "~/assets/vendor_components/fastclick/lib/fastclick.js",
                           "~/Scripts/js/template.js",
                           "~/Scripts/js/demo.js"));


            bundles.Add(new StyleBundle("~/assets/").Include(
                      "~/assets/vendor_components/bootstrap/dist/css/bootstrap.min.css",
                      "~/Content/css/bootstrap-extend.css",
                      "~/Content/css/master_style.css",
                      "~/Content/css/skins/_all-skins.css"));
        }
    }
}
