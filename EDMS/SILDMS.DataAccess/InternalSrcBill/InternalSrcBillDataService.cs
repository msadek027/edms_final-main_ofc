using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.DataAccessInterface.InternalSrcBill;
using SILDMS.Model.CBPSModule;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;

namespace SILDMS.DataAccess.InternalSrcBill
{
    public class InternalSrcBillDataService: IInternalSrcBillDataService
    {
        private readonly string spStatusParam = "@p_Status";
        public List<BPS_BillReceive> FindBills(string BillTrackingNoFrom, string BillTrackingNoTo, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<BPS_BillReceive> billReceiveList = new List<BPS_BillReceive>();


            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;

            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetBillList"))
            {
                db.AddInParameter(dbCommandWrapper, "@BillTrackingNoFrom", SqlDbType.VarChar, BillTrackingNoFrom);
                db.AddInParameter(dbCommandWrapper, "@BillTrackingNoTo", SqlDbType.VarChar, BillTrackingNoTo);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    billReceiveList = dt1.AsEnumerable().Select(reader => new BPS_BillReceive
                    {
                        BillReceiveID = reader.GetString("BillReceiveID"),
                        BillTrackingNo = reader.GetString("BillTrackingNo"),
                        BillSubmitDate = reader.GetDateTime("BillSubmitDate").ToString("dd/MMM/yyyy"),
                        InvoiceNo = reader.GetString("InvoiceNo"),
                        InvoiceAmt = reader.GetToDecimal("InvoiceAmt"),
                        VendorName = reader.GetString("VendorName"),
                        StageName = reader.GetString("StageName")
                    }).ToList();
                }               

            }
            return billReceiveList;
        }

