using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using SILDMS.Service.DocumentType;
using SILDMS.Service.ProcessStageSetup;
using SILDMS.Service.WorkflowCreate;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Areas.WorkflowModule
{
    public class ProStageController : Controller
    {
        #region Fields
        private readonly IProcessStageService _processStageService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult _respStatus = new ValidationResult();
        private readonly IDocTypeService _docTypeService;
        private string res_code = string.Empty;
        private string res_message = string.Empty;
        private string _action = string.Empty;
        private readonly IWorkflowCreateService _workflowCreateService;
        private readonly string UserID = string.Empty;
        #endregion

        public ProStageController(IProcessStageService processStageService, IDocTypeService docTypeService, ILocalizationService localizationService, IWorkflowCreateService workflowCreateService)
        {
            _processStageService = processStageService;
            _localizationService = localizationService;
            _docTypeService = docTypeService;
            _workflowCreateService = workflowCreateService;
            UserID = SILAuthorization.GetUserID();
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<dynamic> GetDocumentTypeForSpecificDocCategory(string _DocCategoryID, string _OwnerID)
        {
            

            var docTypes = new List<DSM_DocType>();
            List<string> DSM_DocTypeID = new List<string>();
            List<WFM_Workflow> Workflows = new List<WFM_Workflow>();
            await Task.Run(() => _docTypeService.GetDocTypes("", UserID, out docTypes));

            var doctypeResult = docTypes.Where(ob => ob.DocCategoryID == _DocCategoryID).Select(x => new
            {
                x.DocTypeID,
                x.DocTypeSL,
                x.UDDocTypeCode,
                x.DocTypeName,
                x.DocPreservationPolicy,
                x.DocPhysicalLocation,
                x.DocCategoryID,
                x.OwnerID,
                x.Status,
                x.DocClassification,
                x.ClassificationLevel

            }).OrderByDescending(ob => ob.DocCategoryID).ToList();
            await Task.Run(() => _workflowCreateService.getListofWorkflow(out Workflows));


            var result = (from wf in Workflows
                          join dt in doctypeResult on wf.DocTypeID equals dt.DocTypeID
                          where wf.Status == 1
                          select new WFM_Workflow
                          {
                              DocTypeID = wf.DocTypeID,
                              
                              WorkflowName = wf.WorkflowName,
                             
                             
                          });



            return Json(new { Msg = "", result }, JsonRequestBehavior.AllowGet);
        }
        public async Task<dynamic> GetALLProcessStage()
        {

            var stages = new List<WFM_ProcessStage>();
            await Task.Run(() => _processStageService.GetALLProcessStage(UserID, out stages));

            return Json(new { stages, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<dynamic> AddProcessStage(WFM_ProcessStage stage)
        {

            stage.SetBy = UserID;
            List<WFM_ProcessStage> stages = new List<WFM_ProcessStage>() { };
            if (stage.StageID != 0)
            {

                await Task.Run(() => _processStageService.UpdateProcessStage(stage, out res_code, out res_message));

                return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                stages.Add(stage);
                await Task.Run(() => _processStageService.AddProcessStages(stages, UserID, out res_message));

            }
            return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
        }
        
      
        [HttpPost]
        public async Task<dynamic> AddProcessStages(List<WFM_ProcessStage> stages)
        {

            await Task.Run(() => _processStageService.AddProcessStages(stages, UserID, out res_message));

            if (res_message.Contains("S201"))
            {
                return Json(new { Message = "Stages Updated Successfully", Code ="1" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "Error", Code = "2" }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public async Task<dynamic> GetProcessStagesByWorkflow(string DocTypeID)
        {
            if (DocTypeID!=string.Empty)
            {
                List<WFM_ProcessStage> stages = new List<WFM_ProcessStage>();
                WFM_Workflow workflow = new WFM_Workflow();
                _respStatus = await Task.Run(() => _processStageService.GetProcessStagesByWorkflow(DocTypeID, UserID, out stages,out workflow));

                if (stages.Count>0)
                {
                    return Json(new { stages, Msg = "success",code="1" }, JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(new { workflow, Msg = "success",code="2" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Msg = "Please fill up all required data",code="3"}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<dynamic> GetProcessStagesByWorkflowForParallelStage(string DocTypeID)
        {
            if (DocTypeID != string.Empty)
            {
                List<WFM_ProcessStage> stages = new List<WFM_ProcessStage>();
                WFM_Workflow workflow = new WFM_Workflow();
                _respStatus = await Task.Run(() => _processStageService.GetProcessStagesByWorkflow(DocTypeID, UserID, out stages, out workflow));
                var obj = stages.Where(c => c.StageSL < c.NumberOFStage);
                if (stages.Count > 0)
                {
                    return Json(new { obj, Msg = "success", code = "1" }, JsonRequestBehavior.AllowGet);
                }
               
            }

            return Json(new { Msg = "Please Update the Stages of Workflow", code = "3" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<dynamic> GetProcessStagesByStageID(int StageID)
        {
            WFM_ProcessStage stage = new WFM_ProcessStage();

            await Task.Run(() => _processStageService.GetProcessStagesByStageID(StageID,out stage, out res_message));

            return Json(new { stage, Message = res_message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<dynamic> GetChildProcessStagesByStageID(int StageID)
        {
            List<WFM_ProcessStage> ChildStages = new List<WFM_ProcessStage>();

            await Task.Run(() => _processStageService.GetChildProcessStagesByStageID(StageID, out ChildStages, out res_message));

            return Json(new { ChildStages, Message = res_message }, JsonRequestBehavior.AllowGet);
        }

    }
}


