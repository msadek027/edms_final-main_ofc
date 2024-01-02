using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.DocScanningModule
{
    public class DSM_VM_Property
    {
        public string DocPropertyID { get; set; }
        public bool HasSMS { get; set; }
        public bool HasEmail { get; set; }
        public bool IsObsoletable { get; set; }
        public DateTime? NotifyDate { get; set; }
        public DateTime? ExpDate { get; set; }
    }
}
