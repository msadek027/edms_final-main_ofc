using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using SILDMS.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.WorkflowSetup
{
    public interface IWorkflowSetupService
    {
        void GetALLStagesForType(string docCategoryID, string ownerID, string docTypeID, out List<WFM_ProcessStageMap> obj);
        void GetDocumentWisePermission(string DocPropertyID, out List<int> obj);
        void GetChildStages(int StageMapID, out List<WFM_ProcessStageMap> obj);
        void SetStagesForType(IEnumerable<WFM_ProcessStageMap> prms, string userID, string OwnerID, string DocCategoryID, string DocTypeID, out string res_code, out string res_message);
        void GetALLPStageProperty(string docCategoryID, string ownerID, string docTypeID, int stageMapID, out WFM_VM_ParallelStageProperty tp);
        void SaveParallelStage(string userID, string docCategoryID, string ownerID, string docTypeID, int ProcessingStageId, int ParallelStageID, int EndStageMapID, out string res_code, out string res_message);
        void SaveProcessingStageConnections(List<WFM_ProcessingStageConnection> ProcessingStageConnections, string setBY, out string res_message);
        void SaveNodeCoordinate(List<WFM_ProcessStage> StagePositionList, string setBY, out string res_message);
        void GetProcessStagesConnectionsByWorkflow(int WorkflowID, out List<WFM_ProcessingStageConnection> ProcessingStageConnections, out string res_code);
        void GetALLStageProperty(string docCategoryID, string ownerID, string docTypeID, int stageMapID, out List<WFM_DocStageProperty> tp);
        void AddStageProperty(WFM_DocStageProperty tp, string action, out string res_code, out string res_message);

        ValidationResult GetAllDocuments(string DocPropertyId, string action, out List<DSM_DocProperty> ownerLevelList);
        ValidationResult AddNewDocument(DSM_DocProperty ownerLevel, List<WFM_ProcessStageMap> DocumentPermissionModal, string action, out string status);
       
    }
}
