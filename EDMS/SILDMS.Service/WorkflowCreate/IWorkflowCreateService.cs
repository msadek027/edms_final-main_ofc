using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using SILDMS.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.WorkflowCreate
{
    public interface IWorkflowCreateService
    {
        
        void AddWorkflow(WFM_Workflow workflow, string UserID, out string _outStatus);
        ValidationResult EditWorkflow(WFM_Workflow workflow, out string _outStatus);
        ValidationResult WorkflowStatusChange(int WorkflowID, int Status, string UserID);
        void getListofWorkflow(out List<WFM_Workflow> workflows);

        //ValidationResult GetAllDocuments(string DocPropertyId, string action, out List<DSM_DocProperty> ownerLevelList);
        //ValidationResult AddNewDocument(DSM_DocProperty ownerLevel, string action, out string status);
    }
}
