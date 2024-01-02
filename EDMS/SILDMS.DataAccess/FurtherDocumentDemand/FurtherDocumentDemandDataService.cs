using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.FurtherDocumentDemand;
using SILDMS.Model;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;

namespace SILDMS.DataAccess.FurtherDocumentDemand
{
    public class FurtherDocumentDemandDataService : IFurtherDocumentDemandDataService
    {
        #region Fields

        private readonly string _spStatusParam;

        #endregion

        #region Constructor
        public FurtherDocumentDemandDataService()
        {
            _spStatusParam = "@p_Status";
        }
        #endregion

        public List<DSM_Owner> GetCompanies(string id, string action, out string errorNumber)
        {
            errorNumber = string.Empty;
            var owners = new List<DSM_Owner>();

            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetAllOwnerByCompany"))
            {
                // Set parameters 
                db.AddOutParameter(dbCommandWrapper, _spStatusParam, DbType.String, 10);
                // Execute SP.
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                {
                    //Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return owners;
                    var dt1 = ds.Tables[0];
                    owners = dt1.AsEnumerable().Select(reader => new DSM_Owner
                    {
                        OwnerID = reader.GetString("OwnerID"),
                        OwnerName = reader.GetString("OwnerName")
                    }).ToList();
                }
            }
            return owners;
        }

        public List<BPS_BillProcessGroup> GetBillProcessGroups(string id, string action, out string errorNumber)
        {
            errorNumber = string.Empty;
            var billsProcessGroup = new List<BPS_BillProcessGroup>();

            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetBillProcessGroup"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, id);
                db.AddOutParameter(dbCommandWrapper, _spStatusParam, DbType.String, 10);
                // Execute SP.
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                {
                    //Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return billsProcessGroup;
                    var dt1 = ds.Tables[0];
                    billsProcessGroup = dt1.AsEnumerable().Select(reader => new BPS_BillProcessGroup
                    {
                        ProcessGroupID = reader.GetString("ProcessGroupID"),
                        ProcessGroupName = reader.GetString("ProcessGroupName")
                    }).ToList();
                }
            }
            return billsProcessGroup;
        }

