using SILDMS.Model.WorkflowModule;
using SILDMS.Service.MasterManager;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Areas.WorkflowModule.Controllers
{
    public class MasterManageController : Controller
    {
        private readonly string UserID = string.Empty;
        private IMasterManagerService masterManagerService;
        private string res_code = string.Empty;
        private string res_message = string.Empty;
        public MasterManageController(IMasterManagerService _masterManagerService)
        {
            masterManagerService = _masterManagerService;
            UserID = SILAuthorization.GetUserID();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }

        public async Task<dynamic> GetMasterForTableType()
        {
            var prm = new List<MasterTableTracker>();
            await Task.Run(() => masterManagerService.GetMasterForTableType(out prm));

            return Json(new { prm, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<dynamic> GetMasterDataByTable(int id)
        {
            var Objs = new List<MasterData>();
            await Task.Run(() => masterManagerService.GetMasterDataByTable(id, UserID, out Objs));

            return Json(new { Objs, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<dynamic> AddItemToMaster(MasterData obj, int masterId, string action)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(() => masterManagerService.AddItemToMaster(action, obj, masterId, UserID, out res_code, out res_message));
                return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
        }

    }
}