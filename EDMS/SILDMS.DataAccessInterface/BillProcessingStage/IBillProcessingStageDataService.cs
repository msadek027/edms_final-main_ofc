using System.Collections.Generic;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccessInterface.BillProcessingStage
{
    public interface IBillProcessingStageDataService
    {
        List<DSM_Owner> GetOwnerByCompany(string id, string action, out string errorNumber);
        List<BPS_BillProcessGroup> GetBillProcessGroups(string id, string action, out string errorNumber);

        List<BPS_BillProcessingStage> GetBillProcessingStages(string ownerId, string stageId, string groupId, string action,
            out string errorNumber);

        List<BPS_StageMakerChecker> GetStageMakerCheckers(string stageId, string action, out string errorNumber);
        List<DSM_Owner> GetOwnersForRestrictedCompany(string id, string action, out string errorNumber);
        List<BPS_RestrictedCompany> GetRestrictedCompany(string id, string action, out string errorNumber);
        string ManipulateBillProcessingStage(BPS_BillProcessingStage stage, string action, out string errorNumber);
        string ManipulateRestrictedCompany(BPS_RestrictedCompany company, string action, out string errorNumber);
        string ManipulateStageMakerChecker(BPS_StageMakerChecker stMkCk, string action, out string errorNumber);
    }
}