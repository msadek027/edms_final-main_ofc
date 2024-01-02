
using System.Web;
using System.Web.Optimization;

namespace SILDMS.Web.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

          
 
    //<script src="~/Scripts/jquery-2.1.4.min.js"></script>    
  

    //<script src="~/Scripts/bootstrap.min.js"></script>
    //<script src=""></script>

    //<script src=""></script>
    //<script src=""></script>
    //<script src=""></script>
    //<script src=""></script>

    //<script src=""></script>
    //<script src=""></script>

    //<script src=""></script>



            bundles.Add(new StyleBundle("~/bundles/css").Include(
               
                      "~/Content/bootstrap.min.css",
                      "~/Content/sb-admin-2.css",
                      //"~/Content/css/font-awesome.min.css",
                      "~/Content/css/ionicons.min.css",
                      "~/Scripts/AdminLTE/plugins/morris/morris.css",
                      "~/Scripts/AdminLTE/plugins/jvectormap/jquery-jvectormap-1.2.2.css",
                      "~/Content/AdminLTE/AdminLTE.min.css",
                      "~/Content/AdminLTE/skins/_all-skins.min.css",
                      "~/Content/plugin/toaster/toastr.css",
                      "~/Content/css/animate.css",
                      "~/Scripts/AdminLTE/plugins/datepicker/datepicker/css/datepicker.css",
                      "~/Content/plugin/Select2/css/select2.min.css",
                      "~/Content/css/cssTree/jsplumbtoolkit-defaults.css",
                      "~/Content/css/cssTree/jsplumbtoolkit-demo.css",
                     "~/Content/CustomSite.css"
                   
                      ));

            bundles.Add(new ScriptBundle("~/bundles/preScript").Include(
                         "~/Scripts/jquery-{version}.js",
                         "~/Scripts/bootstrap.min.js",
                         "~/Content/plugin/Select2/js/select2.min.js",
                         "~/Scripts/respond.js",
                         "~/Scripts/angular.js",
                         "~/Scripts/angular.min.js",
                         "~/Scripts/angular-Cookie.js",
                         "~/Scripts/angular-cookie.min.js",

                         "~/Scripts/angular-route.min.js",
                         "~/Scripts/angular-animate.js",
                         "~/Scripts/angular-touch.js",
                         "~/Scripts/ui-bootstrap-tpls-2.2.0.min.js",
                         "~/Scripts/angular-drag-and-drop-lists.min.js",
                         "~/Scripts/uiSelect2/select2.js",
                         "~/Scripts/jquery.validate.min.js",
                         "~/Scripts/smart-table.js",
                         "~/Content/plugin/toaster/toastr.js",
                         "~/Scripts/angular-validation/angular-validation.min.js",
                         "~/Scripts/AdminLTE/plugins/datepicker/datepicker/js/bootstrap-datepicker.js",
                         "~/Scripts/App/custom.js",
                         "~/Scripts/jsplumb.js",
                         //"~/Scripts/demo.js",
                         "~/Scripts/demo-list.js",
                         "~/Scripts/App/app.js"
                         ));

             bundles.Add(new ScriptBundle("~/bundles/postScript").Include(                 
                      "~/Scripts/AdminLTE/app.js",
                      "~/Scripts/AdminLTE/plugins/sparkline/jquery.sparkline.min.js",
                      "~/Scripts/AdminLTE/plugins/slimScroll/jquery.slimscroll.min.js"
                      ));


            BundleTable.EnableOptimizations = false;

        }
    }
}