        public List<DonutChartFor> ChartValue(string BillTrackingNo, out string _errorNumber)
        {
            _errorNumber = string.Empty;
            List<DonutChartFor> chartValueList = new List<DonutChartFor>();


            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;

            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_ChartValueList"))
            {
                db.AddInParameter(dbCommandWrapper, "@BillTrackingNo", SqlDbType.VarChar, BillTrackingNo);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    _errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    chartValueList = dt1.AsEnumerable().Select(reader => new DonutChartFor
                    {
                        daysForReceive = reader.GetString("daysForReceive"),
                        daysForparking = reader.GetString("daysForparking"),
                        daysForPosting = reader.GetString("daysForPosting"),
                        daysForClearing = reader.GetString("daysForClearing"),
                        daysForCheckProcess = reader.GetString("daysForCheckProcess"),
                        daysForDisbursment = reader.GetString("daysForDisbursment"),
                       
                        ReceiveToNow = reader.GetString("ReceiveToNow"),
                        ParkingToNow = reader.GetString("ParkingToNow"),
                        PostingToNow = reader.GetString("PostingToNow"),
                        ClearToNow = reader.GetString("ClearToNow"),
                        PrintRecToNow = reader.GetString("PrintRecToNow"),
                        DisbursmentRecToNow = reader.GetString("DisbursmentRecToNow"),
                       
                       
                        ReceiveDT = reader.GetString("ReceiveDT"),
                        ParkInitiateDT = reader.GetString("ParkInitiateDT"),
                        ParkDoneDT = reader.GetString("ParkDoneDT"),
                        PostInitiateDT = reader.GetString("PostInitiateDT"),
                        PostDoneDT = reader.GetString("PostDoneDT"),
                        ClearInitiateDT = reader.GetString("ClearInitiateDT"),
                        ClearDoneDT = reader.GetString("ClearDoneDT"),
                        CheckPrintDT = reader.GetString("CheckPrintDT"),
                        AuditInitDT = reader.GetString("AuditInitDT"),
                        AuditDoneDT = reader.GetString("AuditDoneDT"),
                        SignInitDT = reader.GetString("SignInitDT"),
                        SignDoneDT = reader.GetString("SignDoneDT"),
                        ApproveInitDT = reader.GetString("ApproveInitDT"),
                        ApproveDoneDT = reader.GetString("ApproveDoneDT"),
                        DisburseInitDT = reader.GetString("DisburseInitDT"),
                        DisburseDoneDT = reader.GetString("DisburseDoneDT"),
                        CurrentStage = reader.GetString("DisburseDoneDT"),
                        TotDays = reader.GetString("TotDays"),
                    }).ToList();
                }

            }
            return chartValueList;
        }

        public List<DonutChartFor> GraphForBarChart(string BillTrackingNo, out string _errorNumber)
        {
            _errorNumber = string.Empty;
            List<DonutChartFor> chartValueList = new List<DonutChartFor>();


            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;

            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_BarChartValueList"))
            {
                db.AddInParameter(dbCommandWrapper, "@BillTrackingNo", SqlDbType.VarChar, BillTrackingNo);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    _errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    chartValueList = dt1.AsEnumerable().Select(reader => new DonutChartFor
                    {
                        daysForReceive = reader.GetString("Receiveday"),
                        daysForparking = reader.GetString("Parkingday"),
                        daysForPosting = reader.GetString("Postingday"),
                        daysForClearing = reader.GetString("ClearingDay"),
                        daysForCheckProcess = reader.GetString("CheckProcessDay"),
                        daysForDisbursment = reader.GetString("Disbursmentday"),

                    }).ToList();
                }

            }
            return chartValueList;
        }


        public List<CMSVendor> GetVendors(out string errorNumber)
        {
            errorNumber = string.Empty;
            List<CMSVendor> vendorList = new List<CMSVendor>();
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;

            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetAllVendors"))
            {
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    vendorList = dt1.AsEnumerable().Select(reader => new CMSVendor
                    {
                        VendorID = reader.GetString("VendorID"),
                        VendorCode = reader.GetString("VendorCode"),
                        VendorName = reader.GetString("VendorName"),
                        VendorAddressCode = reader.GetString("VendorAddressCode"),

                    }).ToList();
                }

            }
            return vendorList;
        }


        public List<BPS_BillReceive> GetBillByVendors(string vendorCode, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<BPS_BillReceive> billReceiveList = new List<BPS_BillReceive>();
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;

            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetBillListByVendor"))
            {
                db.AddInParameter(dbCommandWrapper, "@vendorCode", SqlDbType.VarChar, vendorCode);                
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    billReceiveList = dt1.AsEnumerable().Select(reader => new BPS_BillReceive
                    {
                        BillReceiveID = reader.GetString("BillReceiveID"),
                        BillTrackingNo = reader.GetString("BillTrackingNo"),
                        BillSubmitDate = reader.GetDateTime("BillSubmitDate").ToString("dd/MMM/yyyy"), //c.ReceiveDate == null ? "" : (Convert.ToDateTime(c.ReceiveDate)).ToString("dd/MM/yyyy"),
                        InvoiceNo = reader.GetString("InvoiceNo"),
                        InvoiceAmt = reader.GetToDecimal("InvoiceAmt"),
                        VendorName = reader.GetString("VendorName"),
                        StageName = reader.GetString("StageName")
                    }).ToList();
                }

            }
            return billReceiveList;
        }


        public List<BPS_BillReceive> GetBillByCompany(string OwnerID, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<BPS_BillReceive> billReceiveList = new List<BPS_BillReceive>();
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;

            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetBillListByCompany"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.VarChar, OwnerID);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    billReceiveList = dt1.AsEnumerable().Select(reader => new BPS_BillReceive
                    {
                        BillReceiveID = reader.GetString("BillReceiveID"),
                        BillTrackingNo = reader.GetString("BillTrackingNo"),
                        BillSubmitDate = reader.GetDateTime("BillSubmitDate").ToString("dd/MMM/yyyy"),
                        InvoiceNo = reader.GetString("InvoiceNo"),
                        InvoiceAmt = reader.GetToDecimal("InvoiceAmt"),
                        VendorName = reader.GetString("VendorName"),
                        StageName = reader.GetString("StageName")
                    }).ToList();
                }

            }
            return billReceiveList;
        }


        public List<BPS_BillReceive> FindBillbyDate(string RecDateOne, string RecDateTwo, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<BPS_BillReceive> billReceiveList = new List<BPS_BillReceive>();


            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;

            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetBillListByRecDate"))
            {
                db.AddInParameter(dbCommandWrapper, "@RecDateOne", SqlDbType.VarChar, RecDateOne);
                db.AddInParameter(dbCommandWrapper, "@RecDateTwo", SqlDbType.VarChar, RecDateTwo);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);

                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    billReceiveList = dt1.AsEnumerable().Select(reader => new BPS_BillReceive
                    {
                        BillReceiveID = reader.GetString("BillReceiveID"),
                        BillTrackingNo = reader.GetString("BillTrackingNo"),
                        BillSubmitDate = reader.GetDateTime("BillSubmitDate").ToString("dd/MMM/yyyy"),
                        InvoiceNo = reader.GetString("InvoiceNo"),
                        InvoiceAmt = reader.GetToDecimal("InvoiceAmt"),
                        VendorName = reader.GetString("VendorName"),
                        StageName = reader.GetString("StageName")
                    }).ToList();
                }

            }
            return billReceiveList;
        }
    }
}
