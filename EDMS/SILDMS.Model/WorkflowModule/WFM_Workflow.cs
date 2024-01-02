using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.WorkflowModule
{
    public class WFM_Workflow
    {
        public int WorkflowID { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowDescription { get; set; }
        public int NumberOfStage { get; set; }
        public string DocTypeID { get; set; }

        public string SetOn { get; set; }
        public string SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Status { get; set; }

    }
}
