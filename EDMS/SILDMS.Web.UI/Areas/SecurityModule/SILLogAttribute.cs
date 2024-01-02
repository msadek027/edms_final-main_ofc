using SILDMS.Model.SecurityModule;
using SILDMS.Service.UserAccessLog;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace SILDMS.Web.UI.Areas.SecurityModule
{
    public class SILLogAttribute : ActionFilterAttribute
    {
        private readonly string UserID = string.Empty;
        protected DateTime start_time;
        private IUserAccessLogService userAccessLogService;

        public SILLogAttribute() : this(DependencyResolver.Current.GetService(typeof(IUserAccessLogService)) as IUserAccessLogService)
        {
            UserID = SILAuthorization.GetUserID();
        }

        public SILLogAttribute(IUserAccessLogService userAccessLogService)
        {
            // TODO: Complete member initialization
            this.userAccessLogService = userAccessLogService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            start_time = DateTime.Now;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {

            //var myValue = (((JsonResult)filterContext.Result).Data).ToString();

            //var sda = myValue.AsEnumerable();

            //var json = new JavaScriptSerializer();
            //var data = json.Deserialize<Dictionary<string, Dictionary<string, string>>[]>(myValue);
         
            var d = HttpContext.Current.Response;
            SEC_UserLog _userLog = new SEC_UserLog();
            RouteData route_data = filterContext.RouteData;
            _userLog.ActionExecuteTime = (DateTime.Now - start_time).ToString();
            _userLog.ActionUrl = (route_data.DataTokens["area"] as string) + "/" + (route_data.Values["controller"] as string) + "/" + route_data.Values["action"] as string;
            _userLog.UserID = SILAuthorization.GetUserID();
            _userLog.UsedIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];//GetIPAddress.LocalIPAddress();
            _userLog.UserAction = route_data.Values["action"] as string;
            _userLog.ActionEventTime = DateTime.Now.ToString();

            _userLog.LoggID = filterContext.Controller.ViewBag.LoggID;
            _userLog.LoggResult = filterContext.Controller.ViewBag.LoggResult;
            _userLog.LoggAction = filterContext.Controller.ViewBag.LoggAction;
            _userLog.LookupTable = filterContext.Controller.ViewBag.LookupTable;

            userAccessLogService.ManipulateUserAccessLog(_userLog);
        }

        public override void OnActionExecuted(ActionExecutedContext _context)
        {           
            var d = HttpContext.Current.Response;          
            SEC_UserLog _userLog = new SEC_UserLog();
            RouteData route_data = _context.RouteData;
            _userLog.ActionExecuteTime = (DateTime.Now - start_time).ToString();
            _userLog.ActionUrl = (route_data.DataTokens["area"] as string) + "/" + (route_data.Values["controller"] as string) + "/" + route_data.Values["action"] as string;
            _userLog.UserID = SILAuthorization.GetUserID();
            _userLog.LoggID = "";
            _userLog.LoggAction ="";
            _userLog.LoggResult = "";
            _userLog.UsedIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];//GetIPAddress.LocalIPAddress();
            _userLog.UserAction = route_data.Values["action"] as string;
            _userLog.ActionEventTime = DateTime.Now.ToString();
          //  userAccessLogService.ManipulateUserAccessLog(_userLog);
        }
    }
}