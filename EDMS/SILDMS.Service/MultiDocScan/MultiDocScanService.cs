using System;
using System.Collections.Generic;
using SILDMS.DataAccessInterface.MultiDocScan;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;

namespace SILDMS.Service.MultiDocScan
{
    public class MultiDocScanService: IMultiDocScanService
    {
        private readonly IMultiDocScanDataService _multiDocScanDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber = string.Empty;
       

        public MultiDocScanService(IMultiDocScanDataService multiDocScanDataService,
            ILocalizationService localizationService)
        {
            _multiDocScanDataService = multiDocScanDataService;
            _localizationService = localizationService;
        }
        public ValidationResult AddDocumentInfo(DocumentsInfo _modelDocumentsInfo,
            List<DSM_VM_Property> _selectedPropID, List<DocMetaValue> _docMetaValues, bool _otherUpload, string _extentions,bool IsSecured, string action,
            out List<DSM_DocPropIdentify> docPropIdentifyList)
        {
            docPropIdentifyList = _multiDocScanDataService.AddDocumentInfo
                (_modelDocumentsInfo, _selectedPropID, _docMetaValues, _otherUpload, _extentions, IsSecured, action, out _errorNumber);

            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, 
                    _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult UpdateDocumentInfo(DocumentsInfo _modelDocumentsInfo,
            string action, out List<DSM_DocPropIdentify> docPropIdentifyList)
        {
            docPropIdentifyList = _multiDocScanDataService.UpdateDocumentInfo
                (_modelDocumentsInfo, action, out _errorNumber);

            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber,
                    _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult GetAllDocumentsInfo(string _UserID, string _DocumentID, 
            out List<DSM_Documents> dsmDocuments)
        {
            dsmDocuments = _multiDocScanDataService.
                GetAllDocumentsInfo(_UserID, _DocumentID, out _errorNumber);

            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber,
                    _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult GetAllDocumentsMetaInfo(string _UserID, string _DocMetaID, 
            out List<DSM_DocumentsMeta> dsmDocumentsMetas)
        {
            dsmDocumentsMetas = _multiDocScanDataService.
                GetAllDocumentsMetaInfo(_UserID, _DocMetaID, out _errorNumber);

            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, 
                    _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult DeleteDocumentInfo(string _DocumentIDs)
        {
            _multiDocScanDataService.DeleteDocumentInfo(_DocumentIDs, out _errorNumber);

            if (_errorNumber == "S201")
            {
                return ValidationResult.Success;
            }
            else
            {
                return ValidationResult.Success;
            }

        }

        public void GetDocPropIdentityForSelectedDocTypes(string userID, string _OwnerID, string _DocCategoryID, string _SelectedPropID, out List<DSM_DocPropIdentify> objDocPropIdentifies)
        {
            objDocPropIdentifies = _multiDocScanDataService.GetDocPropIdentityForSelectedDocTypes(userID, _OwnerID, _DocCategoryID,_SelectedPropID);
        }


        public ValidationResult IsRevesionNoValid(string UserID, string _OwnerID, string _DocCatID, string _DocTypID, string _DocPropertyID, string _DocPropIdentifyID, string _MetaValue, string _revesionNo)
        {
             _multiDocScanDataService.
                IsRevesionNoValid(UserID, _OwnerID, _DocCatID, _DocTypID, _DocPropertyID, _DocPropIdentifyID, _MetaValue, _revesionNo, out _errorNumber);

             return new ValidationResult(_errorNumber);
        }
    }
}
