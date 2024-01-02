using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccessInterface.WorkflowCreate
{
    public interface IWorkflowCreateDataService
    {

        void AddWorkflow(WFM_Workflow workflow, string UserID,out string _outStatus);
        void EditWorkflow(WFM_Workflow workflow, out string _outStatus);
        void WorkflowStatusChange(int WorkflowID, int Status, string UserID, out string _outStatus);
        List<WFM_Workflow> getListofWorkflow();


    }
}
