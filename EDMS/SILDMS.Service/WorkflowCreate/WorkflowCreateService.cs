using SILDMS.DataAccessInterface.WorkflowCreate;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.WorkflowCreate
{
    public class WorkflowCreateService : IWorkflowCreateService
    {
        private readonly IWorkflowCreateDataService workflowCreateDataService;
        private readonly ILocalizationService localizationService;
        private string errorNumber = "";

        public WorkflowCreateService(IWorkflowCreateDataService repository, ILocalizationService _localizationService)
        {
            this.workflowCreateDataService = repository;
            this.localizationService = _localizationService;
        }
       
        public void AddWorkflow(WFM_Workflow workflow, string UserID, out string _outStatus)
        {
            workflowCreateDataService.AddWorkflow(workflow, UserID, out _outStatus);
        }
        public ValidationResult EditWorkflow(WFM_Workflow workflow, out string _outStatus)
        {
            workflowCreateDataService.EditWorkflow(workflow, out _outStatus);
            return _outStatus.Contains("S") ? new ValidationResult(_outStatus, localizationService.GetResource(_outStatus)) : ValidationResult.Success;
        }
        public ValidationResult WorkflowStatusChange(int WorkflowID, int Status,string UserID)
        {
            string _outStatus = string.Empty;
            workflowCreateDataService.WorkflowStatusChange(WorkflowID, Status, UserID,out _outStatus);
            return _outStatus.Contains("S") ? new ValidationResult(_outStatus, localizationService.GetResource(_outStatus)) : ValidationResult.Success;
        }
        public void getListofWorkflow( out List<WFM_Workflow> Workflows)
        {
            Workflows= workflowCreateDataService.getListofWorkflow();
        }

        #region unnecessary
        //    public ValidationResult GetAllDocuments(string DocPropertyId, string action, out List<DSM_DocProperty> docPropertyList)
        //    {
        //        docPropertyList = workflowCreateDataService.GetAllDocuments(DocPropertyId, action, out errorNumber);
        //        if (errorNumber.Length > 0)
        //        {
        //            return new ValidationResult(errorNumber, localizationService.GetResource(errorNumber));
        //        }

        //        return ValidationResult.Success;
        //    }

        //    public ValidationResult AddNewDocument(DSM_DocProperty docProperty, string action, out string status)
        //    {
        //        workflowCreateDataService.AddNewDocument(docProperty, action, out status);
        //        if (status.Length > 0)
        //        {
        //            return new ValidationResult(status, localizationService.GetResource(status));
        //        }

        //        return ValidationResult.Success;


        //    }
        #endregion
    }

}
