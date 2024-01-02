using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.OriginalDocSearching;
using SILDMS.Model.DocScanningModule;
using SILDMS.Web.UI.Models;

namespace SILDMS.DataAccess.OriginalDocSearching
{
    public class OriginalDocSearchingDataService : IOriginalDocSearchingDataService
    {
        private readonly string spStatusParam = "@p_Status";
        public string UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo, string userId, out string errorNumber)
        {
            errorNumber = String.Empty;
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                DataTable docMetaDataTable = new DataTable();
                //docMetaDataTable.Columns.Add("ID");
                docMetaDataTable.Columns.Add("DocMetaID");
                docMetaDataTable.Columns.Add("MetaValue");

                foreach (var item in _modelDocumentsInfo.DocMetaValues)
                {
                    DataRow objDataRow = docMetaDataTable.NewRow();
                    //objDataRow[0] = 0;
                    objDataRow[0] = item.DocMetaID;
                    objDataRow[1] = item.MetaValue;

                    docMetaDataTable.Rows.Add(objDataRow);
                }

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("UpdateDocMetaInfo"))
                {
                    db.AddInParameter(dbCommandWrapper, "@Doc_Meta", SqlDbType.Structured, docMetaDataTable);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, userId);

                    //db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.NVarChar, _modelDocumentsInfo.Status);
                    //db.AddInParameter(dbCommandWrapper, "@DocumentID", SqlDbType.NVarChar, _modelDocumentsInfo.DocumentID);

