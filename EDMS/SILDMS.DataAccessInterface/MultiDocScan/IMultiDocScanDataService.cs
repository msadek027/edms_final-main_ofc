using System.Collections.Generic;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccessInterface.MultiDocScan
{
    public interface IMultiDocScanDataService
    {
        List<DSM_DocPropIdentify> AddDocumentInfo(DocumentsInfo _modelDocumentsInfo,
            List<DSM_VM_Property> _selectedPropID, List<DocMetaValue> _docMetaValues, bool _otherUpload, string _extentions,bool IsSecured,
            string _action, out string _errorNumber);

        List<DSM_DocPropIdentify> UpdateDocumentInfo(DocumentsInfo _modelDocumentsInfo,
            string _action, out string _errorNumber);

        List<DSM_Documents> GetAllDocumentsInfo(string _UserID, string _DocumentID, 
            out string errorNumber);
        List<DSM_DocumentsMeta> GetAllDocumentsMetaInfo(string _UserID, string _DocMetaID,
            out string errorNumber);

        string DeleteDocumentInfo(string _DocumentIDs, out string _errorNumber);
        List<DSM_DocPropIdentify> GetDocPropIdentityForSelectedDocTypes(string userID, string _OwnerID, string _DocCategoryID, string _SelectedPropID);

        string IsRevesionNoValid(string UserID, string _OwnerID, string _DocCatID, string _DocTypID, string _DocPropertyID, string _DocPropIdentifyID, string _MetaValue, string _revesionNo, out string _errorNumber);
        DSM_Documents GetDocumentByDocumentId(string DocumentId);
    }
}
