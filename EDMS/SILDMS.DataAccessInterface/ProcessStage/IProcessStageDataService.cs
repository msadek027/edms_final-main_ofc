using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.DataAccessInterface.ProcessStage
{
    public interface IProcessStageDataService
    {
        List<WFM_ProcessStage> GetALLProcessStage(string userID);
        List<WFM_ProcessStage> GetProcessStagesByWorkflow(string DocTypeID, string userID, out string _outStatus);
         WFM_Workflow GetWorkflow(string DocTypeID, out string _outStatus);
        void UpdateProcessStage(WFM_ProcessStage stage,  out string _res_code, out string _res_message);
        //void AddProcessStage(WFM_ProcessStage stage, out string res_code, out string res_message);
        WFM_ProcessStage GetProcessStagesByStageID( int StageID, out string res_message);
        void AddProcessStages(List<WFM_ProcessStage> stages, string userID, out string res_message);

        List<WFM_ProcessStage> GetChildProcessStagesByStageID(int StageID, out string res_message);
    }
}
