using System;


namespace SILDMS.Model.WorkflowModule
{
    public class WFM_ProcessingStageConnection
    {
        public int StageConnectionID { get; set; }
        public int SourceStageId { get; set; }
        public int TargetStageId { get; set; }
       
        public string DocTypeID { get; set; }
        public int WorkflowID { get; set; }
 
        public DateTime SetOn { get; set; }
        public string SetBy { get; set; }
       
    }
}
