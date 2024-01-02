using System.Collections.Generic;
using SILDMS.DataAccessInterface.MultiDocScan;
using SILDMS.DataAccessInterface.RoleSetup;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.SecurityModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;

namespace SILDMS.Service.DocSharing
{
         public class DocSharingService: IDocSharingService
    {
        private readonly IMultiDocScanDataService _multiDocScanDataService;
        private readonly ILocalizationService _localizationService;
        public DocSharingService(IMultiDocScanDataService multiDocScanDataService, ILocalizationService localizationService)
        {
            _multiDocScanDataService = multiDocScanDataService;
           
            _localizationService = localizationService; 


        }
        public ValidationResult GetDocumentByDocumentId(string DocumentID, out DSM_Documents document)
        {
            document = _multiDocScanDataService.GetDocumentByDocumentId(DocumentID);
            if (document != null)
            {
                return new ValidationResult("No Data Found", _localizationService.GetResource("No Data Found"));
            }
            return ValidationResult.Success;
        }
    }
}
