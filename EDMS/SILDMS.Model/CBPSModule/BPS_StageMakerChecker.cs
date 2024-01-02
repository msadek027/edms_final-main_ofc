using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_StageMakerChecker : BaseModel
    {
        public string StageMCID { get; set; }
        public string StageID { get; set; }
        public string MkCkTitle { get; set; }
        public string DefaultValue { get; set; }
    }
}
