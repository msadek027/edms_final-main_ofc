using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SILDMS.Model.SecurityModule;
using SILDMS.Service.DefaultValueSetup;
using SILDMS.Service.Menu;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule.Models;

namespace SILDMS.Web.UI.Areas.SecurityModule.Controllers
{
    public class DefaultValueSetupController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IDefalutValueSetupService _defalutValueSetupService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult _respStatus;
        private string _outStatus;
        private string _action;
        private readonly string _userId ;

        public DefaultValueSetupController(IMenuService menuService, IDefalutValueSetupService defalutValueSetupService, ValidationResult respStatus, ILocalizationService localizationService)
        {
            _menuService = menuService;
            _defalutValueSetupService = defalutValueSetupService;
            _respStatus = respStatus;
            _localizationService = localizationService;
            _userId = SILAuthorization.GetUserID();
            _action = string.Empty;
            _outStatus = string.Empty;
        }

        // GET: SecurityModule/DefaultValueSetup
        public ActionResult Index()
        {
            return View();
        }

        public async Task<dynamic> GetAllMenu()
        {
            var menuList = new List<SEC_Menu>();
            await Task.Run(() => _defalutValueSetupService.GetMenusForDefaultValue(out menuList));

            var result = menuList.Select(ob => new SEC_Menu
            {
                MenuID = ob.MenuID,
                MenuTitle = ob.MenuTitle,
                MenuUrl = ob.MenuUrl,
                MenuOrder = ob.MenuOrder
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetAllColumn(SEC_ConfigurePage obj)
        {
            var columnList = new List<SEC_ConfigureColumn>();
            await Task.Run(() => _defalutValueSetupService.GetColumnForDefaultValue(obj, out columnList));

            var result = columnList.Select(ob => new
            {
                ConfigureColumnID = ob.ConfigureColumnID,
                AutoColumnTitle = ob.AutoColumnTitle,
                ConfigureToTable = ob.ConfigureToTable,
                ConfigureToColumn = ob.ConfigureToColumn
            }).ToList();

            return Json(new {result, Msg = ""}, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetDefaultPrimary(string menuId)
        {
            var dVS = new object();
            await Task.Run(() => _defalutValueSetupService.GetDefaultPrimary(menuId, out dVS));

            return Json(new { dVS, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<dynamic> GetDefaultValueSetup(SEC_DefaultValue obj)
        {
            var result = new List<SEC_DefaultValue>();
            await Task.Run(() => _defalutValueSetupService.GetDefaultValueSetup(obj, out result));

            return Json(new {result, Msg = ""}, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> AddDefaultValue(SEC_DefaultValue obj)
        {
            if (ModelState.IsValid)
            {
                _action = "add";
                obj.SetBy = _userId;
                _respStatus = await Task.Run(() => _defalutValueSetupService.ManipulateDefaultValues(obj, _action, out _outStatus));

                return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
            }

            _respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));

            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> EditDefaultValue(SEC_DefaultValue obj)
        {
            if (ModelState.IsValid)
            {
                _action = "edit";
                obj.ModifiedBy = _userId;
                _respStatus = await Task.Run(() => _defalutValueSetupService.ManipulateDefaultValues(obj, _action, out _outStatus));

                return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
            }

            _respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));

            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
        }

    }
}