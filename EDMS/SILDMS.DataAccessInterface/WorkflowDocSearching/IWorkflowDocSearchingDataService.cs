using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccessInterface.WorkflowDocSearching
{
    public interface IWorkflowDocSearchingDataService
    {
        DataTable GetWorkflowDocument(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string search, out string errorNumber);
        List<DSM_Documents> GetDocumentForSpecificObject(string _ObjectID, string _UserID);
        VM_DocumentsPropertyValuesAll GetDocumentPropertyValues(string userID, string _ObjectID);
    }
}
