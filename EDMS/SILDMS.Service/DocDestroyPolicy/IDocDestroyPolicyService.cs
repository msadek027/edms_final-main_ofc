

using System.Collections.Generic;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;

namespace SILDMS.Service.DocDestroyPolicy
{
    public interface IDocDestroyPolicyService
    {
        ValidationResult GetDestroyPolicyBySearchParam(string _DestroyPolicyID, string _UserID, string _OwnerID,
            string _DocCategoryID, string _DocTypeID, string _DocPropertyID, 
            string _DocPropIdentityID, out List<DSM_DestroyPolicy> destroyPolicies);

        ValidationResult GetDestroyPolicyDtlByID(string _DestroyPolicyID, out List<DSM_DestroyPolicy> destroyPolicies);

        ValidationResult SetDocDestroyPolicy(DSM_DestroyPolicy model, string action,
            out string status);

        ValidationResult GetDocUsingPolicy(string _DestroyPolicyID, string _UserID, out List<DocSearch> documents);

        ValidationResult SaveImportantDocuments(string documents, string _DestroyPolicyID,string _UserID, out string status);

        ValidationResult GetDeletableDoc(out List<DocSearch> documents);

        ValidationResult DeleteDocByPolicy(string documents,out string status);

        ValidationResult DeleteDocuments(string Documents, string _DestroyPolicyID, string UserID, out string outStatus);
    }
}
