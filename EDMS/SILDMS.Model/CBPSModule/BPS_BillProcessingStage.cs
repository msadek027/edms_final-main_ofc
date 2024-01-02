using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_BillProcessingStage : BaseModel
    {
        public string StageID { get; set; }
        public int StageNo { get; set; }
        public string StageName { get; set; }
        public string ProcessGroupID { get; set; }
        public int HardCopyMove { get; set; }
        public string InitiateIndicator { get; set; }
        public string ReleaseIndicator { get; set; }
        public string NextStage { get; set; }
        public string StageBackKey { get; set; }
        public string StageSuspendKey { get; set; }
        public string PaymentBlockKey { get; set; }
        public string DataPullIndicator { get; set; }
        public string DataSealIndicator { get; set; }
        public int AutoIssue { get; set; }
        public int AutoReceive { get; set; }
        public int VendorNotify { get; set; }
        public int InvoicePartyNotify { get; set; }
        public int BenchMark { get; set; }
    }
}
