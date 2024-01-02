using SILDMS.DataAccessInterface.DocMkCkStage;
using SILDMS.Utillity.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using System.Data;
using SILDMS.Utillity;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.Service.DocMkCkStage
{
    public class DocMkCkStageService : IDocMkCkStageService
    {
        private readonly IDocMkCkStageDataService _docMkCkStageDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber = string.Empty;

        public DocMkCkStageService(IDocMkCkStageDataService docMkCkStageDataService,ILocalizationService localizationService)
        {
            _docMkCkStageDataService = docMkCkStageDataService;
            _localizationService = localizationService;
        }

        public void GetStageAndUserPermission(int stageMapID, string userID, out WFM_ProcessStageMap obj)
        {
            obj = _docMkCkStageDataService.GetStageAndUserPermission(stageMapID,userID);
        }

        public void GetALLDocsProp(string _DocCategoryID, string _OwnerID, string _DocTypeID, int _StageMapID, out VM_DocumentsProperty obj)
        {
            obj = _docMkCkStageDataService.GetALLDocsProp(_DocCategoryID, _OwnerID, _DocTypeID, _StageMapID);
        }

        public void GetMkCkDocuments(string _OwnerID, string _DocCategoryID, string _DocTypeID, int _StageMapID, string userID, int page, int itemsPerPage, string sortBy, bool reverse, string search, out DataTable dt)
        {
            dt = _docMkCkStageDataService.GetMkCkDocuments(_OwnerID, _DocCategoryID, _DocTypeID, _StageMapID, userID, page, itemsPerPage, sortBy, reverse, search);
        }

        public void GetDocumentPropertyValues(string userID, string _ObjectID,string _StageMapID ,out VM_DocumentsPropertyValues obj)
        {
            obj = _docMkCkStageDataService.GetDocumentPropertyValues(userID,_ObjectID, _StageMapID);
        }

        public ValidationResult UpdateDocumentInfo(string objectID, string docs, List<ObjectProperties> props, string userID, string v, out List<DSM_Documents> objDocPropIdentifies)
        {
            objDocPropIdentifies = _docMkCkStageDataService.UpdateDocumentInfo(objectID, docs, props, userID,v, out _errorNumber);

            if (_errorNumber == "S201")
            {
                return ValidationResult.Success;
            }

            return  new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
        }

        public void SetCheckDone(string objectID, int stageMapID, string UserID, out string res_code, out string res_message)
        {
            _docMkCkStageDataService.SetCheckDone(objectID, stageMapID,UserID, out res_code, out res_message);
        }

        public void SetMakeDone(string objectID, int stageMapID, string UserID, out string res_code, out string res_message)
        {
            _docMkCkStageDataService.SetMakeDone(objectID, stageMapID,UserID, out res_code, out res_message);
        }

        public void IsClearForMaking(string objectID, int stageMapID, out bool IsClearForMaking)
        {
            IsClearForMaking= _docMkCkStageDataService.IsClearForMaking(objectID, stageMapID);
        }
        public ValidationResult DeleteDocumentInfo(string objectID, string documentIDs, string action, out string res_code, out string res_message)
        {
            _docMkCkStageDataService.DeleteDocumentInfo(objectID, documentIDs, action, out res_code, out res_message);
             return ValidationResult.Success;
        }

        public void RevertFromMake(string objectID, int stageMapID, string revertReason, string userID, out string res_code, out string res_message)
        {
            _docMkCkStageDataService.RevertFromMake(objectID, stageMapID, revertReason, userID, out res_code, out res_message);
        }

        public void RevertFromCheck(string objectID, int stageMapID,string revertReason, string userID, out string res_code, out string res_message)
        {
            _docMkCkStageDataService.RevertFromCheck(objectID, stageMapID, revertReason, userID, out res_code, out res_message);
        }

        public void GetDocumentListPropertyValuesCheck(string _ObjectID, string _TableRefID, out VM_ListPropertyCheck obj)
        {
            obj = _docMkCkStageDataService.GetDocumentListPropertyValuesCheck(_ObjectID, _TableRefID);
        }

        public void GetMasterDataBySearch(string masterID, string searchKey, out List<MasterData> objs)
        {
            objs=_docMkCkStageDataService.GetMasterDataBySearch(masterID, searchKey);
        }

        public void DeleteListItem(string tableRefID, int id, string userID, out string res_code, out string res_message)
        {
            _docMkCkStageDataService.DeleteListItem(tableRefID, id, userID, out res_code, out res_message);
        }

        public void ToggleAddNewListItem(string tableRefID, out VM_ListTypeProperties obj)
        {
            obj = _docMkCkStageDataService.ToggleAddNewListItem(tableRefID);
        }

        public void AddSingleListItem(List<WFM_TableProperty> listItemColumn, string tableRefID, string objectID, out string res_code, out string res_message)
        {
            _docMkCkStageDataService.AddSingleListItem(listItemColumn, tableRefID, objectID, out res_code, out res_message);
        }

        public ValidationResult AddDocumentInfo(DocumentsInfo _modelDocumentsInfo, string _selectedPropID, List<WFM_DocStageProperty> _docMetaValues, string listPropSql, 
            bool _otherUpload, string _extentions, string action, out List<DSM_DocPropIdentify> docPropIdentifyList)
        {
            docPropIdentifyList = _docMkCkStageDataService.AddDocumentInfo(_modelDocumentsInfo, _selectedPropID, _docMetaValues, listPropSql, _otherUpload, _extentions, action, out _errorNumber);

            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult DeleteDocumentInfo(string _DocumentIDs)
        {
            _docMkCkStageDataService.DeleteDocumentInfo(_DocumentIDs, out _errorNumber);

            if (_errorNumber == "S201")
            {
                return ValidationResult.Success;
            }
            else
            {
                return ValidationResult.Success;
            }
        }

    }
}
