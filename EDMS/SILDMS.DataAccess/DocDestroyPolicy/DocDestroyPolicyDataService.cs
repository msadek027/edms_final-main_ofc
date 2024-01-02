
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.SecurityModule;

namespace SILDMS.DataAccess
{
    public class DocDestroyPolicyDataService : IDocDestroyPolicyDataService
    {
        private readonly string spStatusParam = "@p_Status";
        public List<DSM_DestroyPolicy> GetDestroyPolicyBySearchParam(string _DestroyPolicyID, 
            string _UserID, string _OwnerID,
            string _DocCategoryID, string _DocTypeID,
            string _DocPropertyID, string _DocPropIdentityID, out string _errorNumber)
        {
            _errorNumber = string.Empty;
            List<DSM_DestroyPolicy> destroyPolicies = new List<DSM_DestroyPolicy>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetDestroyPolicyBySearchParam"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, _UserID);
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.VarChar, _OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategory", SqlDbType.VarChar, _DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocType", SqlDbType.VarChar, _DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@DocProperty", SqlDbType.VarChar, _DocPropertyID);
                db.AddInParameter(dbCommandWrapper, "@DocPropIdentity", SqlDbType.VarChar, _DocPropIdentityID);
                
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                // Execute SP.
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    _errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        destroyPolicies = dt1.AsEnumerable().Select(reader => new DSM_DestroyPolicy
                        {
                            DestroyPolicyID = reader.GetString("DestroyPolicyID"),
                            DestroyPolicyDtlID = reader.GetString("DestroyPolicyDtlID"),
                            PolicyFor = reader.GetString("PolicyFor"),
                            PolicyApplicableTo = reader.GetString("PolicyApplicableTo"),
                            DocumentNature = reader.GetString("DocumentNature"),
                            PolicyStatus = reader.GetInt32("PolicyStatus"),
                            Status = reader.GetInt32("PolicyStatus"),
                            OwnerID = reader.GetString("OwnerID"),
                            DocCategoryID = reader.GetString("DocCategoryID"),
                            DocTypeID = reader.GetString("DocTypeID"),
                            DocPropertyID = reader.GetString("DocPropertyID"),
                            DocPropIdentifyID = reader.GetString("DocPropIdentifyID"),
                       
                        }).ToList();
                    }
                }
            }
            return destroyPolicies;
        }

        public List<DSM_DestroyPolicy> GetDestroyPolicyDtlByID(string _DestroyPolicyID, out string _errorNumber)
        {
            _errorNumber = string.Empty;
            List<DSM_DestroyPolicy> destroyPolicies = new List<DSM_DestroyPolicy>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetDestroyPolicyDtlByID"))
            {
                db.AddInParameter(dbCommandWrapper, "@DestroyPolicyID", SqlDbType.VarChar, _DestroyPolicyID);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    _errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                { 
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        destroyPolicies = dt1.AsEnumerable().Select(reader => new DSM_DestroyPolicy
                        {
                            DestroyPolicyID = reader.GetString("DestroyPolicyID"),
                            DestroyPolicyDtlID = reader.GetString("DestroyPolicyDtlID"),                       
                            OwnerID=reader.GetString("OwnerID"),
                            DocCategoryID=reader.GetString("DocCategoryID"),
                            DocTypeID=reader.GetString("DocTypeID"),
                            DocPropertyID=reader.GetString("DocPropertyID"),
                            DocPropIdentifyID=reader.GetString("DocPropIdentifyID"),
                            MetaValue=reader.GetString("MetaValue"),
                            TimeValue=reader.GetString("TimeValue"),
                            TimeValueCon=reader.GetString("TimeValueCon"),
                            AutoDelete=reader.GetBoolean("AutoDelete"),
                            TimeUnit=reader.GetString("TimeUnit"),
                            ExceptionValue = reader.GetString("ExceptionValue"),
                            IsSelected=true
                        }).ToList();
                    }
                }
                
            }
            return destroyPolicies;
        }


        public string SetDocDestroyPolicy(DSM_DestroyPolicy model, string action, out string errorNumber)
        {
            errorNumber = String.Empty;
            try
            {
                //IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                DataTable docCategoryDataTable = new DataTable();
                docCategoryDataTable.Columns.Add("DestroyPolicyDtlID");
                docCategoryDataTable.Columns.Add("DocCategoryID");
                docCategoryDataTable.Columns.Add("TimeValue");
                docCategoryDataTable.Columns.Add("TimeValueCon");
                docCategoryDataTable.Columns.Add("AutoDelete");
                docCategoryDataTable.Columns.Add("TimeUnit");
                docCategoryDataTable.Columns.Add("ExceptionValue");

                DataTable docTypeDataTable = new DataTable();
                docTypeDataTable.Columns.Add("DestroyPolicyDtlID");
                docTypeDataTable.Columns.Add("DocTypeID");
                docTypeDataTable.Columns.Add("TimeValue");
                docTypeDataTable.Columns.Add("TimeValueCon");
                docTypeDataTable.Columns.Add("AutoDelete");
                docTypeDataTable.Columns.Add("TimeUnit");
                docTypeDataTable.Columns.Add("ExceptionValue");

                DataTable docPropertyDataTable = new DataTable();
                docPropertyDataTable.Columns.Add("DestroyPolicyDtlID");
                docPropertyDataTable.Columns.Add("DocPropertyID");
                docPropertyDataTable.Columns.Add("TimeValue");
                docPropertyDataTable.Columns.Add("TimeValueCon");
                docPropertyDataTable.Columns.Add("AutoDelete");
                docPropertyDataTable.Columns.Add("TimeUnit");
                docPropertyDataTable.Columns.Add("ExceptionValue");

                DataTable docPropIdentifyDataTable = new DataTable();
                docPropIdentifyDataTable.Columns.Add("DestroyPolicyDtlID");
                docPropIdentifyDataTable.Columns.Add("DocPropIdentifyID");
                docPropIdentifyDataTable.Columns.Add("PropIdentityMetaValue");
                docPropIdentifyDataTable.Columns.Add("TimeValue");
                docPropIdentifyDataTable.Columns.Add("TimeValueCon");
                docPropIdentifyDataTable.Columns.Add("AutoDelete");
                docPropIdentifyDataTable.Columns.Add("TimeUnit");
                docPropIdentifyDataTable.Columns.Add("ExceptionValue");

                if (model.DocCategoryModel != null)
                {
                    foreach (var item in model.DocCategoryModel)
                    {
                        DataRow objDataRow = docCategoryDataTable.NewRow();
                        
                        objDataRow[0] = item.DestroyPolicyDtlID;
                        objDataRow[1] = item.CategoryID;
                        objDataRow[2] = item.TimeValue;
                        objDataRow[3] = item.TimeValueCon;
                        objDataRow[4] = item.AutoDelete;
                        objDataRow[5] = item.TimeUnit;
                        objDataRow[6] = item.ExceptionValue;
                        
                        docCategoryDataTable.Rows.Add(objDataRow);
                    }
                }
                else if (model.DocTypeModel != null)
                {
                    foreach (var item in model.DocTypeModel)
                    {
                        DataRow objDataRow = docTypeDataTable.NewRow();
                        objDataRow[0] = item.DestroyPolicyDtlID;
                        objDataRow[1] = item.TypeID;
                        objDataRow[2] = item.TimeValue;
                        objDataRow[3] = item.TimeValueCon;
                        objDataRow[4] = item.AutoDelete;
                        objDataRow[5] = item.TimeUnit;
                        objDataRow[6] = item.ExceptionValue;
                        
                        docTypeDataTable.Rows.Add(objDataRow);
                    }
                }
                else if (model.DocPropertyModel != null)
                {
                    
                    foreach (var item in model.DocPropertyModel)
                    {
                        DataRow objDataRow = docPropertyDataTable.NewRow();
                        objDataRow[0] = item.DestroyPolicyDtlID;
                        objDataRow[1] = item.PropertyID;
                        objDataRow[2] = item.TimeValue;
                        objDataRow[3] = item.TimeValueCon;
                        objDataRow[4] = item.AutoDelete;
                        objDataRow[5] = item.TimeUnit;
                        objDataRow[6] = item.ExceptionValue;
                        
                        docPropertyDataTable.Rows.Add(objDataRow);
                    }
                }
                else if (model.DocPropIdentityModel != null)
                {
                    foreach (var item in model.DocPropIdentityModel)
                    {
                        DataRow objDataRow = docPropIdentifyDataTable.NewRow();
                        objDataRow[0] = item.DestroyPolicyDtlID;
                        objDataRow[1] = item.PropIdentityID;
                        objDataRow[2] = item.PropIdentityMetaValue;
                        objDataRow[3] = item.TimeValue;
                        objDataRow[4] = item.TimeValueCon;
                        objDataRow[5] = item.AutoDelete;
                        objDataRow[6] = item.TimeUnit;
                        objDataRow[7] = item.ExceptionValue;
                        
                        docPropIdentifyDataTable.Rows.Add(objDataRow);
                    }
                }


                if (model.PolicyApplicableTo == "Category")
                {
                    using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("SetDocCategoryToDocDestroyPolicy"))
                    {
                        db.AddInParameter(dbCommandWrapper, "@DestroyPolicyID", SqlDbType.NVarChar, model.DestroyPolicyID ?? "");
                        db.AddInParameter(dbCommandWrapper, "@PolicyFor", SqlDbType.NVarChar, model.PolicyFor);
                        db.AddInParameter(dbCommandWrapper, "@DocumentNature", SqlDbType.NVarChar, model.DocumentNature);
                        db.AddInParameter(dbCommandWrapper, "@PolicyApplicableTo", SqlDbType.NVarChar, model.PolicyApplicableTo);
                        db.AddInParameter(dbCommandWrapper, "@Emails", SqlDbType.NVarChar, model.Emails);
                        db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.NVarChar, model.Status);
                        db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, model.OwnerID);
                        db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, model.SetBy);
                        db.AddInParameter(dbCommandWrapper, "@Doc_Category", SqlDbType.Structured, docCategoryDataTable);

                        db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);
                        db.ExecuteNonQuery(dbCommandWrapper);

                        if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                        {
                            errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                        }
                    }
                }
                else if (model.PolicyApplicableTo == "Type")
                {
                    using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("SetDocTypeToDocDestroyPolicy"))
                    {
                        db.AddInParameter(dbCommandWrapper, "@DestroyPolicyID", SqlDbType.NVarChar, model.DestroyPolicyID ?? "");
                        db.AddInParameter(dbCommandWrapper, "@PolicyFor", SqlDbType.NVarChar, model.PolicyFor);
                        db.AddInParameter(dbCommandWrapper, "@DocumentNature", SqlDbType.NVarChar, model.DocumentNature);
                        db.AddInParameter(dbCommandWrapper, "@PolicyApplicableTo", SqlDbType.NVarChar, model.PolicyApplicableTo);
                        db.AddInParameter(dbCommandWrapper, "@Emails", SqlDbType.NVarChar, model.Emails);
                        db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.NVarChar, model.Status);
                        db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, model.OwnerID);
                        db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, model.SetBy);
                        db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, model.DocCategoryID);

                        db.AddInParameter(dbCommandWrapper, "@Doc_Type", SqlDbType.Structured, docTypeDataTable);
                        db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);
                        db.ExecuteNonQuery(dbCommandWrapper);

                        if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                        {
                            errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                        }
                    }
                }
                else if (model.PolicyApplicableTo == "Document")
                {
                    using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("SetDocPropertyToDocDestroyPolicy"))
                    {
                        db.AddInParameter(dbCommandWrapper, "@DestroyPolicyID", SqlDbType.NVarChar, model.DestroyPolicyID ?? "");
                        db.AddInParameter(dbCommandWrapper, "@PolicyFor", SqlDbType.NVarChar, model.PolicyFor);
                        db.AddInParameter(dbCommandWrapper, "@DocumentNature", SqlDbType.NVarChar, model.DocumentNature);
                        db.AddInParameter(dbCommandWrapper, "@PolicyApplicableTo", SqlDbType.NVarChar, model.PolicyApplicableTo);
                        db.AddInParameter(dbCommandWrapper, "@Emails", SqlDbType.NVarChar, model.Emails);
                        db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.NVarChar, model.Status);
                        db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, model.OwnerID);
                        db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, model.SetBy);
                        db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, model.DocCategoryID);
                        db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, model.DocTypeID);

                        db.AddInParameter(dbCommandWrapper, "@Doc_Property", SqlDbType.Structured, docPropertyDataTable);
                        db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);

                        db.ExecuteNonQuery(dbCommandWrapper);

                        if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                        {
                            errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                        }
                    }
                }
                else if (model.PolicyApplicableTo == "Attribute")
                {
                    using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("SetDocPropIdentityToDocDestroyPolicy"))
                    {
                        db.AddInParameter(dbCommandWrapper, "@DestroyPolicyID", SqlDbType.NVarChar, model.DestroyPolicyID ?? "");
                        db.AddInParameter(dbCommandWrapper, "@PolicyFor", SqlDbType.NVarChar, model.PolicyFor);
                        db.AddInParameter(dbCommandWrapper, "@DocumentNature", SqlDbType.NVarChar, model.DocumentNature);
                        db.AddInParameter(dbCommandWrapper, "@PolicyApplicableTo", SqlDbType.NVarChar, model.PolicyApplicableTo);
                        db.AddInParameter(dbCommandWrapper, "@Emails", SqlDbType.NVarChar, model.Emails);
                        db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.NVarChar, model.Status);
                        db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, model.OwnerID);
                        db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, model.SetBy);
                        db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, model.DocCategoryID);
                        db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, model.DocTypeID);
                        db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, model.DocPropertyID);

                        db.AddInParameter(dbCommandWrapper, "@Doc_PropIdentity", SqlDbType.Structured, docPropIdentifyDataTable);
                        db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);

                        db.ExecuteNonQuery(dbCommandWrapper);

                        if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                        {
                            errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                errorNumber = "E404";
            }
            return errorNumber;
        }





        public List<DocSearch> GetDocByPolicy(string _DestroyPolicyID, string _UserID, out string _errorNumber)
        {
            _errorNumber = string.Empty;
            List<DocSearch> documents = new List<DocSearch>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetDocumentsByPolicy"))
            {
                db.AddInParameter(dbCommandWrapper, "@DestroyPolicyID", SqlDbType.VarChar, _DestroyPolicyID);
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, _UserID);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    _errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        documents = dt1.AsEnumerable().Select(reader => new DocSearch
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
                            //OwnerLevelID = reader.GetString("OwnerLevelID"),
                            OwnerID = reader.GetString("OwnerID"),
                            DocCategoryID = reader.GetString("DocCategoryID"),
                            DocTypeID = reader.GetString("DocTypeID"),
                            DocPropertyID = reader.GetString("DocPropertyID"),
                            DocPropertyName = reader.GetString("DocPropertyName"),
                            SetOn=reader.GetDateTime("SetOn"),
                            DeleteOn = reader.GetDateTime("DeleteOn"),
                            DocumentNature = reader.GetString("DocumentNature"),
                            Status = reader.GetString("Status")
                        }).ToList();
                    }
                }

            }
            return documents;
        }


        public string SaveImportantDocuments(string documents, string _DestroyPolicyID, string _UserID, out string errorNumber)
        {
            errorNumber = String.Empty;
            try
            {
                //IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("SaveImpostantDocuments"))
                {
                    db.AddInParameter(dbCommandWrapper, "@Documents", SqlDbType.NVarChar, documents);
                    db.AddInParameter(dbCommandWrapper, "@DestroyPolicyID", SqlDbType.NVarChar, _DestroyPolicyID);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, _UserID);

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
        public List<DocSearch> GetDeletableDoc(out string _errorNumber)
        {
            _errorNumber = string.Empty;
            List<DocSearch> documents = new List<DocSearch>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetAllDeletableDoc"))
            {
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, "");
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    _errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        documents = dt1.AsEnumerable().Select(reader => new DocSearch
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
                            //OwnerLevelID = reader.GetString("OwnerLevelID"),
                            OwnerID = reader.GetString("OwnerID"),
                            DocCategoryID = reader.GetString("DocCategoryID"),
                            DocTypeID = reader.GetString("DocTypeID"),
                            DocPropertyID = reader.GetString("DocPropertyID"),
                            DocPropertyName = reader.GetString("DocPropertyName"),
                            SetOn = reader.GetDateTime("SetOn"),
                            DeleteOn = reader.GetDateTime("DeleteOn"),
                            AutoDelete = reader.GetInt16("AutoDelete"),
                            Status = reader.GetString("Status")
                        }).ToList();
                    }
                }

            }
            return documents;
        }


        public string DeleteDocByPolicy(string documents, out string errorNumber)
        {
            errorNumber = String.Empty;
            try
            {
                //IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("DeleteDocByPolicy"))
                {
                    db.AddInParameter(dbCommandWrapper, "@Documents", SqlDbType.NVarChar, documents);
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


        public string DeleteDocuments(string documents, string _DestroyPolicyID, string _UserID, out string errorNumber)
        {
            errorNumber = String.Empty;
            try
            {
                //IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("DeleteSelectedDoc"))
                {
                    db.AddInParameter(dbCommandWrapper, "@Documents", SqlDbType.NVarChar, documents);
                    db.AddInParameter(dbCommandWrapper, "@DestroyPolicyID", SqlDbType.NVarChar, _DestroyPolicyID);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, _UserID);
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
    }
}
