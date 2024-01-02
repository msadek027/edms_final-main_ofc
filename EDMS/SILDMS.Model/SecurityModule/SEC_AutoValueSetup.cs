using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.Model.SecurityModule
{
    public class SEC_AutoValueSetup
    {
        public string AutoValueSetupID { get; set; }
        public string ConfigureID { get; set; }

        public ddlDSMOwnerLevel OwnerLevel { get; set; }
        public ddlDSMOwner Owner { get; set; }
        public ddlDSMDocCategory DocCategory { get; set; }
        public ddlDSMDocType DocType { get; set; }
        public ddlDSMDocProperty DocProperty { get; set; }
        public ddlDSMDocPropIdentify DocPropIdentify { get; set; }

        public string AutoValueID { get; set; }
        public string ConfigureColumnID { get; set; }
        public string AutoColumnTitle { get; set; }


        public string AutoValueFormat { get; set; }
        public string IncrementValueLength { get; set; }
        public string IncrementReplaceBy { get; set; }
        public string IncrementRestartBy { get; set; }
        public string MaxValue { get; set; }
        public string UserLevel { get; set; }



        
        public string Remarks { get; set; }
        public string SetOn { get; set; }
        public string SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Status { get; set; }
    }
}
