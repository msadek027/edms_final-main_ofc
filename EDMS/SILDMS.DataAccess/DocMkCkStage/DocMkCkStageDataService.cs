using SILDMS.DataAccessInterface.DocMkCkStage;
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

namespace SILDMS.DataAccess.DocMkCkStage
{
    public class DocMkCkStageDataService : IDocMkCkStageDataService
    {
        private readonly string res_code = "@res_code";
        private readonly string res_message = "@res_message";
        private readonly string spStatusParam = "@p_Status";

        public VM_DocumentsProperty GetALLDocsProp(string _DocCategoryID, string _OwnerID, string _DocTypeID, int _StageMapID)
        {
            VM_DocumentsProperty obj = new VM_DocumentsProperty();

            List<DSM_DocProperty> docs = new List<DSM_DocProperty>();
            List<WFM_DocStageProperty> props = new List<WFM_DocStageProperty>();
            List<VM_ListTypeProperties> lprops = new List<VM_ListTypeProperties>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetALLDocsProp"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, _OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, _DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, _DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.NVarChar, _StageMapID);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    docs = dt1.AsEnumerable().Select(reader => new DSM_DocProperty
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

                if (ds.Tables[1].Rows.Count > 0)
                {
                    DataTable dt2 = ds.Tables[1];
                    props = dt2.AsEnumerable().Select(reader => new WFM_DocStageProperty
                    {
                        DocTypePropertyID = reader.GetString("DocTypePropertyID"),
                        OwnerID = reader.GetString("OwnerID"),
                        DocTypeID = reader.GetString("DocTypeID"),
                        StageMapID = reader.GetInt32("StageMapID"),
                        DocCategoryID = reader.GetString("DocCategoryID"),
                        PropertySL = reader.GetInt32("PropertySL"),
                        PropertyName = reader.GetString("PropertyName"),
                        PropertyType = ConvertToHtmlType(reader.GetString("PropertyType")),
                        IsRequired = reader.GetBoolean("IsRequired")
                    }).ToList();
                }

                if (ds.Tables[2].Rows.Count > 0)
                {
                    DataTable dt3 = ds.Tables[2];

                    var rowData = dt3.AsEnumerable().Select(m => new
                    {
                        PropertyName = m.GetString("PropertyName"),
                        TableRefID = m.GetString("TableRefID"),
                        FieldTitle = m.GetString("ColumnName"),
                        DataType = m.GetString("DataType"),
                        MaxSize = m.GetInt32("MaxLength"),
                        Master = m.GetString("MasterTableName"),
                        RelationID = m.GetInt32("RelationID")
                    }).ToList();

                    var fResult = rowData.GroupBy(a => a.TableRefID).Select(reader => new VM_ListTypeProperties
                    {
                        TableRefID = reader.Key,
                        PropertyName = reader.Select(x => x.PropertyName).FirstOrDefault(),
                        ColumnList = reader.Select(m => new WFM_TableProperty
                        {
                            FieldTitle = m.FieldTitle,
                            DataType = m.DataType,
                            MaxSize = m.MaxSize,
                            Master = m.Master,
                            RelationID = m.RelationID
                        }).ToList()
                    }).ToList();

                    obj.ListTypeProperties = fResult;
                }
            }

            obj.Documents = docs;
            obj.TypeProperties = props;

            return obj;
        }

