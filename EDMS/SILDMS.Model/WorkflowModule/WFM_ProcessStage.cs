using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.WorkflowModule
{
    public class WFM_ProcessStage
    {
        public int StageID { get; set; }
        
        public string StageName { get; set; }
        public string DocTypeID { get; set; }
        public int WorkflowID { get; set; }
      
        public string ProcessingGroupID { get; set; }
        public int StageSuspendKey { get; set; }
        public int StageSL { get; set; }
        public string StagePosition { get; set; }

   
        public bool HaveMk { get; set; }
        public bool HaveCk { get; set; }
        public bool NotifyMk { get; set; }
        public bool NotifyCk { get; set; }
        public DateTime SetOn { get; set; }
        public string SetBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int NumberOFStage { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int  Status { get; set; }
    }
}
