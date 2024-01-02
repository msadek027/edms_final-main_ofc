using SILDMS.Model.SecurityModule;
using SILDMS.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.AuditTrail
{
    public interface IAuditTrailService
    {

        ValidationResult GetAllAuditTrail(string _SearchBy, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<SEC_UserLog> lstLogSearch);

        ValidationResult GetAllAuditTrailForReport(string _SearchBy, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<SEC_UserLog> lstLogSearch);
    }
}
