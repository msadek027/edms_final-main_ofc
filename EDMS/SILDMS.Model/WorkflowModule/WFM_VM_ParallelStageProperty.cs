using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.WorkflowModule
{
    public class WFM_VM_ParallelStageProperty
    {
        public List<WFM_ProcessStageMap> ParallelStages { get; set; }
        public List<WFM_ProcessStageMap> ParallelEndStages { get; set; }
        public bool IsConfigured { get; set; }
        public int CurrentPStage { get; set; }
        public int CurrentPEndStage { get; set; }
    }
}
