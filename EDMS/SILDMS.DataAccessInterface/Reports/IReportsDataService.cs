using System;
using System.Data;
using System.Collections.Generic;

namespace SILDMS.DataAccessInterface.Reports
{
    public interface IReportsDataService
    {
        DataTable GetReportByRoleWisePermission(string OwnerID, string RoleID, string UserID, string page, string itemsPerPage, string sortBy);


        DataTable GetAllHRDocList(string DocCategory, string DocType, string FromDate, string ToDate);
    }
}
