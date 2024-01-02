using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class CBPSBillClearing : BaseModel
    {
        public string BillClearingID { get; set; }
        public string BillTrackingNo { get; set; }
        public string CompanyCode { get; set; }
        public string InvoiceDocNo { get; set; }
        public string FiscalYear { get; set; }
        public string ProcessState { get; set; }
        public string ReverseDocNo { get; set; }
        public string ReverseFiscalYear { get; set; }
        public int IsStop { get; set; }
        public int IsReceivedForClearing { get; set; }
        public int InvoiceDocType { get; set; }
        public string InvoiceDocDate { get; set; }
        public string PostingDate { get; set; }
        public string EnteredOn { get; set; }
        public string EnteredAt { get; set; }
        public string ChangedOn { get; set; }
        public string LastUpdate { get; set; }
        public string Reference { get; set; }
        public string InvoiceCurrency { get; set; }
        public string ReferenceKey { get; set; }
        public string ReleaseIndicator { get; set; }
        public string DeletionIndecator { get; set; }
        public string CompletionStatus { get; set; }
        public string DataPullingFlag { get; set; }
        public string DataSource { get; set; }
        public string ProcessGroupID { get; set; }
        public string InitiateDate { get; set; }
        public string ReleaseDate { get; set; }
        public string StageID { get; set; }

        #region Properties For View

        public string DisplayReleaseDate { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string InvoicingPartyCode { get; set; }
        public string InvoicingPartyName { get; set; }
        public string CompanyName { get; set; }
        public decimal AmtInLocalCurrency { get; set; }

        #endregion
    }
}
