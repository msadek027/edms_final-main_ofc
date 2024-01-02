using SILDMS.Model.SecurityModule;
using SILDMS.Service.AutoValueConf;
using SILDMS.Service.Menu;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SILDMS.Utillity;

namespace SILDMS.Web.UI.Areas.SecurityModule.Controllers
{
    public class AutoValueConfController : Controller
    {
        readonly IMenuService _menuService;
        readonly IAutoValueConfService _confService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult _respStatus;
        private string _outStatus;
        private string _action;
        private readonly string _userId;

        public AutoValueConfController(IMenuService repository,IAutoValueConfService confService, ILocalizationService localizationService, ValidationResult repStatus)
        {
            _confService = confService;
            _menuService = repository;
            _localizationService = localizationService;
            _respStatus = repStatus;
            _userId = SILAuthorization.GetUserID();
        }

        // GET: /SecurityModule/AutoValueConf/
        //[SILAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<dynamic> GetDbTables()
        {
            var lstTbl = new List<SysDbTables>();
            await Task.Run(() => _confService.GetDbTables("", out lstTbl));
            var result = lstTbl.OrderBy(ob => ob.TableName).Select(ob => ob.TableName).Distinct();

            return Json(new { Msg = "", result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<dynamic> GetTablesFields(string tbl)
        {
            var lstTbl = new List<SysDbTables>();
            await Task.Run(() => _confService.GetDbTables(tbl, out lstTbl));
            var result = lstTbl.OrderBy(ob => ob.ColumnName).Select(ob => ob.ColumnName).Distinct();
            var key = lstTbl.Where(ob => ob.Position == "1" && ob.IsNull == "NO").Select(ob => ob.ColumnName);

            return Json(new { Msg = "", result, key }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<dynamic> GetMenu()
        {
            var obMenu = new List<SEC_Menu>();
            await Task.Run(() => _menuService.GetMenu("", "", "", out obMenu));

            var result = obMenu.Select(x => new
            {
                x.MenuID,
                x.MenuTitle,
                x.MenuUrl,
                x.ParentMenuID,
                MenuParentTitle = (x.ParentMenuID != "" ? ((from t in obMenu where t.MenuID == x.ParentMenuID select t.MenuTitle).FirstOrDefault()) : "Root"),
                x.MenuIcon,
                x.MenuOrder,
                x.Status
            }).Where(ob => ob.MenuUrl != "#").OrderBy(ob => ob.MenuTitle);

            return Json(new { Msg = "", result }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetAutoValues(SEC_ConfigurePage obj)
        {
            var autoValues = new List<SEC_ConfigureColumn>();
            await Task.Run(() => _confService.GetAutoValue(obj, out autoValues));

            var result = autoValues.Select(x => new SEC_ConfigureColumn
            {
                ConfigureColumnID = x.ConfigureColumnID,
                ConfigureID = x.ConfigureID,
                ConfigureCategory = x.ConfigureCategory,
                ConfigurationFor = x.ConfigurationFor,
                ConfigureToTable = x.ConfigureToTable,
                ConfigureToColumn = x.ConfigureToColumn,
                AutoColumnTitle = x.AutoColumnTitle,
                AutoValueGroupID = x.AutoValueGroupID,
                AutoValueGroup = x.AutoValueGroup,
                CssClass = x.CssClass,
                PrimaryKeyColumn = x.PrimaryKeyColumn,
                Remarks = x.Remarks,
                Status = x.Status
            }).ToList();

            return Json(new { Msg = "", result }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> AddAutoValue(SEC_ConfigureColumn obj)
        {
            if (ModelState.IsValid)
            {
                _action = "add";
                obj.SetBy = _userId;
                _respStatus = await Task.Run(() => _confService.ManipulateAutoValue(obj, _action, out _outStatus));

                return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
            }

            _respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));

            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> EditAutoValue(SEC_ConfigureColumn obj)
        {
            if (ModelState.IsValid)
            {
                _action = "edit";
                obj.ModifiedBy = _userId;
                _respStatus = await Task.Run(() => _confService.ManipulateAutoValue(obj, _action, out _outStatus));

                return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
            }

            _respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));

            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
        }

        public string UserID { get; set; }
    }
}