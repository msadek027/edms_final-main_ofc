using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.Service.DocMkCkStage
{
    public interface IDocMkCkStageService
    {
        void GetStageAndUserPermission(int stageMapID, string userID, out WFM_ProcessStageMap obj);
        void GetALLDocsProp(string _DocCategoryID, string _OwnerID, string _DocTypeID, int _StageMapID, out VM_DocumentsProperty obj);
        void GetMkCkDocuments(string _OwnerID, string _DocCategoryID, string _DocTypeID, int _StageMapID, string userID, int page, int itemsPerPage, string sortBy, bool reverse, string search, out DataTable dt);
        void GetDocumentPropertyValues(string userID, string _ObjectID,string _StageMapID ,out VM_DocumentsPropertyValues obj);
        ValidationResult UpdateDocumentInfo(string objectID, string docs, List<ObjectProperties> props, string userID, string v, out List<DSM_Documents> objDocPropIdentifies);
        void SetCheckDone(string objectID, int stageMapID, string UserID, out string res_code, out string res_message);
        void SetMakeDone(string objectID, int stageMapID, string UserID, out string res_code, out string res_message);
        void IsClearForMaking(string objectID, int stageMapID,  out bool IsClearForMaking);
        ValidationResult DeleteDocumentInfo(string objectID, string documentIDs, string action, out string res_code, out string res_message);
        void RevertFromMake(string objectID, int stageMapID,string revertReason, string userID, out string res_code, out string res_message);
        void RevertFromCheck(string objectID, int stageMapID, string revertReason, string userID, out string res_code, out string res_message);
        void GetDocumentListPropertyValuesCheck( string _ObjectID, string _TableRefID, out VM_ListPropertyCheck obj);
        void GetMasterDataBySearch(string masterID, string searchKey, out List<MasterData> objs);
        void DeleteListItem(string tableRefID, int id, string userID, out string res_code, out string res_message);
        void ToggleAddNewListItem(string tableRefID, out VM_ListTypeProperties obj);
        void AddSingleListItem(List<WFM_TableProperty> listItemColumn, string tableRefID, string objectID, out string res_code, out string res_message);

        ValidationResult AddDocumentInfo(DocumentsInfo _modelDocumentsInfo, string _selectedPropID, List<WFM_DocStageProperty> _docMetaValues, string listPropSql, 
            bool _otherUpload, string _extentions, string action, out List<DSM_DocPropIdentify> docPropIdentifyList);

        ValidationResult DeleteDocumentInfo(string _DocumentIDs);
    }
}
