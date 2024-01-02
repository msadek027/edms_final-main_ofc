using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.Service.ProcessStageSetup
{
    public interface IProcessStageService
    {
        //void AddProcessStage(WFM_ProcessStage stage, out string res_code, out string res_message);
        void UpdateProcessStage(WFM_ProcessStage stage, out string res_code, out string res_message);
        void GetALLProcessStage(string userID, out List<WFM_ProcessStage> stages);
        ValidationResult GetProcessStagesByWorkflow(string DocTypeID, string userID, out List<WFM_ProcessStage> stages, out WFM_Workflow workflow);
        ValidationResult AddProcessStages(List<WFM_ProcessStage> stages, string userID, out string res_message);
        void GetProcessStagesByStageID(int StageID, out WFM_ProcessStage stage, out string res_message);

        void GetChildProcessStagesByStageID(int StageID, out List<WFM_ProcessStage> ChildStages, out string res_message);
    }
}
