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
    public interface IWorkflowDocSearchingService
    {
        ValidationResult GetWorkflowDocument(string _OwnerID, string _DocCategoryID,
           string _DocTypeID, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string search, out DataTable userList);

        void GetDocumentForSpecificObject(string _ObjectID, string _UserID, out List<DSM_Documents> documents);

        void GetDocumentPropertyValues(string userID, string _ObjectID, out VM_DocumentsPropertyValuesAll obj);
    }
}
