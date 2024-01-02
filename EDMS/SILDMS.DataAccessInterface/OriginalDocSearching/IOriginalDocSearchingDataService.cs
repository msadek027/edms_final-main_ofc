using System.Collections.Generic;
using SILDMS.Model.DocScanningModule;
using SILDMS.Web.UI.Models;

namespace SILDMS.DataAccessInterface.OriginalDocSearching
{
    public interface IOriginalDocSearchingDataService
    {
        string DeleteDocument(string _DocumentID,string _DocDistributionID,string _DocumentType, string userId, out string outStatus);

        string UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo, string userId,
            out string outStatus);

        List<DocSearch> GetOriginalDocBySearchParam(string _OwnerID, string _DocCategoryID,
            string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber);

        List<DocSearch> GetOriginalDocBySearchParamV2(string _OwnerID, string _DocCategoryID,
            string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, string searchType, out string errorNumber);

        List<DocSearch> GetOriginalDocumentsByWildSearch(string _SearchFor, string _UserID, out string errorNumber);

        List<OriginalDocMeta> GetOriginalDocMeta(string _DocumentId, string _DocDistributionID, out string errorNumber);

        List<DocSearch> GetDocumentsBySearchParamForVersion(string _OwnerID, string _DocCategoryID,
             string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber);


        List<DocSearch> GetOriginalDocBySearchFromList(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string value, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber);

        List<DocSearch> GetOriginalDocBySearchForPrint(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string Docs, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out string errorNumber);
        void UpdateDocMailNotifyAndExpDate(string UserID, string DocID, string NotifyDate, string ExpDate, out string outStatus);
        DSM_VM_Property GetMailNotifyAndExpDate(string DocumentId);
    
    }
}
