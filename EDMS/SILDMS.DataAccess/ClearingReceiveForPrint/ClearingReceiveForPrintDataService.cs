using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.ClearingReceiveForPrint;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.CheckPrintModule;

namespace SILDMS.DataAccess.ClearingReceiveForPrint
{
    public class ClearingReceiveForPrintDataService : IClearingReceiveForPrintDataService
    {
        #region Fields

        private readonly string _spStatusParam;

        #endregion

        #region Constructor

        public ClearingReceiveForPrintDataService()
        {
            _spStatusParam = "@p_Status";
        }

        #endregion


        public List<CBPSBillClearing> GetClearings(string docNo ,string dateFrom, string dateTo,
            out string errorNumber)
        {
            errorNumber = string.Empty;
            var clearings = new List<CBPSBillClearing>();
            var companyCode = docNo == "" || docNo.IsNullOrZero() ? "" : docNo.Substring(0, 4);
            var clrDocNo = docNo == "" || docNo.IsNullOrZero() ? "" : docNo.Substring(4, 10);
            var fiscalYear = docNo == "" || docNo.IsNullOrZero() ? "" : docNo.Substring(14, 4);
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetClearingForPrint"))
            {
                
                // Set parameters 

                db.AddInParameter(dbCommandWrapper, "@CompanyCode", SqlDbType.NVarChar, companyCode);
                db.AddInParameter(dbCommandWrapper, "@DocNo", SqlDbType.NVarChar, clrDocNo);
                db.AddInParameter(dbCommandWrapper, "@FiscalYear", SqlDbType.NVarChar, fiscalYear);
                db.AddInParameter(dbCommandWrapper, "@DateFrom", SqlDbType.NVarChar, dateFrom);
                db.AddInParameter(dbCommandWrapper, "@DateTo", SqlDbType.NVarChar, dateTo);
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
                    if (ds.Tables[0].Rows.Count <= 0) return clearings;
                    var dt1 = ds.Tables[0];
                    clearings = dt1.AsEnumerable().Select(reader => new CBPSBillClearing
                    {
                        BillClearingID = reader.GetString("BillClearingID"),
                        InvoiceDocNo = reader.GetString("InvoiceDocNo"),
                        ReleaseDate = reader.GetString("ReleaseDate"),
                        FiscalYear = reader.GetString("FiscalYear"),
                        DisplayReleaseDate = string.Format("{0:dd/MM/yyyy}", reader.GetDateTime("ReleaseDate")),
                        VendorCode = reader.GetString("Vendor"),
                        VendorName = reader.GetString("VendorName"),
                        InvoicingPartyCode = reader.GetString("InvoicingParty"),
                        InvoicingPartyName = reader.GetString("InvoicingPartyName"),
                        CompanyCode = reader.GetString("CompanyCode"),
                        CompanyName = reader.GetString("CompanyName"),
                        AmtInLocalCurrency = reader.GetToDecimal("AmtInLocalCurrency")
                        
                    }).ToList();
                }
            }
            return clearings;
        }

