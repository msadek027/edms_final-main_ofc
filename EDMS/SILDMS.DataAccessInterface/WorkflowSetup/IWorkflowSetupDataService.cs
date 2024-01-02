using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccessInterface.WorkflowSetup
{
    public interface IWorkflowSetupDataService
    {
        List<WFM_ProcessStageMap> GetALLStagesForType(string docCategoryID, string ownerID, string docTypeID);
        List<int> GetDocumentWisePermission(string DocPropertyID);
        List<WFM_ProcessStageMap> GetChildStages(int StageMapID);
        WFM_VM_ParallelStageProperty GetALLPStageProperty(string docCategoryID, string ownerID, string docTypeID, int stageMapID);

        void SetStagesForType(IEnumerable<WFM_ProcessStageMap> prms, string userID, string ownerID, string docCategoryID, string docTypeID, out string res_code, out string res_message);
        void SaveParallelStage(string userID, string docCategoryID, string ownerID, string docTypeID, int ProcessingStageId, int ParallelStageID, int EndStageMapID, out string res_code, out string res_message);
        void SaveProcessingStageConnections(List<WFM_ProcessingStageConnection> ProcessingStageConnections, string setBY, out string res_message);
        void SaveNodeCoordinate(List<WFM_ProcessStage> StagePositionList, string setBY, out string _res_message);
        List<WFM_ProcessingStageConnection> GetProcessStagesConnectionsByWorkflow(int WorkflowID, out string _res_message);
        List<WFM_DocStageProperty> GetALLStageProperty(string docCategoryID, string ownerID, string docTypeID, int stageMapID);
        void AddStageProperty(WFM_DocStageProperty tp, string action, out string res_code, out string res_message);
        List<DSM_DocProperty> GetAllDocuments(string DocPropertyId, string action, out string errorNumber);
        string AddNewDocument(DSM_DocProperty ownerLevel, List<WFM_ProcessStageMap> DocumentPermissionModal,string action, out string errorNumber);

    }
}
