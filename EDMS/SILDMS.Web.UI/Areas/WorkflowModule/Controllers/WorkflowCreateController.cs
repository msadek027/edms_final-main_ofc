using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SILDMS.Model.DocScanningModule;
using SILDMS.Service.DocumentCategory;
using SILDMS.Service.DocumentType;
using SILDMS.Service.Owner;
using SILDMS.Service.OwnerLevel;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using System.Web.SessionState;
using SILDMS.Model.SecurityModule;
using SILDMS.Service.UserLevel;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using SILDMS.Service.AutoValueSetup;
using SILDMS.Service.WorkflowCreate;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.Web.UI.Areas.WorkflowModule
{
    public class WorkflowCreateController : Controller
    {
        #region Fields
        readonly IOwnerService _ownerService; 
       
        private readonly IOwnerLevelService _ownerLevelService;
        private readonly IWorkflowCreateService _workflowCreateService;
        private readonly IDocCategoryService _docCategoryService;
        private readonly IDocTypeService _docTypeService;
        private readonly IUserLevelService _userLevelService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult _respStatus = new ValidationResult();
        private string _outStatus = string.Empty;
        
        private string _action = string.Empty;
        private readonly string UserID = string.Empty;
        readonly IAutoValueSetupService _autoValueSetupService;
        #endregion

        #region Constructor
        public WorkflowCreateController(IAutoValueSetupService autoValueSetupService, IDocTypeService docTypeService, ILocalizationService localizationService, 
            IOwnerService ownerService, IOwnerLevelService ownerLevelService, IDocCategoryService docCategoryService, IUserLevelService userLevelService,
            IWorkflowCreateService workflowCreateService)
        {
            _docTypeService = docTypeService;
            _localizationService = localizationService;
            _ownerService = ownerService;
            _ownerLevelService = ownerLevelService;
            _docCategoryService = docCategoryService;
            _userLevelService = userLevelService;
            _autoValueSetupService = autoValueSetupService;
            _workflowCreateService = workflowCreateService;
            UserID = SILAuthorization.GetUserID();
        }
        #endregion

        // GET: DocScanningModule/DocType
       // [SILAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        #region Document Type Manipulation
        // Add New Document Type
        [Authorize]
        [HttpPost]
        [SILLogAttribute]
        public async Task<dynamic> AddDocType(DSM_DocType objDocType, WFM_Workflow workflow)
        {
            if (ModelState.IsValid)
            {
                _action = "add";
                string ID=string.Empty;
                objDocType.SetBy = UserID;
                objDocType.ModifiedBy = objDocType.SetBy;
                objDocType.ConfigureColumnIds = _autoValueSetupService.GetConfigureColumnList("/DocScanningModule/DocType",
                 objDocType.OwnerID, objDocType.DocCategoryID, null, null, null);
                _respStatus = await Task.Run(() => _docTypeService.ManipulateDocTypeWorkflow(objDocType, _action, out _outStatus,out ID));
                if (!ID.Contains("404")) {
                    workflow.DocTypeID = ID;
                    await Task.Run(() => _workflowCreateService.AddWorkflow(workflow, UserID, out _outStatus));
                }
              
                // Error handling.   
                ViewBag.LoggID = objDocType.DocTypeID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Insert DocType";
                ViewBag.LookupTable = "DSM_DocType";
                return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
            }

            _respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));

            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
        }

        // Edit Document Type
        [Authorize]
        [HttpPost]
        [SILLogAttribute]
        public async Task<dynamic> EditDocType(WFM_Workflow workflow)
        {
            if (ModelState.IsValid)
            {
                _action = "edit";

                workflow.ModifiedBy =UserID;
                // _respStatus = await Task.Run(() => _docTypeService.ManipulateDocType(objDocType, _action, out _outStatus));
                _respStatus= await Task.Run(() => _workflowCreateService.EditWorkflow(workflow, out _outStatus));

                ViewBag.LoggID = workflow.DocTypeID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Update DocType";
                ViewBag.LookupTable = "DSM_DocType";
                return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
            }

            _respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));

            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
        }

        // Workflow Status Change
        [Authorize]
        [HttpPost]
        [SILLogAttribute]
        public async Task<dynamic> WorkflowStatusChange(int WorkflowID, int Status)
        {
         
                _respStatus = await Task.Run(() => _workflowCreateService.WorkflowStatusChange(WorkflowID, Status,UserID));

                ViewBag.LoggID = WorkflowID.ToString();
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Update Workflow";
                ViewBag.LookupTable = "WFM_WorkflowID";
              //  return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
            

            //_respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));

            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
        }

        // Delete Document Type
        [Authorize]
        [HttpPost]
        [SILLogAttribute]
        public async Task<dynamic> DeleteDocType(DSM_DocType objDocType)
        {
            _action = "delete";
            _respStatus = await Task.Run(() => _docTypeService.ManipulateDocType(objDocType, _action, out _outStatus));
            ViewBag.LoggID = objDocType.DocTypeID;
            ViewBag.LoggResult = "";
            ViewBag.LoggAction = "Delete DocType";
            ViewBag.LookupTable = "DSM_DocType";
            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get data for view
        [HttpGet]
        public async Task<dynamic> GetOwnerLevel(string id)
        {
            var ownerLevel = new List<DSM_OwnerLevel>();
            await Task.Run(() => _ownerLevelService.GetOwnerLevel(id, UserID, out ownerLevel));

            var result = ownerLevel.Where(ob => ob.Status == 1).Select(x => new
            {
                x.OwnerLevelID,
                x.LevelName
            }).OrderByDescending(ob => ob.LevelName);

            return Json(new { result, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        // Get Owner(s)
        [HttpGet]
        public async Task<dynamic> GetOwners(string id)
        {
            var owner = new List<DSM_Owner>();
            await Task.Run(() => _ownerService.GetAllOwners("", UserID, out owner));

            var result = owner.Where(ob => ob.OwnerLevelID == id && ob.Status == 1).Select(x => new
            {
                x.OwnerID,
                x.OwnerName
            }).OrderByDescending(ob => ob.OwnerID);

            return Json(new { result, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetDocCategoryForOwner(string id)
        {
            var docCategories = new List<DSM_DocCategory>();
            await Task.Run(() => _docCategoryService.GetDocCategories("", UserID, out docCategories));

            var result = docCategories.Where(ob => ob.OwnerID == id && ob.Status == 1).Select(x => new
            {
                x.DocCategoryID,
                x.DocCategoryName
            }).OrderByDescending(ob => ob.DocCategoryID);

            return Json(new { result, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetDocTypeForCategory(string id)
        {
            var docTypes = new List<DSM_DocType>();
            List<string> DSM_DocTypeID = new List<string>();
            List<WFM_Workflow>Workflows = new List<WFM_Workflow>();
            await Task.Run(() => _docTypeService.GetDocTypes("", UserID, out docTypes));

            var doctypeResult = docTypes.Where(ob => ob.DocCategoryID == id).Select(x => new
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
                          select new WFM_Workflow
                          {
                              DocTypeID = wf.DocTypeID,
                              WorkflowID = wf.WorkflowID,
                              WorkflowName = wf.WorkflowName,
                              WorkflowDescription = wf.WorkflowDescription,
                              NumberOfStage = wf.NumberOfStage,
                              Status = wf.Status
                          });




            return Json(new { result, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<dynamic> GetDocClassification(int? userLevel, string levelType)
        {
            var userLevels = new List<SEC_UserLevel>();
            await Task.Run(() => _userLevelService.GetUserLevels(userLevel, _action, levelType, out userLevels));

            var result = userLevels.Select(ob => new
            {
                ob.ID,
                DocClassificationName = ob.UserLevelName,
             
            }).OrderBy(ob => ob.ID);

            return Json(new { result, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<dynamic> GetClassificationLevel(int? userLevel, string levelType)
        {
            var userLevels = new List<SEC_UserLevel>();
            await Task.Run(() => _userLevelService.GetUserLevels(userLevel, _action, levelType, out userLevels));

            var result = userLevels.Select(ob => new
            {
                ob.ID,
                ClassificationLevelName = ob.UserLevelName,

            }).OrderBy(ob => ob.ID);

            return Json(new { result, Msg = "" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}