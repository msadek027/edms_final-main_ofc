
using System.Collections.Generic;
using SILDMS.DataAccessInterface;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;

namespace SILDMS.Service.DocDestroyPolicy
{
    public class DocDestroyPolicyService : IDocDestroyPolicyService
    {
        private readonly IDocDestroyPolicyDataService _docDestroyPolicyDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber = string.Empty;


        public DocDestroyPolicyService(IDocDestroyPolicyDataService docDestroyPolicyDataService,
            ILocalizationService localizationService)
        {
            _docDestroyPolicyDataService = docDestroyPolicyDataService;
            _localizationService = localizationService;
        }



        public ValidationResult GetDestroyPolicyBySearchParam(string _DestroyPolicyID, string _UserID, string _OwnerID, string _DocCategoryID, string _DocTypeID,
            string _DocPropertyID, string _DocPropIdentityID, out List<DSM_DestroyPolicy> destroyPolicies)
        {
            destroyPolicies = _docDestroyPolicyDataService.GetDestroyPolicyBySearchParam
                (_DestroyPolicyID, _UserID, _OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID,
                    _DocPropIdentityID, out _errorNumber);

            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult GetDestroyPolicyDtlByID(string _DestroyPolicyID, out List<DSM_DestroyPolicy> destroyPolicies)
        {
            destroyPolicies = _docDestroyPolicyDataService.GetDestroyPolicyDtlByID(_DestroyPolicyID,out _errorNumber);

            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult SetDocDestroyPolicy(DSM_DestroyPolicy model, string action,
            out string status)
        {
            _docDestroyPolicyDataService.SetDocDestroyPolicy(model, action, out status);
            if (status.Length > 0)
            {
                return new ValidationResult(status, _localizationService.GetResource(status));
            }
            return ValidationResult.Success;
        }


        public ValidationResult GetDocUsingPolicy(string _DestroyPolicyID, string _UserID, out List<DocSearch> documents)
        {
            documents = _docDestroyPolicyDataService.GetDocByPolicy(_DestroyPolicyID, _UserID, out _errorNumber);
            
            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }


        public ValidationResult SaveImportantDocuments(string documents, string _DestroyPolicyID, string _UserID,out string status)
        {
            _docDestroyPolicyDataService.SaveImportantDocuments(documents, _DestroyPolicyID, _UserID, out status);

            if (status.Length > 0)
            {
                return new ValidationResult(status, _localizationService.GetResource(status));
            }

            return ValidationResult.Success;
        }


        public ValidationResult GetDeletableDoc(out List<DocSearch> documents)
        {
            documents = _docDestroyPolicyDataService.GetDeletableDoc(out _errorNumber);

            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }


        public ValidationResult DeleteDocByPolicy(string documents, out string status)
        {
            _docDestroyPolicyDataService.DeleteDocByPolicy(documents, out status);

            if (status.Length > 0)
            {
                return new ValidationResult(status, _localizationService.GetResource(status));
            }

            return ValidationResult.Success;
        }


        public ValidationResult DeleteDocuments(string Documents, string _DestroyPolicyID, string UserID, out string outStatus)
        {
            _docDestroyPolicyDataService.DeleteDocuments(Documents, _DestroyPolicyID, UserID, out outStatus);

            if (outStatus.Length > 0)
            {
                return new ValidationResult(outStatus, _localizationService.GetResource(outStatus));
            }

            return ValidationResult.Success;
        }
    }
}
