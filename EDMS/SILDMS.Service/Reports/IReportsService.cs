using System.Data;
using SILDMS.Utillity;

namespace SILDMS.Service.Reports
{
    public interface IReportsService
    {
        ValidationResult GetReportByRoleWisePermission(string OwnerID, string RoleID, string UserID, string page, string itemsPerPage, string sortBy, out DataTable reportList);

        ValidationResult GetAllHRDocList(string DocCategory, string DocType, string FromDate, string ToDate, out DataTable dt);
    }
}
