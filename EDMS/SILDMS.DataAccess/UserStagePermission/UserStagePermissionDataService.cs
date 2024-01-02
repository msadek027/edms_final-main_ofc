using SILDMS.DataAccessInterface.UserStagePermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.DataAccess.UserStagePermission
{
    public class UserStagePermissionDataService : IUserStagePermissionDataService
    {
        private readonly string res_code = "@res_code";
        private readonly string res_message = "@res_message";

        public List<WFM_ProcessStageMap> GetStagesWithPermission( string _DocTypeID, string _UserID)
        {
            List<WFM_ProcessStageMap> objs = new List<WFM_ProcessStageMap>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetUserStagePermissionForStages"))
            {
               
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, _DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, _UserID);

                try
                {

                    var ds = db.ExecuteDataSet(dbCommandWrapper);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        objs = dt1.AsEnumerable().Select(reader => new WFM_ProcessStageMap
                        {
                           
                            StageID = reader.GetInt32("StageID"),
                            StageName = reader.GetString("StageName"),
                            IsChecked = reader.GetString("UserStagePermissionID") == "" ? false : true,
                            HaveMk = reader.GetString("UserStagePermissionID") == "" ? false : reader.GetBoolean("MkPermission"),
                            HaveCk = reader.GetString("UserStagePermissionID") == "" ? false : reader.GetBoolean("CkPermission"),
                            NotifyMk = reader.GetString("UserStagePermissionID") == "" ? false: reader.GetString("UserID") == "" || reader.GetString("UserID") == null ? true : false,//RoleBase
                            NotifyCk = reader.GetString("UserStagePermissionID") == "" ? false: reader.GetString("UserID") != "" && reader.GetString("UserID") != null ? true : false


                        }).ToList();
                    }
                }

                catch (Exception e){ 
                
                }

            }
            return objs;
        }

        public void SetStagesWithPermission(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _UserID, string _SessionUserID, List<WFM_ProcessStageMap> objs, out string _res_code, out string _res_message)
        {
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                DataTable dtable = new DataTable();
                dtable.Columns.Add("UserID");
                dtable.Columns.Add("StageID");
                dtable.Columns.Add("MkPermission");
                dtable.Columns.Add("CkPermission");

                foreach (var item in objs)
                {
                    DataRow objDataRow = dtable.NewRow();
                    objDataRow[0] = _UserID;
                    objDataRow[1] = item.StageID;
                    objDataRow[2] = 1;
                    objDataRow[3] = 1;
                    dtable.Rows.Add(objDataRow);
                }

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_AddStagePermission"))
                {
                    db.AddInParameter(dbCommandWrapper, "@Permissions", SqlDbType.Structured, dtable);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, _UserID);
                    //db.AddInParameter(dbCommandWrapper, "@SessionUserID", SqlDbType.VarChar, _SessionUserID);                    
                    //db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, _OwnerID);
                    //db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.VarChar, _DocCategoryID);
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.VarChar, _DocTypeID);

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
    }
}
