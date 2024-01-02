using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_MailSms : BaseModel
    {
        public string MailSmsID { get; set; }
        public string BillTrackingNo { get; set; }
        public string VendorID { get; set; }
        public string PONo { get; set; }
        public string ReleaseIndicator { get; set; }
        public string DeletionIndicator { get; set; }
        public string CompletionStatus { get; set; }

    }
}
