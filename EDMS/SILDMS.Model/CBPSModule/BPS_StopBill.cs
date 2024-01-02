using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_StopBill : BaseModel
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
    }
}
