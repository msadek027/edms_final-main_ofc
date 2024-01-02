using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.SecurityModule
{
    public class SEC_ConfigureColumn : SEC_ConfigurePage
    {
        public string ConfigureColumnID { get; set; }
        public string ConfigureCategory { get; set; }
        public string ConfigurationFor { get; set; }
        public string ConfigureToTable { get; set; }
        public string ConfigureToColumn { get; set; }
        public string AutoColumnTitle { get; set; }
        public string AutoValueGroupID { get; set; }
        public string AutoValueGroup { get; set; }
        public string CssClass { get; set; }
        public string PrimaryKeyColumn { get; set; }
        
    }
}
