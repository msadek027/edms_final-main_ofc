using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_FurtherDemand : BaseModel
    {
        public string DemandID { get; set; }
        public string PONo { get; set; }
        public string BillTrackingNo { get; set; }
        public int? DemandTo { get; set; }
        public string MailSmsSendID { get; set; }
        public string DeletionIndicator { get; set; }
        public string CompletionStatus { get; set; }
        public string ProcessState { get; set; }
        public string ProcessGroupID { get; set; }
        public string StageID { get; set; }
    }
}
