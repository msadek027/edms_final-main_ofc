using System.Collections.Generic;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Web.UI.Models;

namespace SILDMS.Service.OriginalDocSearching
{
    public interface IOriginalDocSearchingService
    {
        ValidationResult UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo, string _UserID, out string outStatus);

        ValidationResult DeleteDocument(string _DocumentID, string _DocDistributionID, string _DocumentType, string _UserID, out string outStatus);

        ValidationResult GetOriginalDocBySearchParam(string _OwnerID, string _DocCategoryID,
            string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<DocSearch> userList);

       

        ValidationResult GetOriginalDocBySearchParamV2(string _OwnerID, string _DocCategoryID,
            string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, string searchType, out List<dynamic> docList, out List<string> attributeList);

        ValidationResult GetDocumentsByWildSearch(string _SearchFor, string _UserID, out List<DocSearch> userList);
        ValidationResult GetOriginalDocMeta(string _DocumentId, string _DocDistributionID,out List<OriginalDocMeta> metaList);

        ValidationResult GetDocumentsBySearchParamForVersion(string _OwnerID, string _DocCategoryID,
            string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<DocSearch> userList);


        ValidationResult GetOriginalDocBySearchFromList(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string value, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<DocSearch> docList);

        ValidationResult GetOriginalDocBySearchForPrint(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string Docs, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<DocSearch> docList);
        ValidationResult UpdateDocMailNotifyAndExpDate(string UserID,string DocID,  string NotifyDate, string ExpDate, out string outStatus);
        ValidationResult GetMailNotifyAndExpDate(string DocumentId, out DSM_VM_Property NotifyAndExpDate);
 
    }
}
