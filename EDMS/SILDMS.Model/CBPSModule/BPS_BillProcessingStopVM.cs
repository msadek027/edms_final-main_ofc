using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model
{
    public class BPS_BillProcessingStopVM
    {
        public string StopBillID { get; set; }
        public string PONo { get; set; }
        public string BillTrackingNo { get; set; }
        public string InvoiceNo { get; set; }
        public string VendorUpdate { get; set; }
        public string MailSmsSendID { get; set; }
        public string ReleaseIndicator { get; set; }
        public string DeletionIndicator { get; set; }
        public string CompletionStatus { get; set; }
        public string StageID { get; set; }
        public string StageName { get; set; }
        public string VendorName { get; set; }
        public string InvoicingPartyName { get; set; }
        public string StopBillDtlID { get; set; }
        public string StopDate { get; set; }
        public string StopReason { get; set; }
        public string ReopenDate { get; set; }
        public string ReopenReason { get; set; }
        public string ProcessState { get; set; }
        public string ProcessGroupID { get; set; }
        public string ProcessGroupName { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string DocCategoryID { get; set; }
        public string DocTypeID { get; set; }
        public string DocPropertyID { get; set; }
        public int UserLevel { get; set; }
        public string SetOn { get; set; }
        public string SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Status { get; set; }
        public string CurrentStage { get; set; }
        public int IsStop { get; set; }
        public string VendorID { get; set; }
        public string VendorCode { get; set; }
        public int BrStatus { get; set; }

    }
}
