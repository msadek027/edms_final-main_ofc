using SILDMS.DataAccessInterface.MasterManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Data.Common;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.DataAccess.MasterManager
{
    public class MasterManagerDataService : IMasterManagerDataService
    {
        private readonly string res_code = "@res_code";
        private readonly string res_message = "@res_message";
        public MasterManagerDataService()
        {

        }

        public void AddItemToMaster(string action,MasterData obj, int masterId, string userID, out string _res_code, out string _res_message)
        {

            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;
                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("Master_ManageData"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@action", SqlDbType.NVarChar, action);
                    db.AddInParameter(dbCommandWrapper, "@ID", SqlDbType.NVarChar, obj.ID);
                    db.AddInParameter(dbCommandWrapper, "@Name", SqlDbType.VarChar, obj.Name);
                    db.AddInParameter(dbCommandWrapper, "@MasterID", SqlDbType.VarChar, masterId);
                    db.AddOutParameter(dbCommandWrapper, res_code, SqlDbType.VarChar, 10);
                    db.AddOutParameter(dbCommandWrapper, res_message, SqlDbType.VarChar, 100);
                    db.ExecuteNonQuery(dbCommandWrapper);
                    _res_code = db.GetParameterValue(dbCommandWrapper, res_code).ToString();
                    _res_message = db.GetParameterValue(dbCommandWrapper, res_message).ToString();
                }
            }
            catch (Exception ex)
            {
                _res_code = "0";
                _res_message = "Procedure Exeution Error";
            }
        }

        public List<MasterData> GetMasterDataByTable(int id, string userID)
        {

            List<MasterData> objs = new List<MasterData>();

            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;

            using (var dbCommandWrapper = db.GetStoredProcCommand("Master_GetMasterByTable"))
            {
                db.AddInParameter(dbCommandWrapper, "@Id", SqlDbType.Int, id);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        objs=ds.Tables[0].AsEnumerable().Select(c => new MasterData
                        {
                            ID = c.GetInt32("ID"),
                            Name = c.GetString("Name")
                        }).ToList();
                    }
                }
            }
            return objs;
        }

        public List<MasterTableTracker> GetMasterForTableType()
        {
            List<MasterTableTracker> b = new List<MasterTableTracker>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetSqlStringCommand("SELECT * FROM MasterTableTracker"))
            {

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    b = dt1.AsEnumerable().Select(reader => new MasterTableTracker
                    {
                        MasterTableID = reader.GetInt32("MasterTableID"),
                        MasterTableName = reader.GetString("MasterTableName")
                    }).ToList();
                }

            }

            return b;
        }
    }
}
