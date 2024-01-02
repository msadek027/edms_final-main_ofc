using SILDMS.DataAccessInterface.ProcessStage;
using SILDMS.Utillity.Localization;
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

    public class ProcessStageService : IProcessStageService
    {
        #region Fields

        private readonly IProcessStageDataService _processStageDataService;
        private readonly ILocalizationService localizationService;
        private string _errorNumber = "";
        #endregion

        #region Constractor
        public ProcessStageService(
             IProcessStageDataService repository,
             ILocalizationService localizationService
            )
        {
            this._processStageDataService = repository;
            this.localizationService = localizationService;
        }

        public void UpdateProcessStage(WFM_ProcessStage stage, out string res_code, out string res_message)
        {
            _processStageDataService.UpdateProcessStage(stage,out res_code,out res_message);

        }
        public void GetProcessStagesByStageID(int StageID, out WFM_ProcessStage stage, out string res_message)
        {
            stage=_processStageDataService.GetProcessStagesByStageID(StageID, out res_message);

        }
        public void GetChildProcessStagesByStageID(int StageID, out List<WFM_ProcessStage> ChildStages, out string res_message)
        {
            ChildStages = _processStageDataService.GetChildProcessStagesByStageID(StageID, out res_message);

        }
        public ValidationResult AddProcessStages(List<WFM_ProcessStage> stages, string userID, out string res_message)
        {
            _processStageDataService.AddProcessStages(stages, userID, out res_message);
            return res_message.Contains("S") ? new ValidationResult(res_message, localizationService.GetResource(res_message)) : ValidationResult.Success;
        }

        public void GetALLProcessStage(string userID, out List<WFM_ProcessStage> stages)
        {
            stages =_processStageDataService.GetALLProcessStage(userID);
        }
        public ValidationResult GetProcessStagesByWorkflow(string DocTypeID, string UserID, out List<WFM_ProcessStage> stages,out WFM_Workflow workflow)
        {
            
            string _outStatus = string.Empty;
            
            stages = _processStageDataService.GetProcessStagesByWorkflow(DocTypeID,UserID, out _outStatus );
            workflow = new WFM_Workflow();
            if (!_outStatus.Contains("S201"))
            {
                workflow= _processStageDataService.GetWorkflow(DocTypeID, out _outStatus);

            }
           
            return _outStatus.Contains("S") ? new ValidationResult(_outStatus, localizationService.GetResource(_outStatus)) : ValidationResult.Success;
        }
        #endregion


    }
}
