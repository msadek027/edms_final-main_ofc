using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_MailSmsSend : BaseModel
    {
        public string MailSmsSendID { get; set; }
        public string MailSmsID { get; set; }
        public string MailAddress { get; set; }
        public string MailCC { get; set; }
        public string SmsMobileNo { get; set; }
        public string SmsCC { get; set; }
        public string MailText { get; set; }
        public string SmsText { get; set; }
        public string MailSendStatus { get; set; }
        public string SmsSendStatus { get; set; }
        public string MailSendFrom { get; set; }
        public string SmsSendFrom { get; set; }
        public string MailSmsReason { get; set; }
        public string SendDate { get; set; }
        public string ReferenceNo { get; set; }
        public string Note { get; set; }
        public string DeletionIndicator { get; set; }
        public string CompletionStatus { get; set; }
        public string ProcessState { get; set; }
        public string ProcessGroupID { get; set; }
        public string StageID { get; set; }
        
    }
}
