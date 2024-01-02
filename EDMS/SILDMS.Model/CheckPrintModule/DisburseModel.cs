using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CheckPrintModule
{
    public class DisburseModel
    {
        public List<CPS_PrintAccountDocAction> DocActions { get; set; }
        public List<CPS_PrintDocDisburse> DocDisburses { get; set; }
        public List<CPS_PrintDocDisburseMeta> DocDisburseMetas { get; set; }
 
    }
}
