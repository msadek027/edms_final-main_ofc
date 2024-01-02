using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_BillReceive : BaseModel
    {
        public string BillReceiveID { get; set; }
        public string BoothID { get; set; }
        public string PONo { get; set; }
        public string BillTrackingNo { get; set; }
        public string InvoicingParty { get; set; }
        public int IsVendorPartySame { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public decimal InvoiceAmt { get; set; }
        public string InvoiceCurrency { get; set; }
        public string PreferedPayMode { get; set; }
        public string BillSubmittedBy { get; set; }
        public string BillSubmitDate { get; set; }
        public string BearerContactNo { get; set; }
        public string ReleaseIndicator { get; set; }
        public string DeletionIndecator { get; set; }
        public string CompletionStatus { get; set; }
        public string ProcessState { get; set; }
        public string ProcessGroupID { get; set; }
        public string StageID { get; set; }
        public string CurrentStage { get; set; }
        public int IsStop { get; set; }

        #region View Properties

        public string VendorID { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string VendorMobileNo { get; set; }
        public string ProcessGroupName { get; set; }
        public string StageName { get; set; }
        public string InvoicingPartyName { get; set; }
        public string OwnerName { get; set; }

        #endregion
    }
}
