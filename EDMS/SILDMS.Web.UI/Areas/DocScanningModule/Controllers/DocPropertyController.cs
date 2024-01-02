using SILDMS.Model.DocScanningModule;
using SILDMS.Service.AutoValueSetup;
using SILDMS.Service.DocProperty;
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
using System.Web.SessionState;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class DocPropertyController : Controller
    {
        readonly IDocPropertyService _docPropertyService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult respStatus = new ValidationResult();
        private string outStatus = string.Empty;
        private string action = string.Empty;
        private readonly string UserID = string.Empty;
        readonly IAutoValueSetupService _autoValueSetupService;

        public DocPropertyController(IAutoValueSetupService autoValueSetupService, IDocPropertyService repository, ILocalizationService localizationService)
        {
            this._docPropertyService = repository;
            this._localizationService = localizationService;
            _autoValueSetupService = autoValueSetupService;
            UserID = SILAuthorization.GetUserID();
        }
        
        [SILAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<dynamic> GetDocProperty(string DocCategoryID = "", string OwnerID = "", string DocTypeID = "")
        {
            if(!string.IsNullOrEmpty(DocCategoryID) && !string.IsNullOrEmpty(OwnerID) && !string.IsNullOrEmpty(DocTypeID))
            { 
                List<DSM_DocProperty> obDocProperty = null;
                await Task.Run(() => _docPropertyService.GetDocProperty("", UserID, out obDocProperty));

                var result = obDocProperty.Where(ob => (ob.DocCategoryID == DocCategoryID.Trim()) && (ob.OwnerID == OwnerID.Trim()) && (ob.DocTypeID == DocTypeID.Trim())).Select(x => new DSM_DocProperty
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
                    InformationValidityPeriod = x.InformationValidityPeriod,
                    email = x.email,
                    sms = x.sms,
                    obsulate = x.obsulate,
                    emailBody = x.emailBody,
                    emailSub = x.emailSub,
                    smsBody = x.smsBody,
                    emailId = x.emailId,
                    smsNum = x.smsNum,
                    SetOn = x.SetOn,
                    Status = x.Status             
                }).OrderBy(ob => ob.SerialNo);

                return Json(new {Message = "", result}, JsonRequestBehavior.AllowGet);
            }

            return Json(new {Message = "Invalid Request."}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        [SILLogAttribute]
        public async Task<dynamic> AddDocProperty(DSM_DocProperty objDocProperty)
        {
            if (ModelState.IsValid)
            {
                action = "add";
                objDocProperty.SetBy = UserID;
                objDocProperty.ModifiedBy = objDocProperty.SetBy;
                objDocProperty.ConfigureColumnIds = _autoValueSetupService.GetConfigureColumnList("/DocScanningModule/DocProperty",
                objDocProperty.OwnerID, objDocProperty.DocCategoryID, objDocProperty.DocTypeID, null, null);
                respStatus = await Task.Run(() => _docPropertyService.AddDocProperty(objDocProperty, action, out outStatus));
                // Error handling.   

                ViewBag.LoggID = objDocProperty.DocPropertyID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Insert DocProperty";
                ViewBag.LookupTable = "DSM_DocProperty";

                return Json(new {Message = respStatus.Message, respStatus}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));
            }

            return Json(new {Message = respStatus.Message, respStatus}, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [Authorize]
        [SILLogAttribute]
        public async Task<dynamic> EditDocProperty(DSM_DocProperty obDocProperty)
        {
            if (ModelState.IsValid)
            {
                action = "edit";
                obDocProperty.SetBy = UserID;
                obDocProperty.ModifiedBy = obDocProperty.SetBy;
                respStatus = await Task.Run(() => _docPropertyService.AddDocProperty(obDocProperty, action, out outStatus));
                ViewBag.LoggID = obDocProperty.DocPropertyID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Edit DocProperty";
                ViewBag.LookupTable = "DSM_DocProperty";
                return Json(new {Message = respStatus.Message, respStatus}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));
            }

            return Json(new {Message = respStatus.Message, respStatus}, JsonRequestBehavior.AllowGet);
        }
       
	}
}