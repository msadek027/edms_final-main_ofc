using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.SecurityModule
{
    public class SEC_UserLog
    {
        #region Constructor

        public SEC_UserLog() { }

        #endregion

        #region Fields

        public string LogID { get; set; }
        public string UserID { get; set; }
        public string UserFullName { get; set; }       
        public string UsedIP { get; set; }
        public string ActionUrl { get; set; }
        public string UserAction { get; set; }
        public string ActionEventTime { get; set; }
        public string GenericID { get; set; }
        public int Status { get; set; }
        public string ActionExecuteTime { get; set; }

        public string LoggID { get; set; }
        public string LoggAction { get; set; }
        public string LoggResult { get; set; }
        public string LookupTable { get; set; }
        public string DocumentID { get; set; }
        public string MetaValue { get; set; }
        public string TotalCount { get; set; }
        public string IdentificationAttribute { get; set; }
        public string Remarks { get; set; }
        public string DocumentStatus { get; set; } 
        public string ObsoleteTime { get; set; }
        #endregion

    }
}
