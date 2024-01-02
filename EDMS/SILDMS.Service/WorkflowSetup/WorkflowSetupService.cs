using SILDMS.DataAccessInterface.WorkflowSetup;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.WorkflowSetup
{
    public class WorkflowSetupService : IWorkflowSetupService
    {
        private readonly IWorkflowSetupDataService workflowSetupDataService;
        private readonly ILocalizationService localizationService;
        private string errorNumber = "";

        public WorkflowSetupService(IWorkflowSetupDataService repository, ILocalizationService _localizationService)
        {
            this.workflowSetupDataService = repository;
            this.localizationService = _localizationService;
        }
        public void GetALLStagesForType(string docCategoryID, string ownerID, string docTypeID, out List<WFM_ProcessStageMap> obj)
        {
            obj = workflowSetupDataService.GetALLStagesForType(docCategoryID, ownerID, docTypeID);
        }
        public void GetDocumentWisePermission(string DocPropertyID, out List<int> obj)
        {
            obj = workflowSetupDataService.GetDocumentWisePermission(DocPropertyID);
        }
        public void GetChildStages(int StageMapID, out List<WFM_ProcessStageMap> obj)
        {
            obj = workflowSetupDataService.GetChildStages(StageMapID);
        }
        public void SetStagesForType(IEnumerable<WFM_ProcessStageMap> prms, string userID, string ownerID, string docCategoryID, string docTypeID, out string res_code, out string res_message)
        {
            workflowSetupDataService.SetStagesForType(prms, userID, ownerID, docCategoryID, docTypeID, out res_code, out res_message);
        }

        public void GetALLPStageProperty(string docCategoryID, string ownerID, string docTypeID, int stageMapID, out WFM_VM_ParallelStageProperty tp)
        {
            tp = workflowSetupDataService.GetALLPStageProperty(docCategoryID, ownerID, docTypeID, stageMapID);
        }

        public void SaveParallelStage(string userID, string docCategoryID, string ownerID, string docTypeID, int ProcessingStageId, int ParallelStageID, int EndStageMapID, out string res_code, out string res_message)
        {
            workflowSetupDataService.SaveParallelStage(userID, docCategoryID, ownerID, docTypeID, ProcessingStageId, ParallelStageID, EndStageMapID, out res_code, out res_message);
        }

        public void SaveProcessingStageConnections(List<WFM_ProcessingStageConnection> ProcessingStageConnections, string setBY, out string res_message)

        {
            workflowSetupDataService.SaveProcessingStageConnections(ProcessingStageConnections, setBY,out res_message);
        }
        public void SaveNodeCoordinate(List<WFM_ProcessStage> StagePositionList, string setBY, out string res_message)

        {
            workflowSetupDataService.SaveNodeCoordinate(StagePositionList, setBY, out res_message);
        }
        public void GetALLStageProperty(string docCategoryID, string ownerID, string docTypeID, int stageMapID, out List<WFM_DocStageProperty> tp)
        {
            tp = workflowSetupDataService.GetALLStageProperty(docCategoryID, ownerID, docTypeID, stageMapID);
        }

        public void GetProcessStagesConnectionsByWorkflow(int WorkflowID, out List<WFM_ProcessingStageConnection> ProcessingStageConnections, out string res_code)
        {
            ProcessingStageConnections=workflowSetupDataService.GetProcessStagesConnectionsByWorkflow( WorkflowID, out  res_code);
        }


        public void AddStageProperty(WFM_DocStageProperty tp, string action, out string res_code, out string res_message)
        {
            workflowSetupDataService.AddStageProperty(tp, action, out res_code, out res_message);
        }

        public ValidationResult GetAllDocuments(string DocPropertyId, string action, out List<DSM_DocProperty> docPropertyList)
        {
            docPropertyList = workflowSetupDataService.GetAllDocuments(DocPropertyId, action, out errorNumber);
            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult AddNewDocument(DSM_DocProperty docProperty, List<WFM_ProcessStageMap> DocumentPermissionModal, string action, out string status)
        {
            workflowSetupDataService.AddNewDocument(docProperty, DocumentPermissionModal,action, out status);
            if (status.Length > 0)
            {
                return new ValidationResult(status, localizationService.GetResource(status));
            }

            return ValidationResult.Success;
        }
      
    }
}
