using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.WorkflowModule
{
    public class WFM_ProcessStageMap : WFM_ProcessStage
    {
        public string StageMapID { get; set; }
        public string OwnerLevelID { get; set; }
        public string OwnerID { get; set; }
        public string DocCategoryID { get; set; }
        public string DocTypeID { get; set; }
        public string OwnerLevelName { get; set; }
        public string OwnerName { get; set; }
        
        public string DocCategoryName { get; set; }
        public string DocTypeName { get; set; }
        public bool IsChecked { get; set; }
    }
}
