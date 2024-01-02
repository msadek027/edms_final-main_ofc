using System.Collections.Generic;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccessInterface
{
    public interface IDocDestroyPolicyDataService
    {
        List<DSM_DestroyPolicy> GetDestroyPolicyBySearchParam(string _DestroyPolicyID, string _UserID, string _OwnerID,
            string _DocCategoryID, string _DocTypeID, string _DocPropertyID, 
            string _DocPropIdentityID, out string _errorNumber);

        List<DSM_DestroyPolicy> GetDestroyPolicyDtlByID(string _DestroyPolicyID,out string _errorNumber);

        string SetDocDestroyPolicy(DSM_DestroyPolicy model, string action, out string errorNumber);

        List<DocSearch> GetDocByPolicy(string _DestroyPolicyID, string _UserID, out string _errorNumber);

        string SaveImportantDocuments(string documents, string _DestroyPolicyID, string _UserID, out string errorNumber);
        List<DocSearch> GetDeletableDoc(out string _errorNumber);

        string DeleteDocByPolicy(string documents, out string status);
        string DeleteDocuments(string documents, string _DestroyPolicyID, string _UserID, out string status);
    }
}
