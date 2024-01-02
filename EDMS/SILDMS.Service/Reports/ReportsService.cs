using System;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.DataAccessInterface.Reports;
using System.Data;

namespace SILDMS.Service.Reports
{
    public class ReportsService : IReportsService
    {
        private readonly IReportsDataService _reportDataService;
        private readonly ILocalizationService _localizationService;
        //private string p_Status = string.Empty;

        public ReportsService(IReportsDataService reportDataService, ILocalizationService localizationService)
        {
            _reportDataService = reportDataService;
            _localizationService = localizationService;
        }

        public ValidationResult GetReportByRoleWisePermission(string OwnerID, string RoleID, string UserID, string page, string itemsPerPage, string sortBy, out DataTable reportList)
        {
            reportList = _reportDataService.GetReportByRoleWisePermission(OwnerID, RoleID, UserID, page, itemsPerPage, sortBy);

            //if (p_Status.Length > 0)
            //{
            //    return new ValidationResult(p_Status, _localizationService.GetResource(p_Status));
            //}

            return ValidationResult.Success;
        }



        public ValidationResult GetAllHRDocList(string DocCategory, string DocType, string FromDate, string ToDate, out DataTable dt)
        {
            dt = _reportDataService.GetAllHRDocList(DocCategory, DocType, FromDate, ToDate);            
            return ValidationResult.Success;
        }
    }
}
