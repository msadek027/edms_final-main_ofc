using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_StopBillDtl : BPS_StopBill
    {
        public string StopBillDtlID { get; set; }
        public string StopDate { get; set; }
        public string StopReason { get; set; }
        public string ReopenDate { get; set; }
        public string ReopenReason { get; set; }
        public string ProcessState { get; set; }
        public string ProcessGroupID { get; set; }
        
    }
}
