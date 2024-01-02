using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SILDMS.Web.UI.Models
{
    public class ReportModel
    {
        public string OwnerLevelID { get; set; }
        public string OwnerID { get; set; }
        public string RoleID { get; set; }
        public string ReportType { get; set; }
        public string DocCategory { get; set; }
        public string DocType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}