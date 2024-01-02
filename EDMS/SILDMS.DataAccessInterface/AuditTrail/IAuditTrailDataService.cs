using SILDMS.Model.SecurityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccessInterface.AuditTrail
{
    public interface IAuditTrailDataService
    {
        List<SEC_UserLog> GetAllAuditTrail(string _SearchBy, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber);

        List<SEC_UserLog> GetAllAuditTrailForReport(string _SearchBy, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber);
    }
}
