using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.DataAccessInterface.DocMkCkStage
{
    public interface IDocMkCkStageDataService
    {
        WFM_ProcessStageMap GetStageAndUserPermission(int stageMapID, string userID);
        VM_DocumentsProperty GetALLDocsProp(string _DocCategoryID, string _OwnerID, string _DocTypeID, int _StageMapID);
        DataTable GetMkCkDocuments(string _OwnerID, string _DocCategoryID, string _DocTypeID, int _StageMapID, string userID, int page, int itemsPerPage, string sortBy, bool reverse,string search);
        VM_DocumentsPropertyValues GetDocumentPropertyValues(string userID, string _ObjectID,string _StageMapID);
        List<DSM_Documents> UpdateDocumentInfo(string objectID, string docs, List<ObjectProperties> props, string userID, string v, out string _errorNumber);
        void SetCheckDone(string objectID, int stageMapID, string UserID, out string res_code, out string res_message);
        void SetMakeDone(string objectID, int stageMapID, string UserID, out string res_code, out string res_message);

        bool IsClearForMaking(string objectID, int stageMapID);
        void DeleteDocumentInfo(string objectID, string documentIDs, string action, out string res_code, out string res_message);
        void RevertFromMake(string objectID, int stageMapID, string revertReason, string userID, out string res_code, out string res_message);
        void RevertFromCheck(string objectID, int stageMapID, string revertReason, string userID, out string res_code, out string res_message);
        VM_ListPropertyCheck GetDocumentListPropertyValuesCheck(string _ObjectID, string _TableRefID);
        List<MasterData> GetMasterDataBySearch(string masterID, string searchKey);
        void DeleteListItem(string tableRefID, int id, string userID, out string res_code, out string res_message);
        VM_ListTypeProperties ToggleAddNewListItem(string tableRefID);
        void AddSingleListItem(List<WFM_TableProperty> listItemColumn, string tableRefID, string objectID, out string res_code, out string res_message);
        List<DSM_DocPropIdentify> AddDocumentInfo(DocumentsInfo _modelDocumentsInfo, string _selectedPropID, List<WFM_DocStageProperty> _docMetaValues, string listPropSql, 
            bool _otherUpload, string _extentions, string _action, out string _errorNumber);
        string DeleteDocumentInfo(string _DocumentIDs, out string _errorNumber);
    }
}
