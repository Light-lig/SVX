using System.Web;
using System.Web.Optimization;

namespace SVX
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
           
            bundles.Add(new ScriptBundle("~/bundles/mensaje").Include(
                "~/assets/vendor_components/jquery/dist/jquery.min.js",
                "~/assets/vendor_components/sweetalert/sweetalert.min.js",
                "~/Scripts/alerts.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/Scripts").Include(
                "~/assets/vendor_components/dropzone/dropzone.js",
                "~/assets/vendor_components/bootstrap/dist/js/bootstrap.bundle.min.js",
                "~/assets/vendor_components/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/assets/vendor_components/fastclick/lib/fastclick.js",
                "~/assets/vendor_components/Magnific-Popup-master/dist/jquery.magnific-popup.min.js",
                "~/assets/vendor_components/datatable/datatables.min.js",
                "~/Scripts/js/template.js",
                "~/Scripts/js/demo.js",
                  "~/Scripts/main.js",
                "~/assets/vendor_components/bootstrap-select/bootstrap-select.min.js",
                "~/assets/vendor_components/sweetalert/sweetalert.min.js",
                                "~/assets/vendor_components/sweetalert/jquery.sweet-alert.custom.js",
                "~/assets/vendor_components/jquery-toast-plugin-master/src/jquery.toast.js",
                                      "~/assets/vendor_components/inputmask/dist/inputmask/inputmask.js",
                      "~/assets/vendor_components/inputmask/dist/inputmask/inputmask.extensions.js",
                "~/assets/vendor_components/inputmask/dist/inputmask/jquery.inputmask.js"
               ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryVal").Include(
                        "~/assets/vendor_components/jquery-validation-1.17.0/dist/jquery.validate.min.js"

                 ));

        
            bundles.Add(new ScriptBundle("~/bundles/steper").Include(
                "~/assets/vendor_components/jquery-steps-master/build/jquery.steps.js",
                "~/assets/vendor_components/jquery-validation-1.17.0/dist/jquery.validate.min.js",
                "~/Scripts/js/pages/steps.js"
                ));

            bundles.Add(new StyleBundle("~/assets/").Include(

                     "~/assets/vendor_components/font-awesome/css/all.css",
                      "~/assets/vendor_components/bootstrap/dist/css/bootstrap.min.css",
                         "~/assets/vendor_components/bootstrap-select/bootstrap-select.min.css",
                      "~/assets/vendor_components/datatable/datatables.min.css",
                     "~/assets/vendor_components/dropzone/dropzone.css",
                      "~/Content/css/bootstrap-extend.css",
                      "~/Content/css/master_style.css",
                       "~/assets/vendor_components/Magnific-Popup-master/dist/magnific-popup.css",
                      "~/Content/css/skins/_all-skins.css",
                          "~/assets/vendor_components/sweetalert/sweetalert.css",
                    "~/assets/vendor_components/sweetalert/darks.css",
                    "~/Content/estilos.css",
                      "~/assets/vendor_components/jquery-toast-plugin-master/src/jquery.toast.css"

                ));
        }
    }
}