        public string SetClearingReceiveDocs(List<CBPSBillClearing> docs, out string errorNumber)
        {
            errorNumber = string.Empty;
            #region Convert CBPSBillClearing object to datatable
            var billClearingDataTable = new DataTable();
            billClearingDataTable.Columns.Add("BillClearingID");
            billClearingDataTable.Columns.Add("CompanyCode");
            billClearingDataTable.Columns.Add("BillTrackingNo");
            billClearingDataTable.Columns.Add("InvoiceDocNo");
            billClearingDataTable.Columns.Add("Vendor");
            billClearingDataTable.Columns.Add("InvoicingParty");
            billClearingDataTable.Columns.Add("FiscalYear");
            billClearingDataTable.Columns.Add("InvoiceDocType");
            billClearingDataTable.Columns.Add("InvoiceDocDate");
            billClearingDataTable.Columns.Add("PostingDate");
            billClearingDataTable.Columns.Add("EnteredOn");
            billClearingDataTable.Columns.Add("EnteredAt");
            billClearingDataTable.Columns.Add("ChangedOn");
            billClearingDataTable.Columns.Add("LastUpdate");
            billClearingDataTable.Columns.Add("Reference");
            billClearingDataTable.Columns.Add("InvoiceCurrency");
            billClearingDataTable.Columns.Add("ReferenceKey");
            billClearingDataTable.Columns.Add("ReleaseIndicator");
            billClearingDataTable.Columns.Add("DeletionIndecator");
            billClearingDataTable.Columns.Add("CompletionStatus");
            billClearingDataTable.Columns.Add("ProcessState");
            billClearingDataTable.Columns.Add("DataPullingFlag");
            billClearingDataTable.Columns.Add("DataSource");
            billClearingDataTable.Columns.Add("ProcessGroupID");
            billClearingDataTable.Columns.Add("StageID");
            billClearingDataTable.Columns.Add("OwnerID");
            billClearingDataTable.Columns.Add("DocCategoryID");
            billClearingDataTable.Columns.Add("DocTypeID");
            billClearingDataTable.Columns.Add("DocPropertyID");
            billClearingDataTable.Columns.Add("UserLevel");
            billClearingDataTable.Columns.Add("SetOn");
            billClearingDataTable.Columns.Add("SetBy");
            billClearingDataTable.Columns.Add("ModifiedOn");
            billClearingDataTable.Columns.Add("ModifiedBy");
            billClearingDataTable.Columns.Add("Status");

            #endregion

            foreach (var doc in docs)
            {
                var billClearingDataTableRow = billClearingDataTable.NewRow();
                billClearingDataTableRow[0] = doc.BillClearingID;
                billClearingDataTableRow[1] = doc.CompanyCode;
                billClearingDataTableRow[2] = doc.BillTrackingNo;
                billClearingDataTableRow[3] = doc.InvoiceDocNo;
                billClearingDataTableRow[4] = doc.VendorCode;
                billClearingDataTableRow[5] = doc.InvoicingPartyCode;
                billClearingDataTableRow[6] = doc.FiscalYear;
                billClearingDataTableRow[7] = doc.InvoiceDocType;
                billClearingDataTableRow[8] = doc.InvoiceDocDate;
                billClearingDataTableRow[9] = doc.PostingDate;
                billClearingDataTableRow[10] = doc.EnteredOn;
                billClearingDataTableRow[11] = doc.EnteredAt;
                billClearingDataTableRow[12] = doc.ChangedOn;
                billClearingDataTableRow[13] = doc.LastUpdate;
                billClearingDataTableRow[14] = doc.Reference;
                billClearingDataTableRow[15] = doc.InvoiceCurrency;
                billClearingDataTableRow[16] = doc.ReferenceKey;
                billClearingDataTableRow[17] = doc.ReleaseIndicator;
                billClearingDataTableRow[18] = doc.DeletionIndecator;
                billClearingDataTableRow[19] = doc.CompletionStatus;
                billClearingDataTableRow[20] = doc.ProcessState;
                billClearingDataTableRow[21] = doc.DataPullingFlag;
                billClearingDataTableRow[22] = doc.DataSource;
                billClearingDataTableRow[23] = doc.ProcessGroupID;
                billClearingDataTableRow[24] = doc.StageID;
                billClearingDataTableRow[25] = doc.OwnerID;
                billClearingDataTableRow[26] = doc.DocCategoryID;
                billClearingDataTableRow[27] = doc.DocTypeID;
                billClearingDataTableRow[28] = doc.DocPropertyID;
                billClearingDataTableRow[29] = doc.UserLevel;
                billClearingDataTableRow[30] = doc.SetOn;
                billClearingDataTableRow[31] = doc.SetBy;
                billClearingDataTableRow[32] = doc.ModifiedOn;
                billClearingDataTableRow[33] = doc.ModifiedBy;
                billClearingDataTableRow[34] = doc.Status;

                billClearingDataTable.Rows.Add(billClearingDataTableRow);

            }


            try
            {
                var factory = new DatabaseProviderFactory();
                var db = factory.CreateDefault() as SqlDatabase;
                using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_SetClearingForPrint"))
                {
                    db.AddInParameter(dbCommandWrapper, "@BPS_BillClearing", SqlDbType.Structured, billClearingDataTable);
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

    }
}
