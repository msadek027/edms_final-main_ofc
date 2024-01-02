using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.AuditTrail;
using SILDMS.Model.SecurityModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccess.AuditTrail
{
    public class AuditTrailDataService : IAuditTrailDataService
    {
        private readonly string spStatusParam = "@p_Status";
        public List<SEC_UserLog> GetAllAuditTrail(string _SearchBy, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<SEC_UserLog> logList = new List<SEC_UserLog>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            //using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetAllAuditTrail"))
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetDocAuditTrail"))
            {
                db.AddInParameter(dbCommandWrapper, "@SearchBy", SqlDbType.NVarChar, _SearchBy);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, UserID);
                db.AddInParameter(dbCommandWrapper, "@page", SqlDbType.Int, page);
                db.AddInParameter(dbCommandWrapper, "@itemsPerPage", SqlDbType.Int, itemsPerPage);
                db.AddInParameter(dbCommandWrapper, "@sortBy", SqlDbType.NVarChar, sortBy);
                db.AddInParameter(dbCommandWrapper, "@reverse", SqlDbType.Int, reverse);
                db.AddInParameter(dbCommandWrapper, "@attribute", SqlDbType.NVarChar, attribute);
                db.AddInParameter(dbCommandWrapper, "@search", SqlDbType.NVarChar, search);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.NVarChar, 10);
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        logList = dt1.AsEnumerable().Select(reader => new SEC_UserLog
                        {
                            UserAction = reader.GetString("UserAction"),
                            UserID = reader.GetString("UserID"),
                            UserFullName = reader.GetString("UserFullName"),
                            DocumentID = reader.GetString("DocumentID"),
                            MetaValue = reader.GetString("MetaValue"),
                            IdentificationAttribute = reader.GetString("IdentificationAttribute"),
                            Remarks = reader.GetString("Remarks"),
                            ActionEventTime = reader.GetString("ActionEventTime"),
                            DocumentStatus = reader.GetString("DocumentStatus"),
                            ObsoleteTime = reader.GetString("ObsoleteTime"),
                            TotalCount = reader.GetString("TotCol"),
                        }).ToList();
                    }
                }
            }

            return logList;
        }


        public List<SEC_UserLog> GetAllAuditTrailForReport(string _SearchBy, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<SEC_UserLog> logList = new List<SEC_UserLog>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetAllAuditTrailForReport"))
            {
                db.AddInParameter(dbCommandWrapper, "@SearchBy", SqlDbType.NVarChar, _SearchBy);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, UserID);
                db.AddInParameter(dbCommandWrapper, "@page", SqlDbType.Int, page);
                db.AddInParameter(dbCommandWrapper, "@itemsPerPage", SqlDbType.Int, itemsPerPage);
                db.AddInParameter(dbCommandWrapper, "@sortBy", SqlDbType.NVarChar, sortBy);
                db.AddInParameter(dbCommandWrapper, "@reverse", SqlDbType.Int, reverse);
                db.AddInParameter(dbCommandWrapper, "@attribute", SqlDbType.NVarChar, attribute);
                db.AddInParameter(dbCommandWrapper, "@search", SqlDbType.NVarChar, search);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.NVarChar, 10);
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        logList = dt1.AsEnumerable().Select(reader => new SEC_UserLog
                        {
                            UserAction = reader.GetString("UserAction"),
                            UserID = reader.GetString("UserID"),
                            UserFullName = reader.GetString("UserFullName"),
                            DocumentID = reader.GetString("DocumentID"),
                            MetaValue = reader.GetString("MetaValue"),
                            IdentificationAttribute = reader.GetString("IdentificationAttribute"),
                            Remarks = reader.GetString("Remarks"),
                            ActionEventTime = reader.GetString("ActionEventTime"),
                            DocumentStatus = reader.GetString("DocumentStatus"),
                            ObsoleteTime = reader.GetString("ObsoleteTime"),
                            TotalCount = reader.GetString("TotCol"),
                        }).ToList();
                    }
                }
            }

            return logList;
        }
    }
}
