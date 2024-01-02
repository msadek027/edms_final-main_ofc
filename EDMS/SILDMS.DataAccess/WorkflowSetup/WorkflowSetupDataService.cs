using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.WorkflowSetup;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccess.WorkflowSetup
{
    public class WorkflowSetupDataService : IWorkflowSetupDataService
    {
        private readonly string spStatusParam = "@p_Status";
        private readonly string res_code = "@res_code";
        private readonly string res_message = "@res_message";

        public List<WFM_ProcessStageMap> GetALLStagesForType(string docCategoryID, string ownerID, string docTypeID)
        {
            List<WFM_ProcessStageMap> objs = new List<WFM_ProcessStageMap>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetAllMappedstages"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, ownerID);
                db.AddInParameter(dbCommandWrapper, "@docCategoryID", SqlDbType.NVarChar, docCategoryID);
                db.AddInParameter(dbCommandWrapper, "@docTypeID", SqlDbType.NVarChar, docTypeID);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    objs = dt1.AsEnumerable().Select(reader => new WFM_ProcessStageMap
                    {
                        StageID = reader.GetInt32("StageID"),
                        StageMapID = reader.GetString("StageMapID"),
                        StageName = reader.GetString("StageName"),
                        IsChecked = reader.GetBoolean("IsChecked"),
                        StageSL = reader.GetBoolean("IsChecked") ? reader.GetInt32("StageMapSL") : reader.GetInt32("StageSerial"),
                        //StagePosition = reader.GetBoolean("IsChecked") ? reader.GetString("StageMapPosition") : reader.GetString("DefaultPosition"),
                        
                        HaveMk = reader.GetBoolean("IsChecked") ? reader.GetBoolean("HaveMk") : reader.GetBoolean("DefaultHaveMk"),
                        HaveCk = reader.GetBoolean("IsChecked") ? reader.GetBoolean("HaveCk") : reader.GetBoolean("DefaultHaveCk"),
                        NotifyMk = reader.GetBoolean("IsChecked") ? reader.GetBoolean("NotifyMk") : reader.GetBoolean("DefaultNotifyMk"),
                        NotifyCk = reader.GetBoolean("IsChecked") ? reader.GetBoolean("NotifyCk") : reader.GetBoolean("DefaultNotifyCk")
                    }).ToList();
                }
            }

            return objs;
        }


        public List<int> GetDocumentWisePermission(string DocPropertyID)
        {
            List<int> objs = new List<int>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetSqlStringCommand("SELECT stagemapid FROM wfm_StageWiseDocumentPermission where docpropertyID='" + DocPropertyID + "'"))
            {
                var ds = db.ExecuteDataSet(dbCommandWrapper);
              


                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    //objs = dt1.AsEnumerable().Select(reader => new WFM_ProcessStageMap
                    //{
                    //    StageID = reader.GetInt32("StageID"),
                    //    StageMapID = reader.GetString("StageMapID"),
                    //    StageName = reader.GetString("StageName")
                    //}).ToList();
                    objs = dt1.AsEnumerable().Select(s => s.Field<int>("StageMapID")).ToList <int>();
                }
            }

            return objs;
        }
        public List<WFM_ProcessStageMap> GetChildStages(int StageMapID)
        {
            List<WFM_ProcessStageMap> objs = new List<WFM_ProcessStageMap>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetChildStages"))
            {
                db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.Int, StageMapID);
      

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    objs = dt1.AsEnumerable().Select(reader => new WFM_ProcessStageMap
                    {
                        StageID = reader.GetInt32("StageID"),
                        StageMapID = reader.GetString("StageMapID"),
                        StageName = reader.GetString("StageName")
                    }).ToList();
                }
            }

            return objs;
        }

        public void SetStagesForType(IEnumerable<WFM_ProcessStageMap> prms, string userID, string ownerID, string docCategoryID, string docTypeID, out string _res_code, out string _res_message)
        {
            DataTable stageDataTable = new DataTable();
            stageDataTable.Columns.Add("OwnerID");
            stageDataTable.Columns.Add("DocCategoryID");
            stageDataTable.Columns.Add("DocTypeID");
            stageDataTable.Columns.Add("StageID");

            stageDataTable.Columns.Add("StageMapSL");
            stageDataTable.Columns.Add("StageMapPosition");
            stageDataTable.Columns.Add("NextStage");
            stageDataTable.Columns.Add("AutoIssue");

            stageDataTable.Columns.Add("AutoReceive");
            stageDataTable.Columns.Add("HaveMk");
            stageDataTable.Columns.Add("HaveCk");
            stageDataTable.Columns.Add("NotifyMk");

            stageDataTable.Columns.Add("NotifyCk");
            stageDataTable.Columns.Add("SetOn");
            stageDataTable.Columns.Add("SetBy");

            int i = 1;
            foreach (var item in prms)
            {
                DataRow objDataRow = stageDataTable.NewRow();
                objDataRow[0] = item.OwnerID;
                objDataRow[1] = item.DocCategoryID;
                objDataRow[2] = item.DocTypeID;
                objDataRow[3] = item.StageID;
                objDataRow[4] = i;
                objDataRow[5] = i == 1 ? "First" : (i == prms.Count() ? "Last" : "Midle");
                objDataRow[6] = null;
                objDataRow[7] = 0;
                objDataRow[8] = 0;
                objDataRow[9] = item.HaveMk;
                objDataRow[10] = item.HaveCk;
                objDataRow[11] = item.NotifyMk;
                objDataRow[12] = item.NotifyCk;
                objDataRow[13] = "";
                objDataRow[14] = item.SetBy;
                stageDataTable.Rows.Add(objDataRow);

                i++;
            }

            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_SetStagesForType"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, userID);
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, ownerID);
                    db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, docCategoryID);
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, docTypeID);
                    db.AddInParameter(dbCommandWrapper, "@StageMaps", SqlDbType.Structured, stageDataTable);

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

        public WFM_VM_ParallelStageProperty GetALLPStageProperty(string docCategoryID, string ownerID, string docTypeID, int stageMapID)
        {
            WFM_VM_ParallelStageProperty tpList = new WFM_VM_ParallelStageProperty();

            List<WFM_ProcessStageMap> a = new List<WFM_ProcessStageMap>();

            List<WFM_ProcessStageMap> b = new List<WFM_ProcessStageMap>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetAllPStageInfo"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, ownerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, docCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, docTypeID);
                db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.Int, stageMapID);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    a = dt1.AsEnumerable().Select(reader => new WFM_ProcessStageMap
                    {
                        StageID = reader.GetInt32("StageID"),
                        StageName = reader.GetString("StageName"),
                        StageSL=reader.GetInt32("StageSerial"),
                        NumberOFStage=reader.GetInt32("NumberOFStage")
                    }).ToList();
                    a=a.Where(c => c.StageSL > 1 && c.StageSL < c.NumberOFStage).ToList();
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    DataTable dt2 = ds.Tables[1];
                    b = dt2.AsEnumerable().Select(reader => new WFM_ProcessStageMap
                    {
                        StageMapID = reader.GetString("StageMapID"),
                        StageName = reader.GetString("StageName")
                    }).ToList();
                }

                if (ds.Tables[2].Rows.Count > 0)
                {
                    tpList.IsConfigured = true;
                    tpList.CurrentPStage = Convert.ToInt32(ds.Tables[2].Rows[0][0]);
                    tpList.CurrentPEndStage = Convert.ToInt32(ds.Tables[2].Rows[0][1]);
                }
                else
                {
                    tpList.IsConfigured = false;
                }
            }

            tpList.ParallelStages = a;
            tpList.ParallelEndStages = b;

            return tpList;
        }

        public void SaveParallelStage(string userID, string docCategoryID, string ownerID, string docTypeID, int ProcessingStageId, int ParallelStageID, int EndStageMapID, out string _res_code, out string _res_message)
        {
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_SaveParallelStage"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, userID);
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, ownerID);
                    db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, docCategoryID);
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, docTypeID);
                    db.AddInParameter(dbCommandWrapper, "@ProcessingStageId", SqlDbType.Int, ProcessingStageId);
                    db.AddInParameter(dbCommandWrapper, "@ParallelStageID", SqlDbType.Int, ParallelStageID);
                    db.AddInParameter(dbCommandWrapper, "@EndStageMapID", SqlDbType.Int, EndStageMapID);

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

        public List<WFM_DocStageProperty> GetALLStageProperty(string docCategoryID, string ownerID, string docTypeID, int stageMapID)
        {
            List<WFM_DocStageProperty> tpList = new List<WFM_DocStageProperty>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetSqlStringCommand("SELECT * FROM WFM_DocTypeProperty where OwnerID='" + ownerID + "' AND DocCategoryID='" + docCategoryID + "' AND DocTypeID='" + docTypeID + "' AND StageMapID='" + stageMapID + "'"))
            {
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    tpList = dt1.AsEnumerable().Select(reader => new WFM_DocStageProperty
                    {
                        DocTypePropertyID = reader.GetString("DocTypePropertyID"),
                        OwnerID = reader.GetString("OwnerID"),
                        DocCategoryID = reader.GetString("DocCategoryID"),
                        DocTypeID = reader.GetString("DocTypeID"),
                        StageMapID = reader.GetInt32("StageMapID"),
                        PropertyName = reader.GetString("PropertyName"),
                        PropertyType = reader.GetString("PropertyType"),
                        PropertySL = reader.GetInt32("PropertySL"),
                        IsRequired = reader.GetBoolean("IsRequired"),
                        SetBy = reader.GetString("SetBy")
                    }).ToList();
                }
            }

            return tpList;
        }

        public void AddStageProperty(WFM_DocStageProperty tp, string action, out string _res_code, out string _res_message)
        {
            DataTable tableTypeDataTable = new DataTable();

            if (tp.PropertyType == "List")
            {
                tableTypeDataTable = new DataTable();
                tableTypeDataTable.Columns.Add("FieldTitle");
                tableTypeDataTable.Columns.Add("DataType");
                tableTypeDataTable.Columns.Add("MaxSize");
                tableTypeDataTable.Columns.Add("Master");

                foreach (var item in tp.TableProperties)
                {
                    DataRow objDataRow = tableTypeDataTable.NewRow();

                    objDataRow[0] = item.FieldTitle;
                    objDataRow[1] = item.DataType;
                    objDataRow[2] = item.MaxSize;
                    objDataRow[3] = Convert.ToInt32(item.Master);

                    tableTypeDataTable.Rows.Add(objDataRow);
                }
            }

            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_SetPropertyForStage"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@DocTypePropertyID", SqlDbType.NVarChar, tp.DocTypePropertyID);
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, tp.OwnerID);
                    db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, tp.DocCategoryID);
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, tp.DocTypeID);
                    db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.Int, tp.StageMapID);
                    db.AddInParameter(dbCommandWrapper, "@PropertyName", SqlDbType.NVarChar, tp.PropertyName);
                    db.AddInParameter(dbCommandWrapper, "@PropertyType", SqlDbType.NVarChar, tp.PropertyType);
                    db.AddInParameter(dbCommandWrapper, "@PropertySL", SqlDbType.Int, tp.PropertySL);
                    db.AddInParameter(dbCommandWrapper, "@IsRequired", SqlDbType.Bit, tp.IsRequired);
                    db.AddInParameter(dbCommandWrapper, "@action", SqlDbType.NVarChar, action);
                    db.AddInParameter(dbCommandWrapper, "@SetBy", SqlDbType.NVarChar, tp.SetBy);

                    if (tp.PropertyType == "List")
                    {
                        db.AddInParameter(dbCommandWrapper, "@TableType", SqlDbType.Structured, tableTypeDataTable);
                    }

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

        
        public List<DSM_DocProperty> GetAllDocuments(string DocPropertyId, string userID, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<DSM_DocProperty> docPropertyList = new List<DSM_DocProperty>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetAllDocumentsForStage"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, userID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.VarChar, DocPropertyId);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);

                // Execute SP.
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        docPropertyList = dt1.AsEnumerable().Select(reader => new DSM_DocProperty
                        {
                            DocPropertyID = reader.GetString("DocPropertyID"),
                            DocCategoryID = reader.GetString("DocCategoryID"),
                            OwnerLevelID = reader.GetString("OwnerLevelID"),
                            OwnerID = reader.GetString("OwnerID"),
                            DocTypeID = reader.GetString("DocTypeID"),
                            StageMapID = reader.GetInt32("StageMapID"),
                            DocPropertySL = reader.GetString("DocPropertySL"),
                            UDDocPropertyCode = reader.GetString("UDDocPropertyCode"),
                            DocPropertyName = reader.GetString("DocPropertyName"),
                            DocClassification = reader.GetString("DocClassification"),
                            PreservationPolicy = reader.GetString("PreservationPolicy"),
                            PhysicalLocation = reader.GetString("PhysicalLocation"),
                            SerialNo = reader.GetInt32("SerialNo"),
                            Remarks = reader.GetString("Remarks"),
                            SetOn = reader.GetString("SetOn"),
                            SetBy = reader.GetString("SetBy"),
                            ModifiedOn = reader.GetString("ModifiedOn"),
                            ModifiedBy = reader.GetString("ModifiedBy"),
                            Status = reader.GetInt32("Status")
                        }).ToList();
                    }
                }
            }

            return docPropertyList;
        }

        public string AddNewDocument(DSM_DocProperty docProperty, List<WFM_ProcessStageMap> DocumentPermissionModal, string action, out string errorNumber)
        {
            errorNumber = String.Empty;
            DataTable DocumentPermissionTable = new DataTable();


            DocumentPermissionTable = new DataTable();
            DocumentPermissionTable.Columns.Add("ID");
            DocumentPermissionTable.Columns.Add("StageMapID");



            int i = 1;
            foreach (var item in DocumentPermissionModal)
            {
                DataRow objDataRow = DocumentPermissionTable.NewRow();

                objDataRow[0] = i;
                objDataRow[1] = item.StageMapID;


            DocumentPermissionTable.Rows.Add(objDataRow);
                i++;
            }
            
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_SetDocumentsForStage"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@DocumentPermissionTable", SqlDbType.Structured, DocumentPermissionTable);
                    db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, docProperty.DocPropertyID);
                    db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, docProperty.DocCategoryID.Trim());
                    db.AddInParameter(dbCommandWrapper, "@OwnerLevelID", SqlDbType.NVarChar, docProperty.OwnerLevelID.Trim());
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, docProperty.OwnerID.Trim());
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID ", SqlDbType.NVarChar, docProperty.DocTypeID.Trim());
                    db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.Int, docProperty.StageMapID);
                    db.AddInParameter(dbCommandWrapper, "@DocPropertySL", SqlDbType.NVarChar, docProperty.DocPropertySL != null ? docProperty.DocPropertySL.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@UDDocPropertyCode", SqlDbType.NVarChar, docProperty.UDDocPropertyCode != null ? docProperty.UDDocPropertyCode.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@DocPropertyName", SqlDbType.NVarChar, docProperty.DocPropertyName != null ? docProperty.DocPropertyName.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@DocClassification", SqlDbType.NVarChar, docProperty.DocClassification != null ? docProperty.DocClassification.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@PreservationPolicy ", SqlDbType.NVarChar, docProperty.PreservationPolicy);
                    db.AddInParameter(dbCommandWrapper, "@PhysicalLocation", SqlDbType.NVarChar, docProperty.PhysicalLocation != null ? docProperty.PhysicalLocation.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@Remarks", SqlDbType.NVarChar, docProperty.Remarks != null ? docProperty.Remarks.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@SetBy ", SqlDbType.NVarChar, docProperty.SetBy.Trim());
                    db.AddInParameter(dbCommandWrapper, "@ModifiedBy", SqlDbType.NVarChar, docProperty.ModifiedBy.Trim());
                    db.AddInParameter(dbCommandWrapper, "@SerialNo", SqlDbType.Int, docProperty.SerialNo);
                    db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.Int, docProperty.Status);
                    db.AddInParameter(dbCommandWrapper, "@ConfColumnIds", SqlDbType.VarChar, docProperty.ConfigureColumnIds);
                    db.AddInParameter(dbCommandWrapper, "@Action", SqlDbType.VarChar, action);
                    db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);

                    // Execute SP.
                    db.ExecuteNonQuery(dbCommandWrapper);

                    // Getting output parameters and setting response details.
                    if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                    {
                        // Get the error number, if error occurred.
                        errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                    }
                }
            }
            catch (Exception ex)
            {
                errorNumber = "E404"; // Log ex.Message  Insert Log Table               
            }

            return errorNumber;
        }

        public void SaveProcessingStageConnections(List<WFM_ProcessingStageConnection> ProcessingStageConnections,string setBY,out string _res_message)
        {

            DataTable StageConnectionDataTable = new DataTable();
            StageConnectionDataTable.Columns.Add("SourceStageId");
            StageConnectionDataTable.Columns.Add("TargetStageId");
            StageConnectionDataTable.Columns.Add("WorkflowID");
            StageConnectionDataTable.Columns.Add("DocTypeID");
            


            if (ProcessingStageConnections != null)
            {

                foreach (var item in ProcessingStageConnections)
                {
                  
                    DataRow objDataRow = StageConnectionDataTable.NewRow();
                    objDataRow[0] = item.SourceStageId;
                    objDataRow[1] = item.TargetStageId;
                    objDataRow[2] = item.WorkflowID;
                    objDataRow[3] = item.DocTypeID;
                    
                    StageConnectionDataTable.Rows.Add(objDataRow);
                 

                }

            }

            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_SaveProcessingStageConnections"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@ProcessingStageConnections", SqlDbType.Structured, StageConnectionDataTable);
                    db.AddInParameter(dbCommandWrapper, "@SetBy", SqlDbType.NVarChar, setBY);
                    db.AddInParameter(dbCommandWrapper, "@WorkflowID", SqlDbType.Int, ProcessingStageConnections[0].WorkflowID);
                    

                    db.AddOutParameter(dbCommandWrapper, res_message, SqlDbType.VarChar, 100);
                    db.ExecuteNonQuery(dbCommandWrapper);
                    _res_message = db.GetParameterValue(dbCommandWrapper, res_message).ToString();
                }
            }
            catch (Exception ex)
            {
               
                _res_message = "Procedure Exeution Error";
            }


        }
        public void SaveNodeCoordinate(List<WFM_ProcessStage> StagePositionList, string setBY, out string _res_message)
        {

            DataTable StagePositionListDataTable = new DataTable();
            StagePositionListDataTable.Columns.Add("ID");
            StagePositionListDataTable.Columns.Add("StageID");
            StagePositionListDataTable.Columns.Add("PositionX");
            StagePositionListDataTable.Columns.Add("PositionY");


            int i = 1;
            foreach (var item in StagePositionList)
            {

                DataRow objDataRow = StagePositionListDataTable.NewRow();
                objDataRow[0] = i++;
                objDataRow[1] = item.StageID;
                objDataRow[2] = item.PositionX;
                objDataRow[3] = item.PositionY;

                StagePositionListDataTable.Rows.Add(objDataRow);


            }



            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_SaveNodeCoordinate"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@WFM_DefinedProcessingStagePositions", SqlDbType.Structured, StagePositionListDataTable);
                    db.AddInParameter(dbCommandWrapper, "@SetBy", SqlDbType.NVarChar, setBY);
                  
                    db.ExecuteNonQuery(dbCommandWrapper);
                    _res_message ="Success";

                }
            }
            catch (Exception ex)
            {

                _res_message = "Procedure Exeution Error";
            }


        }
        public List<WFM_ProcessingStageConnection> GetProcessStagesConnectionsByWorkflow(int WorkflowID, out string _res_message) {


            List<WFM_ProcessingStageConnection> ProcessingStageConnections = new List<WFM_ProcessingStageConnection>();
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetProcessingStageConnections"))
                {
                    // Set parameters 
              
                    db.AddInParameter(dbCommandWrapper, "@WorkflowID", SqlDbType.Int,WorkflowID);


                    db.AddOutParameter(dbCommandWrapper, res_message, SqlDbType.VarChar, 100);
                    var ds = db.ExecuteDataSet(dbCommandWrapper);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        ProcessingStageConnections = dt1.AsEnumerable().Select(reader => new WFM_ProcessingStageConnection
                        {
                            SourceStageId=reader.GetInt32("SourceStageId"),
                            TargetStageId = reader.GetInt32("TargetStageId")
                            
                           
                        }).ToList();
                    }
                    _res_message = db.GetParameterValue(dbCommandWrapper, res_message).ToString();
                }
            }
            catch (Exception ex)
            {

                _res_message = "Procedure Exeution Error";
            }

            return ProcessingStageConnections;
        }


    }
}
