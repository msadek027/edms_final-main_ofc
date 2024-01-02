using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.WorkflowDocSearching;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace SILDMS.DataAccess.WorkflowDocSearching
{
    public class WorkflowDocSearchingDataService : IWorkflowDocSearchingDataService
    {
        private readonly string spStatusParam = "@p_Status";
        public DataTable GetWorkflowDocument(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string search, out string errorNumber)
        {
            errorNumber = string.Empty;
            DataTable userList = new DataTable();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetWorkflowDocument"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, _OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, _DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, _DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, _UserID);
                db.AddInParameter(dbCommandWrapper, "@page", SqlDbType.Int, page);
                db.AddInParameter(dbCommandWrapper, "@itemsPerPage", SqlDbType.Int, itemsPerPage);
                db.AddInParameter(dbCommandWrapper, "@SortColumn", SqlDbType.NVarChar, sortBy);
                db.AddInParameter(dbCommandWrapper, "@SortDirection", SqlDbType.NVarChar, reverse ? "DESC" : "ASC");
                db.AddInParameter(dbCommandWrapper, "@search", SqlDbType.NVarChar, search);
                db.AddOutParameter(dbCommandWrapper, "@p_Status", DbType.Int32, 10);

                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables.Count > 0)
                    {
                        userList = ds.Tables[0];
                    }
                }
            }

            return userList;
        }

        public List<DSM_Documents> GetDocumentForSpecificObject(string _ObjectID, string _UserID)
        {
            List<DSM_Documents> DocumensList = new List<DSM_Documents>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetSqlStringCommand("SELECT * FROM DSM_Documents d LEFT JOIN SEC_Server s ON d.ServerID=s.ServerID LEFT JOIN DSM_DocProperty p ON d.DocPropertyID=p.DocPropertyID WHERE d.ObjectID = '" + _ObjectID + "' ORDER BY p.DocPropertySL ASC"))
            {
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    DocumensList = dt1.AsEnumerable().Select(reader => new DSM_Documents
                    {
                        DocumentID = reader.GetString("DocumentID"),
                        FileServerURL = reader.GetString("FileServerURL"),
                        DocPropertyName = reader.GetString("DocPropertyName"),
                        ServerIP = reader.GetString("ServerIP"),
                        ServerPort = reader.GetString("FtpPort"),
                        FtpUserName = reader.GetString("FtpUserName"),
                        FtpPassword = reader.GetString("FtpPassword")
                    }).ToList();
                }
            }

            return DocumensList;
        }

        public VM_DocumentsPropertyValuesAll GetDocumentPropertyValues(string userID, string _ObjectID)
        {
            VM_DocumentsPropertyValuesAll obj = new VM_DocumentsPropertyValuesAll();

            List<ObjectProperties> props = new List<ObjectProperties>();
            List<ObjectDocuments> docs = new List<ObjectDocuments>();
            List<StageChangeHistory> history = new List<StageChangeHistory>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetDocumentPropertyValuesAll"))
            {
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, userID);
                db.AddInParameter(dbCommandWrapper, "@ObjectID", SqlDbType.NVarChar, _ObjectID);

                var ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt0 = ds.Tables[0];

                    obj.LevelName = dt0.Rows[0].GetString("LevelName");
                    obj.OwnerName = dt0.Rows[0].GetString("OwnerName");
                    obj.DocCategoryName = dt0.Rows[0].GetString("DocCategoryName");
                    obj.DocTypeName = dt0.Rows[0].GetString("DocTypeName");
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[1];
                    props = dt1.AsEnumerable().Select(reader => new ObjectProperties
                    {
                        ObjectPropertyID = reader.GetString("ObjectPropertyID"),
                        DocTypePropertyID = reader.GetString("DocTypePropertyID"),
                        PropertyName = reader.GetString("PropertyName"),
                        PropertyType = reader.GetString("PropertyType"),
                        IsRequired = reader.GetBoolean("IsRequired"),
                        PropertyValue = reader.GetString("PropertyValue"),
                        StageMapID = reader.GetInt32("StageMapID")
                    }).ToList();
                }

                if (ds.Tables[2].Rows.Count > 0)
                {
                    DataTable dt2 = ds.Tables[2];
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
                    }).Where(a => a.DocumentID != null && a.DocumentID != "").ToList();
                }

                if (ds.Tables[3].Rows.Count > 0)
                {
                    DataTable dt3 = ds.Tables[3];
                    history = dt3.AsEnumerable().Select(reader => new StageChangeHistory
                    {
                        FromStage = reader.GetString("FromStage"),
                        ToStage = reader.GetString("ToStage"),
                        FromStageName = reader.GetString("FromStageName"),
                        ToStageName = reader.GetString("ToStageName"),
                        IsMakeOrCheck = reader.GetBoolean("IsMakeOrCheck"),
                        TypeOfChange = reader.GetInt32("TypeOfChange"),
                        Reason = reader.GetString("Reason"),
                        SetBy = reader.GetString("SetBy"),
                        SetOn = reader.GetString("SetOn")
                    }).ToList();
                }
            }

            obj.ObjectProperties = props;
            obj.ObjectDocuments = docs;
            obj.StageChangeHistory = history;

            return obj;
        }

    }
}