        public VM_DocumentsPropertyValues GetDocumentPropertyValues(string userID, string _ObjectID, string _StageMapID)
        {
            VM_DocumentsPropertyValues obj = new VM_DocumentsPropertyValues();
            
            List<ObjectProperties> props = new List<ObjectProperties>();
            List<ObjectDocuments> docs = new List<ObjectDocuments>();
            List<PermittedDocument> PermittedDocs = new List<PermittedDocument>();
            List<ParentStage> ParentStages = new List<ParentStage>();

            List<DataTable> listProps = new List<DataTable>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetDocumentPropertyValues"))
            {
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, userID);
                db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, _ObjectID);
                db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.NVarChar, _StageMapID);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    props = dt1.AsEnumerable().Select(reader => new ObjectProperties
                    {
                        ObjectPropertyID = reader.GetString("ObjectPropertyID"),
                        DocTypePropertyID = reader.GetString("DocTypePropertyID"),
                        PropertyName = reader.GetString("PropertyName"),
                        PropertyType = ConvertToHtmlType(reader.GetString("PropertyType")),
                        IsRequired = reader.GetBoolean("IsRequired"),
                        PropertyValue = reader.GetString("PropertyValue"),
                        StageMapID = reader.GetInt32("StageMapID"),
                        TableRefID = reader.GetString("TableRefID")
                    }).ToList();
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    DataTable dt2 = ds.Tables[1];
                    docs = dt2.AsEnumerable().Select(reader => new ObjectDocuments
                    {
                        DocPropertyID = reader.GetString("DocPropertyID"),
                        DocPropertyName = reader.GetString("DocPropertyName"),
                        DocClassification = reader.GetString("DocClassification"),
                        StageMapID = reader.GetInt32("StageMapID"),
                        DocumentID = reader.GetString("DocumentID"),
                        FileOriginalName = reader.GetString("FileOriginalName"),
                        FileServerURL = reader.GetString("FileServerURL"),
                        FtpUserName = reader.GetString("FtpUserName"),
                        FtpPassword = reader.GetString("FtpPassword"),
                        ServerIP = reader.GetString("ServerIP"),
                        ServerPort = reader.GetString("FtpPort"),
                        ServerID = reader.GetString("ServerID")
                    }).ToList();
                }

                if (ds.Tables[2].Rows.Count > 0)
                {
                    DataTable dt3 = ds.Tables[2];
                    PermittedDocs = dt3.AsEnumerable().Select(reader => new PermittedDocument
                    {
                        DocPropertyID = reader.GetString("DocPropertyID")
                       
                    }).ToList();
                }
                if (ds.Tables[3].Rows.Count > 0)
                {
                    DataTable dt4 = ds.Tables[3];
                    ParentStages = dt4.AsEnumerable().Select(reader => new ParentStage
                    {
                        StageMapID = reader.GetInt32("StageMapID"),
                        StageName = reader.GetString("StageName"),
                        StageID = reader.GetInt32("SourceStageId")

                    }).ToList();
                }
                if (ds.Tables[4].Rows.Count > 0)
                {
                    obj.IsBacked = false;
                    obj.BackReason = ds.Tables[4].Rows[0]["Reason"].ToString();
                    if (obj.BackReason.Length > 0) {
                        obj.IsBacked = true;
                    }
                }
                else
                {
                    obj.IsBacked = false;
                }
                if (ds.Tables.Count > 4)
                {
                    for (int i = 4; i < ds.Tables.Count; i++)
                    {
                        listProps.Add(ds.Tables[i]);
                    }
                }
            }

            obj.ObjectProperties = props;
            obj.ObjectDocuments = docs;
            obj.ListProperties = listProps;
            obj.PermittedDocuments = PermittedDocs;
            obj.ParentStages = ParentStages;

            return obj;
        }

        public DataTable GetMkCkDocuments(string _OwnerID, string _DocCategoryID, string _DocTypeID, int _StageMapID, string userID, int page, int itemsPerPage, string sortBy, bool reverse, string search)
        {
            DataTable userList = new DataTable();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetOriginalDocumentsForMkCk"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, _OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, _DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, _DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.Int, _StageMapID);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, userID);
                db.AddInParameter(dbCommandWrapper, "@page", SqlDbType.Int, page);
                db.AddInParameter(dbCommandWrapper, "@itemsPerPage", SqlDbType.Int, itemsPerPage);
                db.AddInParameter(dbCommandWrapper, "@SortColumn", SqlDbType.NVarChar, sortBy);
                db.AddInParameter(dbCommandWrapper, "@SortDirection", SqlDbType.NVarChar, reverse ? "DESC" : "ASC");
                db.AddInParameter(dbCommandWrapper, "@search", SqlDbType.NVarChar, search);

                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables.Count > 0)
                {
                    userList = ds.Tables[0];
                }
            }

            return userList;
        }

        public WFM_ProcessStageMap GetStageAndUserPermission(int stageMapID, string userID)
        {
            WFM_ProcessStageMap stageList = new WFM_ProcessStageMap();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetStagePropertyAndUserPermission"))
            {
                db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.Int, stageMapID);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, userID);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    stageList = dt1.AsEnumerable().Select(reader => new WFM_ProcessStageMap
                    {
                        StageID = reader.GetInt32("StageID"),
                        OwnerLevelID = reader.GetString("OwnerLevelID"),
                        OwnerID = reader.GetString("OwnerID"),
                        DocCategoryID = reader.GetString("DocCategoryID"),
                        DocTypeID = reader.GetString("DocTypeID"),
                        StageName = reader.GetString("StageName"),
                        StagePosition = reader.GetString("StageMapPosition"),
                        HaveMk = reader.GetBoolean("HaveMk"),
                        StageSL=reader.GetInt32("StageSerial"),
                        HaveCk = reader.GetBoolean("HaveCk"),
                        NotifyMk = reader.GetBoolean("MkPermission"),
                        NotifyCk = reader.GetBoolean("CkPermission"),
                        OwnerLevelName = reader.GetString("LevelName"),
                        OwnerName = reader.GetString("OwnerName"),
                        DocCategoryName = reader.GetString("DocCategoryName"),
                        DocTypeName = reader.GetString("DocTypeName")
                    }).ToList().First();
                }
            }

            return stageList;
        }

        public void SetCheckDone(string objectID, int stageMapID, string UserID, out string _res_code, out string _res_message)
        {
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_SetCheckDone"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, objectID);
                    db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.NVarChar, stageMapID);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, UserID);
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

        public void SetMakeDone(string objectID, int stageMapID, string UserID, out string _res_code, out string _res_message)
        {
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_SetMakeDone"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, objectID);
                    db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.NVarChar, stageMapID);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, UserID);
                    db.AddOutParameter(dbCommandWrapper, res_code, SqlDbType.VarChar, 10);
                    db.AddOutParameter(dbCommandWrapper, res_message, SqlDbType.VarChar, 100);
                    db.ExecuteNonQuery(dbCommandWrapper);
                    _res_code = db.GetParameterValue(dbCommandWrapper, res_code).ToString();
                    _res_message = db.GetParameterValue(dbCommandWrapper, res_message).ToString();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Subquery returned more than 1 value")) {
                    _res_code ="1";
                    _res_message = "Successfull";
                }
                else {
                    _res_code = "0";
                    _res_message = "Procedure Exeution Error";
                }
                
            }
        }
        public bool IsClearForMaking(string objectID, int stageMapID)
        {
            bool IsClearForMaking = false;
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_IsClearForMaking"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, objectID);
                    db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.NVarChar, stageMapID);

                    IDataReader dr = db.ExecuteReader(dbCommandWrapper);
                    while (dr.Read())
                    {
                        IsClearForMaking = dr.GetBoolean("IsClearForMaking");
                    }

                    return IsClearForMaking;

                }
            }
            catch (Exception ex)
            {
                return IsClearForMaking;
            }
        }
        public List<DSM_Documents> UpdateDocumentInfo(string objectID, string docs, List<ObjectProperties> props, string userID, string v, out string _errorNumber)
        {
            DataTable docMetaDataTable = new DataTable();
            docMetaDataTable.Columns.Add("ObjectPropertyID");
            docMetaDataTable.Columns.Add("DocTypePropertyID");
            docMetaDataTable.Columns.Add("PropertyValue");

            foreach (var item in props)
            {
                DataRow objDataRow = docMetaDataTable.NewRow();
                objDataRow[0] = item.ObjectPropertyID;
                objDataRow[1] = item.DocTypePropertyID;
                objDataRow[2] = (item.PropertyType == "date") && (!string.IsNullOrEmpty(item.PropertyValue)) ? item.PropertyValue.Substring(0, 10) : item.PropertyValue;

                docMetaDataTable.Rows.Add(objDataRow);
            }

     
          

            DataTable docPropertyIDDataTable = new DataTable();
            docPropertyIDDataTable.Columns.Add("DocPropertyID");

            if (docs.Length > 0)
            {
                string[] docPropIDs = docs.Split(',');

                foreach (var item in docPropIDs)
                {
                    DataRow objDataRow = docPropertyIDDataTable.NewRow();
                    objDataRow[0] = item;
                    docPropertyIDDataTable.Rows.Add(objDataRow);
                }
            }

            List<DSM_Documents> docPropIdentifyList = new List<DSM_Documents>();
            _errorNumber = String.Empty;
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_UpdateDocumentInfo"))
            {
                db.AddInParameter(dbCommandWrapper, "@Documents", SqlDbType.Structured, docPropertyIDDataTable);
                db.AddInParameter(dbCommandWrapper, "@Properties", SqlDbType.Structured, docMetaDataTable);
                db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, objectID);
                db.AddInParameter(dbCommandWrapper, "@UploaderIP", SqlDbType.NVarChar, v);
                db.AddInParameter(dbCommandWrapper, "@SetBy", SqlDbType.NVarChar, userID);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);

                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    _errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        docPropIdentifyList = dt1.AsEnumerable().Select(reader => new DSM_Documents
                        {
                            DocumentID = reader.GetString("DocumentID"),
                            OwnerID = reader.GetString("OwnerID"),
                            DocCategoryID = reader.GetString("DocCategoryID"),
                            DocTypeID = reader.GetString("DocTypeID"),
                            DocPropertyID = reader.GetString("DocPropertyID"),
                            FileServerURL = reader.GetString("FileServerUrl"),
                            ServerIP = reader.GetString("ServerIP"),
                            ServerPort = reader.GetString("ServerPort"),
                            FtpUserName = reader.GetString("FtpUserName"),
                            FtpPassword = reader.GetString("FtpPassword")
                        }).ToList();
                    }
                }
            }

            return docPropIdentifyList;
        }

        public void DeleteDocumentInfo(string objectID, string documentIDs, string action, out string _res_code, out string _res_message)
        {
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_DeleteDocumentInfo"))
                {
                    db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, objectID);
                    db.AddInParameter(dbCommandWrapper, "@Documents", SqlDbType.NVarChar, documentIDs);
                    db.AddInParameter(dbCommandWrapper, "@Action", SqlDbType.NVarChar, action);
                    db.AddOutParameter(dbCommandWrapper, res_code, SqlDbType.VarChar, 10);
                    db.AddOutParameter(dbCommandWrapper, res_message, SqlDbType.VarChar, 100);
                    db.ExecuteNonQuery(dbCommandWrapper);
                    _res_code = db.GetParameterValue(dbCommandWrapper, res_code).ToString();
                    _res_message = db.GetParameterValue(dbCommandWrapper, res_message).ToString();
                }
            }
            catch (Exception)
            {
                _res_code = "0";
                _res_message = "Procedure Exeution Error";
            }
        }

        public void RevertFromMake(string objectID, int stageMapID, string revertReason, string userID, out string _res_code, out string _res_message)
        {
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                //using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_RevertFromMake"))

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_RevertFromCheckSingleStage"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, objectID);
                    db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.NVarChar, stageMapID);
                    db.AddInParameter(dbCommandWrapper, "@RevertReason", SqlDbType.NVarChar, revertReason);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, userID);
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

        public void RevertFromCheck(string objectID, int stageMapID, string revertReason, string userID, out string _res_code, out string _res_message)
        {
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                //using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_RevertFromCheck"))
                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_RevertFromCheckSingleStage"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, objectID);
                    db.AddInParameter(dbCommandWrapper, "@StageMapID", SqlDbType.NVarChar, stageMapID);
                    db.AddInParameter(dbCommandWrapper, "@RevertReason", SqlDbType.NVarChar, revertReason);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, userID);
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

        public VM_ListPropertyCheck GetDocumentListPropertyValuesCheck(string _ObjectID, string _TableRefID)
        {
            VM_ListPropertyCheck obj = new VM_ListPropertyCheck();
            VM_ListTypeProperties prophead = new VM_ListTypeProperties();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetListPropertyForCheck"))
            {
                db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, _ObjectID);
                db.AddInParameter(dbCommandWrapper, "@TableRefID", SqlDbType.NVarChar, _TableRefID);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    prophead.PropertyName = dt1.Rows[0]["PropertyName"].ToString();
                    prophead.TableRefID = dt1.Rows[0]["TableRefID"].ToString();
                    prophead.ColumnList = dt1.AsEnumerable().Select(reader => new WFM_TableProperty
                    {
                        FieldTitle = reader.GetString("ColumnName")
                    }).ToList();

                    obj.ListPropertyHead = prophead;
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    obj.ListPropertyBody = ds.Tables[1];
                }
            }

            return obj;
        }

        public List<MasterData> GetMasterDataBySearch(string masterID, string searchKey)
        {
            List<MasterData> objs = new List<MasterData>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetSqlStringCommand("SELECT ID,Name FROM " + masterID + ""))
            {
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];

                    objs = dt1.AsEnumerable().Select(reader => new MasterData
                    {
                        ID = reader.GetInt32("ID"),
                        Name = reader.GetString("Name")
                    }).ToList();
                }
            }

            return objs;
        }

        public void DeleteListItem(string tableRefID, int id, string userID, out string res_code, out string res_message)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetSqlStringCommand("DELETE FROM " + tableRefID + " where ID=" + id))
            {
                int count = db.ExecuteNonQuery(dbCommandWrapper);

                if (count > 0)
                {
                    res_code = "1";
                    res_message = "Successfully Deleted";
                }
                else
                {
                    res_code = "0";
                    res_message = "Cant be Deleted";
                }
            }
        }

        public VM_ListTypeProperties ToggleAddNewListItem(string tableRefID)
        {
            VM_ListTypeProperties obj = new VM_ListTypeProperties();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetSqlStringCommand(@"SELECT table_name as 'TableRefID',(SELECT PropertyName FROM WFM_DocTypeProperty where TableRefID = table_name) as 'PropertyName',
                                                column_name as 'ColumnName', data_type as 'DataType',character_maximum_length as 'MaxLength',
                                                t2.ID as 'RelationID', t3.MasterTableName
                                                FROM information_schema.columns t1 LEFT JOIN MasterCollumnRelation t2 ON t1.table_name = t2.TableName AND t1.COLUMN_NAME = t2.CollumnName 
                                                LEFT JOIN MasterTableTracker t3 ON t2.MasterTableID = t3.MasterTableID WHERE table_name='" + tableRefID + "'"))
            {
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];

                    obj.TableRefID = dt1.Rows[0][0].ToString();
                    obj.PropertyName = dt1.Rows[0][1].ToString();

                    obj.ColumnList = dt1.AsEnumerable().Select(m => new WFM_TableProperty
                    {
                        FieldTitle = m.GetString("ColumnName"),
                        DataType = m.GetString("DataType"),
                        MaxSize = m.GetInt32("MaxLength"),
                        Master = m.GetString("MasterTableName"),
                        RelationID = m.GetInt32("RelationID")

                    }).ToList();
                }
            }

            return obj;
        }

        public void AddSingleListItem(List<WFM_TableProperty> listItemColumn, string tableRefID, string objectID, out string res_code, out string res_message)
        {
            string colTxt;
            string valTxt;

            colTxt = "ObjectID";
            valTxt = "@ObjectID";

            foreach (var c in listItemColumn)
            {
                if (c.FieldTitle != "ObjectID" && c.FieldTitle != "ID")
                {
                    colTxt += "," + c.FieldTitle;
                    valTxt += ",'" + c.Value + "'";
                }
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetSqlStringCommand("INSERT INTO " + tableRefID + " (" + colTxt + ") VALUES(" + valTxt + "); "))
            {
                db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, objectID);

                int count = db.ExecuteNonQuery(dbCommandWrapper);

                if (count > 0)
                {
                    res_code = "1";
                    res_message = "Successfully Deleted";
                }
                else
                {
                    res_code = "0";
                    res_message = "Cant be Deleted";
                }
            }
        }

        public List<DSM_DocPropIdentify> AddDocumentInfo(DocumentsInfo _modelDocumentsInfo,string _selectedPropID, List<WFM_DocStageProperty> _docMetaValues, string listPropSql, bool _otherUpload, string _extentions, string _action, out string _errorNumber)
        {
            DataTable docMetaDataTable = new DataTable();
            docMetaDataTable.Columns.Add("DocTypePropertyID");
            docMetaDataTable.Columns.Add("PropertyValue");

            foreach (var item in _docMetaValues)
            {
                DataRow objDataRow = docMetaDataTable.NewRow();

                objDataRow[0] = item.DocTypePropertyID;
                objDataRow[1] = item.PropertyValue;

                docMetaDataTable.Rows.Add(objDataRow);
            }

            DataTable docPropertyIDDataTable = new DataTable();
            docPropertyIDDataTable.Columns.Add("DocPropertyID");

            string[] docPropIDs = _selectedPropID.Split(',');
            foreach (var item in docPropIDs)
            {
                DataRow objDataRow = docPropertyIDDataTable.NewRow();
                objDataRow[0] = item;

                docPropertyIDDataTable.Rows.Add(objDataRow);
            }

            List<DSM_DocPropIdentify> docPropIdentifyList = new List<DSM_DocPropIdentify>();
            _errorNumber = String.Empty;
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_SetDocumentsInfo"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerLevelID", SqlDbType.NVarChar, _modelDocumentsInfo.OwnerLevelID);
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, _modelDocumentsInfo.OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID ", SqlDbType.NVarChar, _modelDocumentsInfo.DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, _modelDocumentsInfo.DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@FileOriginalName", SqlDbType.NVarChar, "");
                db.AddInParameter(dbCommandWrapper, "@FileCodeName", SqlDbType.NVarChar, "");
                db.AddInParameter(dbCommandWrapper, "@FileExtension", SqlDbType.NVarChar, _extentions);
                db.AddInParameter(dbCommandWrapper, "@IsOtherUploder", SqlDbType.NVarChar, _otherUpload ? 1 : 0);
                db.AddInParameter(dbCommandWrapper, "@UploaderIP", SqlDbType.NVarChar, _modelDocumentsInfo.UploaderIP);
                db.AddInParameter(dbCommandWrapper, "@SetBy ", SqlDbType.NVarChar, _modelDocumentsInfo.SetBy);
                db.AddInParameter(dbCommandWrapper, "@ModifiedBy", SqlDbType.NVarChar, _modelDocumentsInfo.ModifiedBy);
                db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.Int, 1);

                db.AddInParameter(dbCommandWrapper, "@Properties", SqlDbType.Structured, docMetaDataTable);
                db.AddInParameter(dbCommandWrapper, "@Documents", SqlDbType.Structured, docPropertyIDDataTable);
                db.AddInParameter(dbCommandWrapper, "@ListPropertySql", SqlDbType.NVarChar, listPropSql);

                db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);

                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    _errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        docPropIdentifyList = dt1.AsEnumerable().Select(reader => new DSM_DocPropIdentify
                        {
                            ObjectID = reader.GetString("ObjectID"),
                            DocumentID = reader.GetString("DocumentID"),
                            OwnerID = reader.GetString("OwnerID"),
                            DocCategoryID = reader.GetString("DocCategoryID"),
                            DocTypeID = reader.GetString("DocTypeID"),
                            DocPropertyID = reader.GetString("DocPropertyID"),
                            FileCodeName = reader.GetString("FileCodeName"),
                            FileServerUrl = reader.GetString("FileServerUrl"),
                            ServerIP = reader.GetString("ServerIP"),
                            ServerPort = reader.GetString("ServerPort"),
                            FtpUserName = reader.GetString("FtpUserName"),
                            FtpPassword = reader.GetString("FtpPassword")
                        }).ToList();
                    }
                }
            }

            return docPropIdentifyList;
        }

        public string DeleteDocumentInfo(string _DocumentIDs, out string _errorNumber)
        {
            _errorNumber = String.Empty;
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_DeleteDocumentInfo"))
            {
                db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, _DocumentIDs);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);

                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    _errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
            }

            return _errorNumber;
        }

        public string ConvertToHtmlType(string oldType)
        {
            string outPutType = "text";

            switch (oldType)
            {
                case "String":
                    outPutType = "text";
                    break;
                case "Date":
                    outPutType = "date";
                    break;
                default:
                    outPutType = "text";
                    break;
            }

            return outPutType;
        }

    }
}
