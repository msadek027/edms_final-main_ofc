using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using SILDMS.Service.AutoValueSetup;
using SILDMS.Service.WorkflowSetup;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Areas.WorkflowModule.Controllers
{
    public class WorkflowSetupController : Controller
    {
        // GET: WorkflowModule/WorkflowSetup

        private readonly string UserID = string.Empty;
        private string res_code = string.Empty;
        private string res_message = string.Empty;
        private readonly IWorkflowSetupService _service;
        private readonly IAutoValueSetupService _autoValueSetupService;
        private readonly ILocalizationService _localizationService;
        private string action = string.Empty;
        private ValidationResult respStatus = new ValidationResult();
        private string outStatus = string.Empty;

        public WorkflowSetupController(IWorkflowSetupService service, IAutoValueSetupService autoValueSetupService, ILocalizationService localizationService)
        {
            UserID = SILAuthorization.GetUserID();
            this._service = service;
            this._autoValueSetupService = autoValueSetupService;
            this._localizationService = localizationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ParallelStageSetup()
        {
            return View();
        }

        public ActionResult WorkflowConfiguration()
        {
            return View();
        }

        //Normal Stages
        public async Task<dynamic> GetALLStagesForType(string OwnerID = "", string DocCategoryID = "", string DocTypeID = "")
        {
            var obj = new List<WFM_ProcessStageMap>();
            await Task.Run(() => _service.GetALLStagesForType(DocCategoryID, OwnerID, DocTypeID, out obj));

            return Json(new { obj, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetsPECIFICStagesForType(string OwnerID = "", string DocCategoryID = "", string DocTypeID = "")
        {
            var obj = new List<WFM_ProcessStageMap>();
            await Task.Run(() => _service.GetALLStagesForType(DocCategoryID, OwnerID, DocTypeID, out obj));

            return Json(new { obj=obj.Where(n=>n.IsChecked), Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<dynamic> SetStagesForType(IEnumerable<WFM_ProcessStageMap> objs, string OwnerID = "", string DocCategoryID = "", string DocTypeID = "")
        {
            try
            {
                await Task.Run(() => _service.SetStagesForType(objs, UserID, OwnerID, DocCategoryID, DocTypeID, out res_code, out res_message));
                return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Parallel Stages
        public async Task<dynamic> GetAllBaseStage(string OwnerID = "", string DocCategoryID = "", string DocTypeID = "") {
            var obj = new List<WFM_ProcessStageMap>();
            await Task.Run(() => _service.GetALLStagesForType(DocCategoryID, OwnerID, DocTypeID, out obj));

            var result = obj.Where(c => c.StageSL>1 && c.StageSL <c.NumberOFStage);

            return Json(new { result, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetDocumentWisePermission(string DocPropertyID)
        {
            var obj = new List<int>();
            await Task.Run(() => _service.GetDocumentWisePermission(DocPropertyID, out obj));

           

            return Json(new { obj, Msg = "" }, JsonRequestBehavior.AllowGet);
        }
        public async Task<dynamic> GetChildStages(int StageMapID)
        {
            var obj = new List<WFM_ProcessStageMap>();
            await Task.Run(() => _service.GetChildStages(StageMapID, out obj));

            

            return Json(new { obj }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetALLPStageProperty(string DocCategoryID = "", string OwnerID = "", string DocTypeID = "", int stageMapID = 0)
        {
            var tp = new WFM_VM_ParallelStageProperty();
            await Task.Run(() => _service.GetALLPStageProperty(DocCategoryID, OwnerID, DocTypeID, stageMapID, out tp));

            return Json(new { tp, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<dynamic> SaveParallelStage(string DocCategoryID = "", string OwnerID = "", string DocTypeID = "", int ProcessingStageId = 0, int ParallelStageID = 0, int EndStageMapID = 0)
        {
            try
            {
                await Task.Run(() => _service.SaveParallelStage(UserID, DocCategoryID, OwnerID, DocTypeID, ProcessingStageId, ParallelStageID, EndStageMapID, out res_code, out res_message));
                return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
            }
        }

        //WorkflowConfiguration -----Property
        public async Task<dynamic> GetALLStageProperty(string DocCategoryID = "", string OwnerID = "", string DocTypeID = "", int stageMapID = 0)
        {
            var tp = new List<WFM_DocStageProperty>();
            await Task.Run(() => _service.GetALLStageProperty(DocCategoryID, OwnerID, DocTypeID, stageMapID, out tp));

            return Json(new { tp, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<dynamic> AddStageProperty(WFM_DocStageProperty tp)
        {
            if (ModelState.IsValid)
            {
                if (tp.DocTypePropertyID == "" || tp.DocTypePropertyID == null)
                {
                    action = "add";
                }
                else
                {
                    action = "edit";
                }

                tp.SetBy = UserID;
                await Task.Run(() => _service.AddStageProperty(tp, action, out res_code, out res_message));

                return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
        }

        //WorkflowConfiguration -----Document
        [HttpGet]
        [Authorize]
        public async Task<dynamic> GetAllDocuments(string OwnerID = "", string DocCategoryID = "", string DocTypeID = "", int StageMapID = 0)
        {
            if (!string.IsNullOrEmpty(DocCategoryID) && !string.IsNullOrEmpty(OwnerID) && !string.IsNullOrEmpty(DocTypeID) && StageMapID != 0)
            {
                List<DSM_DocProperty> obDocProperty = null;
                await Task.Run(() => _service.GetAllDocuments("", UserID, out obDocProperty));

                var result = obDocProperty.Where(ob => (ob.DocCategoryID == DocCategoryID.Trim()) && (ob.OwnerID == OwnerID.Trim()) && (ob.DocTypeID == DocTypeID.Trim()) && (ob.StageMapID == StageMapID)).Select(x => new DSM_DocProperty
                {
                    DocPropertyID = x.DocPropertyID,
                    DocCategoryID = x.DocCategoryID,
                    OwnerLevelID = x.OwnerLevelID,
                    OwnerID = x.OwnerID,
                    DocTypeID = x.DocTypeID,
                    DocPropertySL = x.DocPropertySL,
                    UDDocPropertyCode = x.UDDocPropertyCode,
                    DocPropertyName = x.DocPropertyName,
                    DocClassification = x.DocClassification,
                    PreservationPolicy = x.PreservationPolicy,
                    PhysicalLocation = x.PhysicalLocation,
                    Remarks = x.Remarks,
                    SerialNo = x.SerialNo,
                    SetOn = x.SetOn,
                    Status = x.Status
                }).OrderBy(ob => ob.SerialNo);

                return Json(new { Message = "", result }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "Invalid Request." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        [SILLogAttribute]
        public async Task<dynamic> AddNewDocument(DSM_DocProperty objDocProperty,List<WFM_ProcessStageMap> DocumentPermissionModal)
        {
         
                if (objDocProperty.DocPropertyID == "" || objDocProperty.DocPropertyID == null)
                {
                    action = "add";
                }
                else
                {
                    action = "edit";
                }

                objDocProperty.SetBy = UserID;
                objDocProperty.ModifiedBy = objDocProperty.SetBy;
                objDocProperty.ConfigureColumnIds = _autoValueSetupService.GetConfigureColumnList("/DocScanningModule/DocProperty",
                objDocProperty.OwnerID, objDocProperty.DocCategoryID, objDocProperty.DocTypeID, null, null);

                respStatus = await Task.Run(() => _service.AddNewDocument(objDocProperty, DocumentPermissionModal, action, out outStatus));
                
                // Error handling.   
                ViewBag.LoggID = objDocProperty.DocPropertyID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Add Documents";
                ViewBag.LookupTable = "DSM_Documents";
                return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
         

            return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Authorize]
        [SILLogAttribute]
        public async Task<dynamic> SaveProcessingStageConnections(List<WFM_ProcessingStageConnection> ProcessingStageConnections, List<WFM_ProcessStage> StagePositionList)
        {
            if (StagePositionList != null) {
               
                    await Task.Run(() => _service.SaveNodeCoordinate(StagePositionList, UserID, out res_message));
                
            }
           
            try
            {
                await Task.Run(() => _service.SaveProcessingStageConnections(ProcessingStageConnections, UserID, out res_message));
                return Json(new { Message = res_message, Code = "1" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [Authorize]
        [SILLogAttribute]
        public async Task<dynamic> GetProcessStagesConnectionsByWorkflow(int WorkflowID)
        {
            List<WFM_ProcessingStageConnection> ProcessingStageConnections = new List<WFM_ProcessingStageConnection>();
            try
            {
                await Task.Run(() => _service.GetProcessStagesConnectionsByWorkflow(WorkflowID, out ProcessingStageConnections,out res_code));
                return Json(new { ProcessingStageConnections, Message = res_code, Code = "1" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}