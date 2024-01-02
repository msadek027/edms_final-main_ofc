using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SILDMS.Service.Owner;
using SILDMS.Service.Reports;
using SILDMS.Service.Roles;
using SILDMS.Service.Users;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using SILDMS.Web.UI.Models;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Controllers
{
    public class ReportsController : Controller
    {
        readonly IOwnerService _ownerService;
        readonly IRoleService _roleService;
        //readonly IUserService _userService;
        readonly IReportsService _reportService;

        private readonly ILocalizationService _localizationService;
        private ValidationResult respStatus = new ValidationResult();

        private string outStatus = string.Empty;
        private readonly string UserID = string.Empty;
        private string action = string.Empty;
        private string res_code = string.Empty;
        private string res_message = string.Empty;

        public ReportsController(IOwnerService ownerService, IRoleService roleService, IReportsService reportService, ILocalizationService localizationService)
        {
            this._ownerService = ownerService;
            this._roleService = roleService;
            this._reportService = reportService;
            this._localizationService = localizationService;
            //UserID = SILAuthorization.GetUserID();
        }

        //[SILAuthorize]
        public ActionResult ReportByRoleWisePermission()
        {
            return View();
        }

        //[Authorize]
        [HttpPost]
        //[SILLogAttribute]
        //string OwnerID, string RoleID, string UserID, string page, string itemsPerPage, string sortBy
        public async Task<dynamic> ReportByRoleWisePermission(ReportModel reportModel)
        {
            try
            {
                DataTable dt = new DataTable();
                ReportDocument reportDocument = new ReportDocument();

                string ReportPath = Server.MapPath("~/Reports") + "/RoleWisePermission.rpt";
                await Task.Run(() => _reportService.GetReportByRoleWisePermission(reportModel.OwnerID, reportModel.RoleID, UserID, "1", "20", "", out dt));

                reportDocument.Load(ReportPath);
                reportDocument.SetDataSource(dt);
                reportDocument.Refresh();

                //reportDocument.SetParameterValue("rptUser", UserID);
                //reportDocument.SetParameterValue("fromDate", model.BillReceiveFromDate);
                //reportDocument.SetParameterValue("toDate", model.BillReceiveToDate);

                reportDocument.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "RoleWisePermission");
               

                reportDocument.Close();
                reportDocument.Dispose();
            }
            catch (System.Exception ex)
            {

            }

            return View();
        }


        public ActionResult ShowAllDocInfo()
        {
            return View();
        }

        [HttpPost]
        public async Task<dynamic> ShowAllDocInfo(ReportModel reportModel)
        {
            try
            {
                DataTable dt = new DataTable();
                ReportDocument reportDocument = new ReportDocument();

                string ReportPath = Server.MapPath("~/Reports") + "/rptAllHRDocList.rpt";
                await Task.Run(() => _reportService.GetAllHRDocList(reportModel.DocCategory, reportModel.DocType, reportModel.FromDate, reportModel.ToDate, out dt));

                reportDocument.Load(ReportPath);
                reportDocument.SetDataSource(dt);
                reportDocument.Refresh();

                //reportDocument.ExportToHttpResponse(ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, false, "AllDocumentInformation");
                reportDocument.ExportToHttpResponse(reportModel.ReportType == "EXCEL" ? ExportFormatType.ExcelRecord : ExportFormatType.PortableDocFormat,
                  System.Web.HttpContext.Current.Response, true, "AllDocumentInformation");

                reportDocument.Close();
                reportDocument.Dispose();
            }
            catch (System.Exception ex)
            {
                ViewBag.Title = ex.Message;
                //return View("Index");
            }

          return View("Index");
        }

        public ActionResult ReportView()
        {
            return View();
        }
    }
}