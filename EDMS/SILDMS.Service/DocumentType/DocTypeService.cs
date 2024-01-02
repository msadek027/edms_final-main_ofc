using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.DataAccessInterface.DocumentType;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;

namespace SILDMS.Service.DocumentType
{
    public class DocTypeService : IDocTypeService
    {
        #region Fields

        private readonly IDocTypeDataService _docTypeDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber = string.Empty;

        #endregion

        #region Constructor

        public DocTypeService(IDocTypeDataService docTypeDataService, ILocalizationService localizationService)
        {
            _docTypeDataService = docTypeDataService;
            _localizationService = localizationService;
        }

        #endregion
        public ValidationResult GetDocTypes(string id, string action, out List<DSM_DocType> docTypeList)
        {
            docTypeList = _docTypeDataService.GetDocType(id, action, out _errorNumber);
            return _errorNumber.Length > 0 ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber)) : ValidationResult.Success;
        }

        public ValidationResult ManipulateDocType(DSM_DocType docType, string action, out string status)
        {
            _docTypeDataService.ManipulateDocType(docType, action, out status);
            return status.Length > 0 ? new ValidationResult(status, _localizationService.GetResource(status)) : ValidationResult.Success;
        }
        public ValidationResult ManipulateDocTypeWorkflow(DSM_DocType docType, string action, out string status,out string ID)
        {
            _docTypeDataService.ManipulateDocTypeWorkflow(docType, action, out status,out ID);
            return status.Length > 0 ? new ValidationResult(status, _localizationService.GetResource(status)) : ValidationResult.Success;
        }
    }
}
