using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SILDMS.Web.UI.Models
{
    public class EmailNotificationJob4DUModel
    {
        public string OwnerID { get; set; }
        public string DocCategoryName { get; set; }
        public string DocTypeName { get; set; }
        public string DocPropertyName { get; set; }
        public string IdentificationAttribute { get; set; }
        public string MetaValue { get; set; }
        public string Emails { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string NotificationDate { get; set; }
        public string DocumentExpDate { get; set; }
        public string DocumentID { get; set; }
        public string MessageLogID { get; set; }
  
    }
}