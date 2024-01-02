using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SILDMS.Web.UI.Models
{
    public class Notification
    {
        public int StageMapID { get; set; }
        public string ObjectID { get; set; }
        public string Message { get; set; }
        public bool IsNew { get; set; }
        public DateTime NotifyAt { get; set; }
    }
}