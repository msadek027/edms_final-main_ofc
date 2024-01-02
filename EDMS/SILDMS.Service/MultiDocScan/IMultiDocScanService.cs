using System.Collections.Generic;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;

namespace SILDMS.Service.MultiDocScan
{
    public interface IMultiDocScanService
    {
        ValidationResult AddDocumentInfo(DocumentsInfo _modelDocumentsInfo,
            List<DSM_VM_Property> _selectedPropID, List<DocMetaValue> _docMetaValues, bool _otherUpload, string _extentions,bool IsSecured, string action,
            out List<DSM_DocPropIdentify> docPropIdentifyList);

        ValidationResult UpdateDocumentInfo(DocumentsInfo _modelDocumentsInfo,
           string action, out List<DSM_DocPropIdentify> docPropIdentifyList);

        ValidationResult GetAllDocumentsInfo(string _UserID, string _DocumentID, 
            out List<DSM_Documents> dsmDocuments);

        ValidationResult GetAllDocumentsMetaInfo(string _UserID, string _DocMetaID,
            out List<DSM_DocumentsMeta> dsmDocumentsMetas);

        ValidationResult DeleteDocumentInfo(string _DocumentIDs);
        void GetDocPropIdentityForSelectedDocTypes(string userID, string _OwnerID, string _DocCategoryID, string _SelectedPropID, out List<DSM_DocPropIdentify> objDocPropIdentifies);

        ValidationResult IsRevesionNoValid(string UserID, string _OwnerID, string _DocCatID, string _DocTypID, string _DocPropertyID, string _DocPropIdentifyID, string _MetaValue, string _revesionNo);
    }
}
