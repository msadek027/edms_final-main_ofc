using SILDMS.DataAccessInterface.WorkflowDocSearching;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using SILDMS.Utillity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.WorkflowDocSearching
{
    public class WorkflowDocSearchingService : IWorkflowDocSearchingService
    {
        private readonly IWorkflowDocSearchingDataService workflowDocSearchingDataService;
        private readonly Utillity.Localization.ILocalizationService _localizationService;
        private string errorNumber = "";
        public WorkflowDocSearchingService(IWorkflowDocSearchingDataService _workflowDocSearchingDataService, Utillity.Localization.ILocalizationService localizationService)
        {
            workflowDocSearchingDataService = _workflowDocSearchingDataService;
            this._localizationService = localizationService;
        }

        public ValidationResult GetWorkflowDocument(string _OwnerID, string _DocCategoryID,
            string _DocTypeID, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string search,
            out DataTable docList)
        {
            docList = workflowDocSearchingDataService.GetWorkflowDocument(_OwnerID, _DocCategoryID, _DocTypeID,
                _UserID, page, itemsPerPage, sortBy, reverse, search, out errorNumber);
            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }
            return ValidationResult.Success;
        }

        public void GetDocumentForSpecificObject(string _ObjectID, string _UserID, out List<DSM_Documents> documents)
        {
            documents = workflowDocSearchingDataService.GetDocumentForSpecificObject(_ObjectID,
                _UserID);
        }

        public void GetDocumentPropertyValues(string userID, string _ObjectID, out VM_DocumentsPropertyValuesAll obj)
        {
            obj = workflowDocSearchingDataService.GetDocumentPropertyValues(userID, _ObjectID);
        }
    }
}