                    db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);
                    db.ExecuteNonQuery(dbCommandWrapper);

                    if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                    {
                        errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                    }

                }
            }
            catch (Exception ex)
            {
                errorNumber = "E404";
            }

            return errorNumber;
        }

        public string DeleteDocument(string _DocumentID, string _DocDistributionID, string _DocumentType, string userId, out string errorNumber)
        {
            errorNumber = String.Empty;

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("DeleteDocument"))
            {
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, userId);
                db.AddInParameter(dbCommandWrapper, "@DocumentID", SqlDbType.NVarChar, _DocumentID);
                db.AddInParameter(dbCommandWrapper, "@DocDistributionID", SqlDbType.NVarChar, _DocDistributionID);
                db.AddInParameter(dbCommandWrapper, "@DocumentType", SqlDbType.NVarChar, _DocumentType);
                db.ExecuteNonQuery(dbCommandWrapper);
            }

            return errorNumber;
        }

        public List<DocSearch> GetOriginalDocBySearchParam(string _OwnerID, string _DocCategoryID,
            string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<DocSearch> userList = new List<DocSearch>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetOriginalDocBySearchParam"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, _OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, _DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, _DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, _DocPropertyID);
                db.AddInParameter(dbCommandWrapper, "@SearchBy", SqlDbType.NVarChar, _SearchBy);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, _UserID);
                db.AddInParameter(dbCommandWrapper, "@page", SqlDbType.Int, page);
                db.AddInParameter(dbCommandWrapper, "@itemsPerPage", SqlDbType.Int, itemsPerPage);
                db.AddInParameter(dbCommandWrapper, "@sortBy", SqlDbType.NVarChar, sortBy);
                db.AddInParameter(dbCommandWrapper, "@reverse", SqlDbType.Int, reverse);
                db.AddInParameter(dbCommandWrapper, "@attribute", SqlDbType.NVarChar, attribute);
                db.AddInParameter(dbCommandWrapper, "@search", SqlDbType.NVarChar, search);
                //db.AddOutParameter(dbCommandWrapper, "@p_Status", DbType.Int32, 10);
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
                        userList = dt1.AsEnumerable().Select(reader => new DocSearch
                        {
                            DocumentID = reader.GetString(0),
                            FileCodeName = reader.GetString(1),
                            DocPropIdentifyID = reader.GetString(2),
                            DocPropIdentifyName = reader.GetString(3),
                            MetaValue = reader.GetString(4),
                            DocPropertyName = reader.GetString(5),

                            FileServerURL = reader.GetString(6),
                            ServerIP = reader.GetString(7),
                            ServerPort = reader.GetString(8),
                            FtpUserName = reader.GetString(9),
                            FtpPassword = reader.GetString(10),

                            VersionNo = reader.GetString(11),
                            DocDistributionID = reader.GetString(12),

                            OwnerID = reader.GetString(13),
                            DocCategoryID = reader.GetString(14),
                            DocTypeID = reader.GetString(15),
                            DocPropertyID = reader.GetString(16),

                            Status = reader.GetString(18),
                            TotalCount = reader.GetInt32(19),
                            OwnerLevelID = reader.GetString(20),
                            FileExtenstion = reader.GetString(21),
                            IsSecured = Convert.ToBoolean(reader.GetInt32("IsSecured")),
                            IsObsolutable = Convert.ToBoolean(reader.GetInt32("IsObsolete")),

                            ServerID = reader.GetString("ServerID")

                        }).ToList();
                    }
                }
            }

            return userList;
        }

        public List<DocSearch> GetOriginalDocBySearchParamV2(string _OwnerID, string _DocCategoryID,
            string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, string searchType, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<DocSearch> userList = new List<DocSearch>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetOriginalDocBySearchParamV2"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, _OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, _DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, _DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, _DocPropertyID);
                db.AddInParameter(dbCommandWrapper, "@SearchBy", SqlDbType.NVarChar, _SearchBy);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, _UserID);
                db.AddInParameter(dbCommandWrapper, "@page", SqlDbType.Int, page);
                db.AddInParameter(dbCommandWrapper, "@itemsPerPage", SqlDbType.Int, itemsPerPage);
                db.AddInParameter(dbCommandWrapper, "@sortBy", SqlDbType.NVarChar, sortBy);
                db.AddInParameter(dbCommandWrapper, "@reverse", SqlDbType.Int, reverse);
                db.AddInParameter(dbCommandWrapper, "@attribute", SqlDbType.NVarChar, attribute);
                db.AddInParameter(dbCommandWrapper, "@search", SqlDbType.NVarChar, search);
                db.AddInParameter(dbCommandWrapper, "@searchType", SqlDbType.NVarChar, searchType);
                db.AddOutParameter(dbCommandWrapper, "@p_Status", DbType.Int32, 10);

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
                        userList = dt1.AsEnumerable().Select(reader => new DocSearch
                        {
                            DocDistributionID = reader.GetString(0),
                            DocCategoryID = reader.GetString(1),
                            DocTypeID = reader.GetString(2),
                            DocumentID = reader.GetString(3),
                            DocPropertyID = reader.GetString(4),
                            DocPropIdentifyID = reader.GetString(5),
                            DocPropertyName = reader.GetString(7),
                            DocPropIdentifyName = reader.GetString(8),
                            MetaValue = reader.GetString(9),
                            OwnerID = reader.GetString(10),
                            OwnerLevelID = reader.GetString(11),
                            FileExtenstion = reader.GetString(12),
                            FileCodeName = reader.GetString(13),
                            FileServerURL = reader.GetString(14),
                            ServerIP = reader.GetString(15),
                            ServerPort = reader.GetString(16),
                            FtpUserName = reader.GetString(17),
                            FtpPassword = reader.GetString(18),
                            VersionNo = reader.GetString(19),
                            InfoCopy = reader.GetString(21),
                            AddText = reader.GetString(22),
                            Status = reader.GetString(23),
                            IsSecured = Convert.ToBoolean(reader.GetInt32("IsSecured")),
                            IsObsolutable = Convert.ToBoolean(reader.GetInt32("IsObsolete")),
                            TotalCount = reader.GetInt32("Count")
                        }).ToList();
                    }
                }
            }

            return userList;
        }

        public List<DocSearch> GetOriginalDocumentsByWildSearch(string _SearchFor, string _UserID, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<DocSearch> userList = new List<DocSearch>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetOriginalDocumentsByWildSearch"))
            {
                db.AddInParameter(dbCommandWrapper, "@SearchFor", SqlDbType.NVarChar, _SearchFor);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, _UserID);
                db.AddOutParameter(dbCommandWrapper, "@p_Status", DbType.Int32, 10);

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
                        userList = dt1.AsEnumerable().Select(reader => new DocSearch
                        {
                            //DocMetaIDVersion = reader.GetString("DocMetaIDVersion"),
                            //DocVersionID = reader.GetString("DocVersionID"),
                            DocMetaID = reader.GetString("DocMetaID"),
                            FileCodeName = reader.GetString("FileCodeName"),
                            DocumentID = reader.GetString("DocumentID"),
                            //ReferenceVersionID = reader.GetString("ReferenceVersionID"),

                            DocPropIdentifyID = reader.GetString("DocPropIdentifyID"),
                            DocPropIdentifyName = reader.GetString("DocPropIdentifyName"),
                            MetaValue = reader.GetString("MetaValue"),
                            //OriginalReference = reader.GetString("OriginalReference"),
                            //DocPropertyID = reader.GetString("DocPropertyID"),
                            //DocPropertyName = reader.GetString("DocPropertyName"),
                            FileServerURL = reader.GetString("FileServerURL"),
                            ServerIP = reader.GetString("ServerIP"),
                            ServerPort = reader.GetString("ServerPort"),
                            FtpUserName = reader.GetString("FtpUserName"),
                            FtpPassword = reader.GetString("FtpPassword"),
                            VersionNo = reader.GetString("VersionNo"),
                            DocDistributionID = reader.GetString("DocDistributionID"),
                            OwnerLevelID = reader.GetString("OwnerLevelID"),
                            OwnerID = reader.GetString("OwnerID"),
                            DocCategoryID = reader.GetString("DocCategoryID"),
                            DocTypeID = reader.GetString("DocTypeID"),
                            DocPropertyID = reader.GetString("DocPropertyID"),
                            DocPropertyName = reader.GetString("DocPropertyName")
                        }).ToList();
                    }
                }
            }

            return userList;
        }

        public List<OriginalDocMeta> GetOriginalDocMeta(string _DocumentId, string _DocDistributionID, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<OriginalDocMeta> docList = new List<OriginalDocMeta>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetOriginalDocMeta"))
            {
                db.AddInParameter(dbCommandWrapper, "@DocumentId", SqlDbType.NVarChar, _DocumentId);
                db.AddInParameter(dbCommandWrapper, "@DocDistributionID", SqlDbType.NVarChar, _DocDistributionID);
                db.AddOutParameter(dbCommandWrapper, "@p_Status", DbType.Int32, 10);

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
                        docList = dt1.AsEnumerable().Select(reader => new OriginalDocMeta
                        {
                            DocID = reader.GetString("DocumentID"),
                            DocMetaID = reader.GetString("DocMetaID"),
                            DocPropIdentifyName = reader.GetString("IdentificationAttribute"),
                            MetaValue = reader.GetString("MetaValue"),
                            Remarks = reader.GetString("Remarks")
                        }).ToList();
                    }
                }
            }

            return docList;
        }


        public List<DocSearch> GetDocumentsBySearchParamForVersion(string _OwnerID, string _DocCategoryID,
             string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<DocSearch> userList = new List<DocSearch>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetOriginalDocBySearchParamForVersion"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, _OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, _DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, _DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, _DocPropertyID);
                db.AddInParameter(dbCommandWrapper, "@SearchBy", SqlDbType.NVarChar, _SearchBy);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, _UserID);
                db.AddInParameter(dbCommandWrapper, "@page", SqlDbType.Int, page);
                db.AddInParameter(dbCommandWrapper, "@itemsPerPage", SqlDbType.Int, itemsPerPage);
                db.AddInParameter(dbCommandWrapper, "@sortBy", SqlDbType.NVarChar, sortBy);
                db.AddInParameter(dbCommandWrapper, "@reverse", SqlDbType.Int, reverse);
                db.AddInParameter(dbCommandWrapper, "@attribute", SqlDbType.NVarChar, attribute);
                db.AddInParameter(dbCommandWrapper, "@search", SqlDbType.NVarChar, search);
                //db.AddOutParameter(dbCommandWrapper, "@p_Status", DbType.Int32, 10);
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
                        userList = dt1.AsEnumerable().Select(reader => new DocSearch
                        {
                            DocumentID = reader.GetString(0),
                            FileCodeName = reader.GetString(1),
                            DocPropIdentifyID = reader.GetString(2),
                            DocPropIdentifyName = reader.GetString(3),
                            MetaValue = reader.GetString(4),
                            DocPropertyName = reader.GetString(5),

                            FileServerURL = reader.GetString(6),
                            ServerIP = reader.GetString(7),
                            ServerPort = reader.GetString(8),
                            FtpUserName = reader.GetString(9),
                            FtpPassword = reader.GetString(10),

                            VersionNo = reader.GetString(11),
                            DocDistributionID = reader.GetString(12),

                            OwnerID = reader.GetString(13),
                            DocCategoryID = reader.GetString(14),
                            DocTypeID = reader.GetString(15),
                            DocPropertyID = reader.GetString(16),

                            Status = reader.GetString(18),
                            TotalCount = reader.GetInt32(19),
                            OwnerLevelID = reader.GetString(20),
                            FileExtenstion = reader.GetString(21),
                            IsSecured = Convert.ToBoolean(reader.GetInt32("IsSecured")),
                            IsObsolutable = Convert.ToBoolean(reader.GetInt32("IsObsolete"))
                        }).ToList();
                    }
                }
            }

            return userList;
        }



        public List<DocSearch> GetOriginalDocBySearchFromList(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string value, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<DocSearch> userList = new List<DocSearch>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetOriginalDocBySearchFromList"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, _OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, _DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, _DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, _DocPropertyID);
                db.AddInParameter(dbCommandWrapper, "@value", SqlDbType.NVarChar, value);
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
                        userList = dt1.AsEnumerable().Select(reader => new DocSearch
                        {
                            DocumentID = reader.GetString(0),
                            FileCodeName = reader.GetString(1),
                            DocPropIdentifyID = reader.GetString(2),
                            DocPropIdentifyName = reader.GetString(3),
                            MetaValue = reader.GetString(4),
                            DocPropertyName = reader.GetString(5),

                            FileServerURL = reader.GetString(6),
                            ServerIP = reader.GetString(7),
                            ServerPort = reader.GetString(8),
                            FtpUserName = reader.GetString(9),
                            FtpPassword = reader.GetString(10),

                            VersionNo = reader.GetString(11),
                            DocDistributionID = reader.GetString(12),

                            OwnerID = reader.GetString(13),
                            DocCategoryID = reader.GetString(14),
                            DocTypeID = reader.GetString(15),
                            DocPropertyID = reader.GetString(16),

                            Status = reader.GetString(18),
                            TotalCount = reader.GetInt32(19),
                            OwnerLevelID = reader.GetString(20),
                            FileExtenstion = reader.GetString(21),
                            IsSecured = Convert.ToBoolean(reader.GetInt32("IsSecured")),
                            IsObsolutable = Convert.ToBoolean(reader.GetInt32("IsObsolete"))
                        }).ToList();
                    }
                }
            }

            return userList;
        }


        public List<DocSearch> GetOriginalDocBySearchForPrint(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string Docs, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<DocSearch> userList = new List<DocSearch>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetOriginalDocBySearchForPrint"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, _OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, _DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, _DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, _DocPropertyID);
                db.AddInParameter(dbCommandWrapper, "@value", SqlDbType.NVarChar, Docs);
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
                        userList = dt1.AsEnumerable().Select(reader => new DocSearch
                        {
                            DocumentID = reader.GetString(0),
                            FileCodeName = reader.GetString(1),
                            DocPropIdentifyID = reader.GetString(2),
                            DocPropIdentifyName = reader.GetString(3),
                            MetaValue = reader.GetString(4),
                            DocPropertyName = reader.GetString(5),

                            FileServerURL = reader.GetString(6),
                            ServerIP = reader.GetString(7),
                            ServerPort = reader.GetString(8),
                            FtpUserName = reader.GetString(9),
                            FtpPassword = reader.GetString(10),

                            VersionNo = reader.GetString(11),
                            DocDistributionID = reader.GetString(12),

                            OwnerID = reader.GetString(13),
                            DocCategoryID = reader.GetString(14),
                            DocTypeID = reader.GetString(15),
                            DocPropertyID = reader.GetString(16),

                            Status = reader.GetString(18),
                            TotalCount = reader.GetInt32(19),
                            OwnerLevelID = reader.GetString(20),
                            FileExtenstion = reader.GetString(21),
                            IsSecured = Convert.ToBoolean(reader.GetInt32("IsSecured")),
                            IsObsolutable = Convert.ToBoolean(reader.GetInt32("IsObsolete"))
                        }).ToList();
                    }
                }
            }

            return userList;
        }

        public void UpdateDocMailNotifyAndExpDate(string UserID, string DocID, string NotifyDate, string ExpDate, out string outStatus)
        {

            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;
                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("UpdateNotifyDateandExpDateForDocument"))
                {
                    db.AddInParameter(dbCommandWrapper, "@DocID", SqlDbType.NVarChar, DocID);
                    db.AddInParameter(dbCommandWrapper, "@NotifyDate", SqlDbType.NVarChar, NotifyDate);
                    db.AddInParameter(dbCommandWrapper, "@ExpDate", SqlDbType.NVarChar, ExpDate);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, UserID);

                    db.ExecuteNonQuery(dbCommandWrapper);
                    outStatus = "success";


                }
            }
            catch (Exception ex)
            {
                outStatus = "Failed";
            }
        }
        public DSM_VM_Property GetMailNotifyAndExpDate(string DocumentId)
        {
            DSM_VM_Property NotifyAndExpDate = new DSM_VM_Property();
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;
                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetNotifyAndExpDate"))
                {
                    db.AddInParameter(dbCommandWrapper, "@DocumentId", SqlDbType.NVarChar, DocumentId);


                    IDataReader dr = db.ExecuteReader(dbCommandWrapper);
                    while (dr.Read())
                    {
                        NotifyAndExpDate.NotifyDate = dr.GetDateTime("NotificationDate");
                        NotifyAndExpDate.ExpDate = dr.GetDateTime("DocumentExpDate");
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return NotifyAndExpDate;
        }
    }
}

