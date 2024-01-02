using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.DocScanningModule
{
    public class EmailDataViewModel
    {
        public string DestroyPolicyID { get; set; }
        public string PolicyFor { get; set; }
        public string DocumentNature { get; set; }
        public string OwnerLavelID { get; set; }
        public string OwnerID { get; set; }
        public string DocCategoryID { get; set; }
        public string DocTypeID { get; set; }
        public string DocPropertyID { get; set; }
        public string DocPropIdentifyID { get; set; }
        public string MetaValue { get; set; }
        public int TimeValue { get; set; }
        public int TimeValueCon { get; set; }
        public string TimeUnit { get; set; }
        public string Emails { get; set; }
    }
}