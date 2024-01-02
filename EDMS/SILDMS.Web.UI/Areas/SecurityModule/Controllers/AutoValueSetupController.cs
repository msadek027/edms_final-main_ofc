using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.SecurityModule;
using SILDMS.Service.AutoValueSetup;
using SILDMS.Service.DocProperty;
using SILDMS.Service.DocumentCategory;
using SILDMS.Service.DocumentType;
using SILDMS.Service.Owner;
using SILDMS.Service.OwnerLevel;
using SILDMS.Service.OwnerProperIdentity;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule.Models;

namespace SILDMS.Web.UI.Areas.SecurityModule.Controllers
{
    public class AutoValueSetupController : Controller
    {
        readonly IAutoValueSetupService _autoValueSetupService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult respStatus = new ValidationResult();
        private string outStatus = string.Empty;
        private string action = "";
        private readonly string UserID = string.Empty;

        public AutoValueSetupController(IAutoValueSetupService autoValueSetupService, ILocalizationService localizationService)
        {
            _autoValueSetupService = autoValueSetupService;
            _localizationService = localizationService;
            UserID = SILAuthorization.GetUserID();
        }
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]   
        public async Task<dynamic> AddAutoValueSetup(SEC_AutoValueSetup autoValueSetupModel)
        {
            if (ModelState.IsValid)
            {
                action = "add";
                autoValueSetupModel.SetBy = UserID;
                autoValueSetupModel.ModifiedBy = autoValueSetupModel.SetBy;
                respStatus = await Task.Run(() => _autoValueSetupService.AddAutoValueSetup(autoValueSetupModel, action, out outStatus));

                return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));
            }

            return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]  
        public async Task<dynamic> EditAutoValueSetup(SEC_AutoValueSetup modelDsmDocPropIdentify)
        {
            if (ModelState.IsValid)
            {
                action = "edit";
                modelDsmDocPropIdentify.SetBy = UserID;
                modelDsmDocPropIdentify.ModifiedBy = modelDsmDocPropIdentify.SetBy;
                respStatus = await Task.Run(() => _autoValueSetupService.EditAutoValueSetup(modelDsmDocPropIdentify, action, out outStatus));

                return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));
            }

            return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]    
        public async Task<dynamic> GetAutoValueSetupData(string _ConfigureID, string _ConfigureColumnID)
        {
            List<SEC_AutoValueSetup> autoValueSetups = new List<SEC_AutoValueSetup>();

            _autoValueSetupService.GetAutoValueSetupData(_ConfigureID, _ConfigureColumnID, UserID, out autoValueSetups);

            return Json(autoValueSetups, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
 
        public async Task<dynamic> GetConfiguredMenuList()
        {
            List<SEC_Menu> secMenus = new List<SEC_Menu>();
            _autoValueSetupService.GetConfiguredMenuList(UserID, out secMenus);

            return Json(secMenus, JsonRequestBehavior.AllowGet);
        }

        //[Authorize]
        //[HttpGet]       
        //public string GetConfigureColumnList(string menuUrl)
        //{
        //  return  _autoValueSetupService.GetConfigureColumnList(menuUrl).ToString();
        //}

	}
}