        public List<BPS_BillProcessingStage> GetBillProcessingStages(string stageId, string groupId, string action, out string errorNumber)
        {
            errorNumber = string.Empty;
            var billsProcessingStages = new List<BPS_BillProcessingStage>();

            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetBillProcessingStage"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@StageID", SqlDbType.NVarChar, stageId);
                db.AddInParameter(dbCommandWrapper, "@ProcessGroupID", SqlDbType.NVarChar, groupId);
                db.AddOutParameter(dbCommandWrapper, _spStatusParam, DbType.String, 10);
                // Execute SP.
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                {
                    //Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return billsProcessingStages;
                    var dt1 = ds.Tables[0];
                    billsProcessingStages = dt1.AsEnumerable().Select(reader => new BPS_BillProcessingStage
                    {
                        StageID = reader.GetString("StageID"),
                        StageName = reader.GetString("StageName")
                    }).ToList();
                }
            }
            return billsProcessingStages;
        }

        public List<BPS_BillReceive> GetBillReceiveForFDD(string billTrackingNo, out string errorNumber)
        {
            errorNumber = string.Empty;
            var billReceives = new List<BPS_BillReceive>();

            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetBillReceiveForFDD"))
            {
                // Set parameters 

                db.AddInParameter(dbCommandWrapper, "@BillTrackingNo", SqlDbType.NVarChar, billTrackingNo);
                db.AddOutParameter(dbCommandWrapper, _spStatusParam, DbType.String, 10);
                // Execute SP.
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                {
                    //Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return billReceives;
                    var dt1 = ds.Tables[0];
                    
                    billReceives = dt1.AsEnumerable().Select(reader => new BPS_BillReceive
                    {
                        BillReceiveID = reader.GetString("BillReceiveID"),
                        PONo = reader.GetString("PONo"),
                        VendorID = reader.GetString("VendorID"),
                        VendorCode = reader.GetString("VendorCode"),
                        VendorName = reader.GetString("VendorName"),
                        VendorEmail = reader.GetString("VendorEmail"),
                        VendorMobileNo = reader.GetString("VendorMobileNo"),
                        BillTrackingNo = reader.GetString("BillTrackingNo"),
                        InvoicingParty = reader.GetString("InvoicingParty"),
                        InvoicingPartyName = reader.GetString("InvoicingPartyName"),
                        InvoiceNo = reader.GetString("InvoiceNo"),
                        InvoiceDate = string.Format("{0:dddd, MMMM d, yyyy}", reader.GetDateTime("InvoiceDate")),
                        ProcessState = reader.GetString("ProcessState"),
                        ProcessGroupID = reader.GetString("ProcessGroupID"),
                        ProcessGroupName = reader.GetString("ProcessGroupName"),
                        CurrentStage = reader.GetString("CurrentStage"),
                        StageName = reader.GetString("StageName"),
                        OwnerID = reader.GetString("OwnerID"),
                        OwnerName = reader.GetString("OwnerName"),
                        DocCategoryID = reader.GetString("DocCategoryID"),
                        DocTypeID = reader.GetString("DocTypeID")
                    }).ToList();
                }
            }
            return billReceives;
        }

        public List<DSM_DocProperty> GetDocPropertyForFDD(string docCatId, string docPropId, out string errorNumber)
        {
            errorNumber = string.Empty;
            var docProps = new List<DSM_DocProperty>();

            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetDocPropertyForFDD"))
            {
                // Set parameters 

                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, docCatId);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, docPropId);
                db.AddOutParameter(dbCommandWrapper, _spStatusParam, DbType.String, 10);
                // Execute SP.
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                {
                    //Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return docProps;
                    var dt1 = ds.Tables[0];

                    docProps = dt1.AsEnumerable().Select(reader => new DSM_DocProperty
                    {
                        DocPropertyID = reader.GetString("DocPropertyID"),
                        DocPropertyName = reader.GetString("DocPropertyName"),
                        DocTypeID = reader.GetString("DocTypeID")
                    }).ToList();
                }
            }
            return docProps;
        }

        public string SetMailSmsAndFurtherDoc(BPS_MailSms mailSms, BPS_MailSmsSend mailSmsSend, BPS_FurtherDemand furtherDemand, 
            List<BPS_FurtherDemandDoc> futherDemandDoc, string action, out string errorNumber)
        {
            errorNumber = string.Empty;

            #region Convert mailSms object to datatable
            var mailSmsDataTable = new DataTable();
            mailSmsDataTable.Columns.Add("MailSmsID");
            mailSmsDataTable.Columns.Add("BillTrackingNo");
            mailSmsDataTable.Columns.Add("VendorID");
            mailSmsDataTable.Columns.Add("PONo");
            mailSmsDataTable.Columns.Add("ReleaseIndicator");
            mailSmsDataTable.Columns.Add("DeletionIndicator");
            mailSmsDataTable.Columns.Add("CompletionStatus");
            mailSmsDataTable.Columns.Add("OwnerID");
            mailSmsDataTable.Columns.Add("DocCategoryID");
            mailSmsDataTable.Columns.Add("DocTypeID");
            mailSmsDataTable.Columns.Add("DocPropertyID");
            mailSmsDataTable.Columns.Add("UserLevel");
            mailSmsDataTable.Columns.Add("SetOn");
            mailSmsDataTable.Columns.Add("SetBy");
            mailSmsDataTable.Columns.Add("ModifiedOn");
            mailSmsDataTable.Columns.Add("ModifiedBy");
            mailSmsDataTable.Columns.Add("Status");

            var mailSmsDataTableRow = mailSmsDataTable.NewRow();
            mailSmsDataTableRow[0] = mailSms.MailSmsID;
            mailSmsDataTableRow[1] = mailSms.BillTrackingNo;
            mailSmsDataTableRow[2] = mailSms.VendorID;
            mailSmsDataTableRow[3] = mailSms.PONo;
            mailSmsDataTableRow[4] = mailSms.ReleaseIndicator;
            mailSmsDataTableRow[5] = mailSms.DeletionIndicator;
            mailSmsDataTableRow[6] = mailSms.CompletionStatus;
            mailSmsDataTableRow[7] = mailSms.OwnerID;
            mailSmsDataTableRow[8] = mailSms.DocCategoryID;
            mailSmsDataTableRow[9] = mailSms.DocTypeID;
            mailSmsDataTableRow[10] = mailSms.DocPropertyID;
            mailSmsDataTableRow[11] = mailSms.UserLevel;
            mailSmsDataTableRow[12] = mailSms.SetOn;
            mailSmsDataTableRow[13] = mailSms.SetBy;
            mailSmsDataTableRow[14] = mailSms.ModifiedOn;
            mailSmsDataTableRow[15] = mailSms.ModifiedBy;
            mailSmsDataTableRow[16] = mailSms.Status;

            mailSmsDataTable.Rows.Add(mailSmsDataTableRow);
            #endregion

            #region Convert mailSmsSend object to datatable

            var mailSmsSendDataTable = new DataTable();
            mailSmsSendDataTable.Columns.Add("MailSmsSendID");
            mailSmsSendDataTable.Columns.Add("MailSmsID");
            mailSmsSendDataTable.Columns.Add("MailAddress");
            mailSmsSendDataTable.Columns.Add("MailCC");
            mailSmsSendDataTable.Columns.Add("SmsMobileNo");
            mailSmsSendDataTable.Columns.Add("SmsCC");
            mailSmsSendDataTable.Columns.Add("MailText");
            mailSmsSendDataTable.Columns.Add("SmsText");
            mailSmsSendDataTable.Columns.Add("MailSendStatus");
            mailSmsSendDataTable.Columns.Add("SmsSendStatus");
            mailSmsSendDataTable.Columns.Add("MailSendFrom");
            mailSmsSendDataTable.Columns.Add("SmsSendFrom");
            mailSmsSendDataTable.Columns.Add("MailSmsReason");
            mailSmsSendDataTable.Columns.Add("SendDate");
            mailSmsSendDataTable.Columns.Add("ReferenceNo");
            mailSmsSendDataTable.Columns.Add("Note");
            mailSmsSendDataTable.Columns.Add("DeletionIndicator");
            mailSmsSendDataTable.Columns.Add("CompletionStatus");
            mailSmsSendDataTable.Columns.Add("ProcessState");
            mailSmsSendDataTable.Columns.Add("ProcessGroupID");
            mailSmsSendDataTable.Columns.Add("StageID");
            mailSmsSendDataTable.Columns.Add("OwnerID");
            mailSmsSendDataTable.Columns.Add("DocCategoryID");
            mailSmsSendDataTable.Columns.Add("DocTypeID");
            mailSmsSendDataTable.Columns.Add("DocPropertyID");
            mailSmsSendDataTable.Columns.Add("UserLevel");
            mailSmsSendDataTable.Columns.Add("SetOn");
            mailSmsSendDataTable.Columns.Add("SetBy");
            mailSmsSendDataTable.Columns.Add("ModifiedOn");
            mailSmsSendDataTable.Columns.Add("ModifiedBy");
            mailSmsSendDataTable.Columns.Add("Status");

            var mailSmsSendDataTableRow = mailSmsSendDataTable.NewRow();
            mailSmsSendDataTableRow[0] = mailSmsSend.MailSmsSendID;
            mailSmsSendDataTableRow[1] = mailSmsSend.MailSmsID;
            mailSmsSendDataTableRow[2] = mailSmsSend.MailAddress;
            mailSmsSendDataTableRow[3] = mailSmsSend.MailCC;
            mailSmsSendDataTableRow[4] = mailSmsSend.SmsMobileNo;
            mailSmsSendDataTableRow[5] = mailSmsSend.SmsCC;
            mailSmsSendDataTableRow[6] = mailSmsSend.MailText;
            mailSmsSendDataTableRow[7] = mailSmsSend.SmsText;
            mailSmsSendDataTableRow[8] = mailSmsSend.MailSendStatus;
            mailSmsSendDataTableRow[9] = mailSmsSend.SmsSendStatus;
            mailSmsSendDataTableRow[10] = mailSmsSend.MailSendFrom;
            mailSmsSendDataTableRow[11] = mailSmsSend.SmsSendFrom;
            mailSmsSendDataTableRow[12] = mailSmsSend.MailSmsReason;
            mailSmsSendDataTableRow[13] = mailSmsSend.SendDate;
            mailSmsSendDataTableRow[14] = mailSmsSend.ReferenceNo;
            mailSmsSendDataTableRow[15] = mailSmsSend.Note;
            mailSmsSendDataTableRow[16] = mailSmsSend.DeletionIndicator;
            mailSmsSendDataTableRow[17] = mailSmsSend.CompletionStatus;
            mailSmsSendDataTableRow[18] = mailSmsSend.ProcessState;
            mailSmsSendDataTableRow[19] = mailSmsSend.ProcessGroupID;
            mailSmsSendDataTableRow[20] = mailSmsSend.StageID;
            mailSmsSendDataTableRow[21] = mailSmsSend.OwnerID;
            mailSmsSendDataTableRow[22] = mailSmsSend.DocCategoryID;
            mailSmsSendDataTableRow[23] = mailSmsSend.DocTypeID;
            mailSmsSendDataTableRow[24] = mailSmsSend.DocPropertyID;
            mailSmsSendDataTableRow[25] = mailSmsSend.UserLevel; 
            mailSmsSendDataTableRow[26] = mailSmsSend.SetOn;
            mailSmsSendDataTableRow[27] = mailSmsSend.SetBy;
            mailSmsSendDataTableRow[28] = mailSmsSend.ModifiedOn;
            mailSmsSendDataTableRow[29] = mailSmsSend.ModifiedBy;
            mailSmsSendDataTableRow[30] = mailSmsSend.Status;

            mailSmsSendDataTable.Rows.Add(mailSmsSendDataTableRow);
            #endregion

            #region Convert furtherDemand object to datatabel

            var furtherDemandDataTable = new DataTable();
            furtherDemandDataTable.Columns.Add("DemandID");
            furtherDemandDataTable.Columns.Add("PONo");
            furtherDemandDataTable.Columns.Add("BillTrackingNo");
            furtherDemandDataTable.Columns.Add("DemandTo");
            furtherDemandDataTable.Columns.Add("MailSmsSendID");
            furtherDemandDataTable.Columns.Add("DeletionIndicator");
            furtherDemandDataTable.Columns.Add("CompletionStatus");
            furtherDemandDataTable.Columns.Add("ProcessState");
            furtherDemandDataTable.Columns.Add("ProcessGroupID");
            furtherDemandDataTable.Columns.Add("StageID");
            furtherDemandDataTable.Columns.Add("OwnerID");
            furtherDemandDataTable.Columns.Add("DocCategoryID");
            furtherDemandDataTable.Columns.Add("DocTypeID");
            furtherDemandDataTable.Columns.Add("DocPropertyID");
            furtherDemandDataTable.Columns.Add("UserLevel");
            furtherDemandDataTable.Columns.Add("SetOn");
            furtherDemandDataTable.Columns.Add("SetBy");
            furtherDemandDataTable.Columns.Add("ModifiedOn");
            furtherDemandDataTable.Columns.Add("ModifiedBy");
            furtherDemandDataTable.Columns.Add("Status");

            var furtherDemandDataTableRow = furtherDemandDataTable.NewRow();
            furtherDemandDataTableRow[0] = furtherDemand.DemandID;
            furtherDemandDataTableRow[1] = furtherDemand.PONo;
            furtherDemandDataTableRow[2] = furtherDemand.BillTrackingNo;
            furtherDemandDataTableRow[3] = furtherDemand.DemandTo;
            furtherDemandDataTableRow[4] = furtherDemand.MailSmsSendID;
            furtherDemandDataTableRow[5] = furtherDemand.DeletionIndicator;
            furtherDemandDataTableRow[6] = furtherDemand.CompletionStatus;
            furtherDemandDataTableRow[7] = furtherDemand.ProcessState;
            furtherDemandDataTableRow[8] = furtherDemand.ProcessGroupID;
            furtherDemandDataTableRow[9] = furtherDemand.StageID;
            furtherDemandDataTableRow[10] = furtherDemand.OwnerID;
            furtherDemandDataTableRow[11] = furtherDemand.DocCategoryID;
            furtherDemandDataTableRow[12] = furtherDemand.DocTypeID;
            furtherDemandDataTableRow[13] = furtherDemand.DocPropertyID;
            furtherDemandDataTableRow[14] = furtherDemand.UserLevel;
            furtherDemandDataTableRow[15] = furtherDemand.SetOn;
            furtherDemandDataTableRow[16] = furtherDemand.SetBy;
            furtherDemandDataTableRow[17] = furtherDemand.ModifiedOn;
            furtherDemandDataTableRow[18] = furtherDemand.ModifiedBy;
            furtherDemandDataTableRow[19] = furtherDemand.Status;

            furtherDemandDataTable.Rows.Add(furtherDemandDataTableRow);
            #endregion

            #region Convert furtherDemandDocs object to datatable

            var furtherDemandDocsDataTable = new DataTable();
            furtherDemandDocsDataTable.Columns.Add("DemandDocID");
            furtherDemandDocsDataTable.Columns.Add("DemandID");
            furtherDemandDocsDataTable.Columns.Add("DocumentID");
            furtherDemandDocsDataTable.Columns.Add("DocPropertyID");
            furtherDemandDocsDataTable.Columns.Add("DocumentName");
            furtherDemandDocsDataTable.Columns.Add("DocumentNature");
            furtherDemandDocsDataTable.Columns.Add("NoOfCopy");
            furtherDemandDocsDataTable.Columns.Add("NecessityType");
            furtherDemandDocsDataTable.Columns.Add("RequireBy");
            furtherDemandDocsDataTable.Columns.Add("DocNote");
            furtherDemandDocsDataTable.Columns.Add("DeletionIndicator");
            furtherDemandDocsDataTable.Columns.Add("CompletionStatus");
            furtherDemandDocsDataTable.Columns.Add("OwnerID");
            furtherDemandDocsDataTable.Columns.Add("DocCategoryID");
            furtherDemandDocsDataTable.Columns.Add("DocTypeID");
            furtherDemandDocsDataTable.Columns.Add("UserLevel");
            furtherDemandDocsDataTable.Columns.Add("SetOn");
            furtherDemandDocsDataTable.Columns.Add("SetBy");
            furtherDemandDocsDataTable.Columns.Add("ModifiedOn");
            furtherDemandDocsDataTable.Columns.Add("ModifiedBy");
            furtherDemandDocsDataTable.Columns.Add("Status");

            foreach (var doc in futherDemandDoc)
            {
                var futherDemandDocRow = furtherDemandDocsDataTable.NewRow();
                futherDemandDocRow[0] = doc.DemandDocID;
                futherDemandDocRow[1] = doc.DemandID;
                futherDemandDocRow[2] = doc.DocumentID;
                futherDemandDocRow[3] = doc.DocPropertyID;
                futherDemandDocRow[4] = doc.DocumentName;
                futherDemandDocRow[5] = doc.DocumentNature;
                futherDemandDocRow[6] = doc.NoOfCopy;
                futherDemandDocRow[7] = doc.NecessityType;
                futherDemandDocRow[8] = doc.RequireBy == null ? (DateTime?)null : DataValidation.DateTimeConversion(doc.RequireBy);
                futherDemandDocRow[9] = doc.DocNote;
                futherDemandDocRow[10] = doc.DeletionIndicator;
                futherDemandDocRow[11] = doc.CompletionStatus;
                futherDemandDocRow[12] = doc.OwnerID;
                futherDemandDocRow[13] = doc.DocCategoryID;
                futherDemandDocRow[14] = doc.DocTypeID;
                futherDemandDocRow[15] = doc.UserLevel;
                futherDemandDocRow[16] = doc.SetOn;
                futherDemandDocRow[17] = doc.SetBy;
                futherDemandDocRow[18] = doc.ModifiedOn;
                futherDemandDocRow[19] = doc.ModifiedBy;
                futherDemandDocRow[20] = doc.Status;

                furtherDemandDocsDataTable.Rows.Add(futherDemandDocRow);
            }

            #endregion
            try
            {
                var factory = new DatabaseProviderFactory();
                var db = factory.CreateDefault() as SqlDatabase;
                using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_SetFurtherDocumentDemand"))
                {
                    db.AddInParameter(dbCommandWrapper, "@BPS_MailSms", SqlDbType.Structured, mailSmsDataTable);
                    db.AddInParameter(dbCommandWrapper, "@BPS_MailSmsSend", SqlDbType.Structured, mailSmsSendDataTable);
                    db.AddInParameter(dbCommandWrapper, "@BPS_FurtherDemand", SqlDbType.Structured, furtherDemandDataTable);
                    db.AddInParameter(dbCommandWrapper, "@BPS_FurtherDemandDoc", SqlDbType.Structured, furtherDemandDocsDataTable);
                    db.AddInParameter(dbCommandWrapper, "@Action", SqlDbType.NVarChar, action);
                    db.AddOutParameter(dbCommandWrapper, _spStatusParam, SqlDbType.VarChar, 10);

                    db.ExecuteNonQuery(dbCommandWrapper);
                    // Getting output parameters and setting response details.
                    if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                    {
                        // Get the error number, if error occurred.
                        errorNumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                    }
                }
            }
            catch
            {
                errorNumber = "E404"; // Log ex.Message  Insert Log Table               
            }
            return errorNumber;
        }


        //public  DataTable ToDataTable<T>(IList<T> data)
        //{
        //    PropertyDescriptorCollection props =
        //        TypeDescriptor.GetProperties(typeof(T));
        //    DataTable table = new DataTable();
        //    for (int i = 0; i < props.Count; i++)
        //    {
        //        PropertyDescriptor prop = props[i];
        //        table.Columns.Add(prop.Name, prop.PropertyType);
        //    }
        //    object[] values = new object[props.Count];
        //    foreach (T item in data)
        //    {
        //        for (int i = 0; i < values.Length; i++)
        //        {
        //            if (props[i].PropertyType == typeof(DateTime))
        //            {
        //                DateTime currDT = (DateTime)props[i].GetValue(item);
        //                values[i] = currDT.ToUniversalTime();
        //            }
        //            else
        //            {
        //                values[i] = props[i].GetValue(item);
        //            }
        //        }
        //        table.Rows.Add(values);
        //    }
        //    return table;
        //}
       
    }
}
