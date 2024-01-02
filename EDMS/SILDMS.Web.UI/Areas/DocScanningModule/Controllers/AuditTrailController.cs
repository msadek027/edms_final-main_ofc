using SILDMS.Model.SecurityModule;
using SILDMS.Service.AuditTrail;
using SILDMS.Utillity;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class AuditTrailController : Controller
    {
        private readonly IAuditTrailService _iAuditTrailService;
        private ValidationResult respStatus = new ValidationResult();

        private string outStatus = string.Empty;
        private readonly string UserID = string.Empty;
    
        public AuditTrailController(IAuditTrailService iAuditTrailService)
        {
            _iAuditTrailService = iAuditTrailService;
            UserID = SILAuthorization.GetUserID();
        }

        [Authorize]
        public ActionResult Index()
        { 
            //trturtu
            return View();
        }

        [Authorize]
        public async Task<dynamic> GetAllAuditTrail(string _SearchBy, int page = 1, int itemsPerPage = 20, string sortBy = "", bool reverse = false, string attribute = null, string search = null)
        {
            List<SEC_UserLog> lstLogSearch = null;
            respStatus = await Task.Run(() => _iAuditTrailService.GetAllAuditTrail(_SearchBy, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out lstLogSearch));

            var totalPages = lstLogSearch.Select(o => o.TotalCount).FirstOrDefault();

            return Json(new { respStatus, lstLogSearch, totalPages }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetAllAuditTrailForReport(string _SearchBy, int page = 1, int itemsPerPage = 20, string sortBy = "", bool reverse = false, string attribute = null, string search = null)
        {
            List<SEC_UserLog> lstLogSearch = null;
            respStatus = await Task.Run(() => _iAuditTrailService.GetAllAuditTrailForReport(_SearchBy, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out lstLogSearch));

            var totalPages = lstLogSearch.Select(o => o.TotalCount).FirstOrDefault();

            return Json(new { respStatus, lstLogSearch, totalPages }, JsonRequestBehavior.AllowGet);
        }
	
	}
}