using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using SILDMS.Model.CBPSModule;
using SILDMS.DataAccessInterface.Reports;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace SILDMS.DataAccess.Reports
{
    public class ReportsDataService : IReportsDataService
    {
        //private readonly string spErrorParam = "@p_Error";
        //private readonly string spStatusParam = "@p_Status";

        public DataTable GetReportByRoleWisePermission(string OwnerID, string RoleID, string UserID, string page, string itemsPerPage, string sortBy)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            DataTable dt;

            try
            {
                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetReportByRoleWiseSearchParam"))
                {
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.VarChar, OwnerID);
                    db.AddInParameter(dbCommandWrapper, "@RoleID", SqlDbType.VarChar, RoleID);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, UserID);
                    db.AddInParameter(dbCommandWrapper, "@page", SqlDbType.Int, page);
                    db.AddInParameter(dbCommandWrapper, "@itemsPerPage", SqlDbType.Int, itemsPerPage);
                    db.AddInParameter(dbCommandWrapper, "@sortBy", SqlDbType.VarChar, sortBy);

                    dbCommandWrapper.CommandTimeout = 300;

                    var ds = db.ExecuteDataSet(dbCommandWrapper);
                    dt = ds.Tables[0];
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dt;
        }



        public DataTable GetAllHRDocList(string DocCategory, string DocType, string FromDate, string ToDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            DataTable dt;
            if (!string.IsNullOrEmpty(FromDate))
            {
                string[] fromdate = FromDate.Split('/');
                FromDate = fromdate[2] + "-" + fromdate[1] + "-" + fromdate[0];
            }
            if (!string.IsNullOrEmpty(ToDate))
            {
                string[] todate = ToDate.Split('/');
                ToDate = todate[2] + "-" + todate[1] + "-" + todate[0];
            }
            try
            {
                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetAllHRDocList"))
                {
                    db.AddInParameter(dbCommandWrapper, "@DocCategory", SqlDbType.VarChar, DocCategory);
                    db.AddInParameter(dbCommandWrapper, "@DocType", SqlDbType.VarChar, DocType);
                    db.AddInParameter(dbCommandWrapper, "@FromDate", SqlDbType.VarChar, FromDate);
                    db.AddInParameter(dbCommandWrapper, "@ToDate", SqlDbType.VarChar, ToDate);
                    dbCommandWrapper.CommandTimeout = 10000;

                    var ds = db.ExecuteDataSet(dbCommandWrapper);
                    dt = ds.Tables[0];
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dt;
        }
    }
}
