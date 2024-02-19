using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using SILDMS.Model.SecurityModule;
using SILDMS.Service.Departments;
using SILDMS.Service.Users;
using SILDMS.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SILDMS.Service.Roles;
using Microsoft.AspNet.Identity.Owin;
using SILDMS.Service.Menu;
using System.Web.UI.HtmlControls;
using SILDMS.Service.Dashboard;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System.Web.UI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Configuration;
using SILDMS.Service.RoleSetup;

namespace SILDMS.Web.UI.Areas.SecurityModule.Controllers
{
    public class AccountController : Controller
    {
        readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;
        private readonly IDashboardService _dashboardService;
        private readonly string _userId;
        private readonly IRoleSetupService _roleSetupService;
        private readonly string _adminPassword;
        public AccountController(IUserService repository, IDepartmentService dept, IRoleService role, IMenuService menuService, IRoleSetupService _roleSetupService, IDashboardService dashboardService)
        {          
            this._userService = repository;        
            _roleService = role;
            _menuService = menuService;
            _dashboardService = dashboardService;
            _userId = SILAuthorization.GetUserID();
            this._roleSetupService = _roleSetupService;
             //Password for production
             //_adminPassword = "h3DHpT8VicTY1KN6JD+lFA==";
             //Password for Development
             _adminPassword = WebConfigurationManager.AppSettings["SystemUsr"].ToString();
        }

        [Authorize]
        [OutputCache(Duration = 2000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult Dashboard()
        {
            return View();
        }
     
        [OutputCache(Duration = 2000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult Login(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }

        [SILAuthorize]
        [SILLogAttribute]
        [OutputCache(Duration = 2000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult Register()
        {
            ViewBag.LoggID ="";
            ViewBag.LoggResult = "";
            ViewBag.LoggAction = "Rigister";
            ViewBag.LookupTable = "";
            return View("Register");
        }

        [HttpPost]
        //[SILLogAttribute]
        public async Task<string> SetAuthorization(string user, string password, string isRemeber)
        {
            List<GetUserAccessPermission_Result> permissionMenu = null;
            SEC_Role role = null;
            string returnUrl = TempData["ReturnUrl"] == null ? "/SecurityModule/Account/Dashboard" : TempData["ReturnUrl"].ToString();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                return "";
            }

            IpValidationInfo info = await Task.Run(() => _userService.GetIpValidationInfo(StringEncription.RemoveSpecialCharacters(user.Trim()), Convert.ToString(this.Request.UserHostAddress)));
            //if (!info.IsIpValid)
            //{
            //    return "E402";
            //}

            if (user.ToLower() == "superadmin" && StringEncription.Encrypt(password, true) != _adminPassword)
            {
                return "E403";
            }

            bool result = await Task.Run(() => _userService.IsValidUser(StringEncription.RemoveSpecialCharacters(user.Trim()), StringEncription.Encrypt(password.Trim(), true), Convert.ToString(this.Request.UserHostAddress), out permissionMenu));
            if (result)
            {
               
                string roleId= (from temp in permissionMenu where temp.RoleID != "" select temp.RoleID).FirstOrDefault().ToString();
                var ident = new ClaimsIdentity(
                new[] 
                { 
                    new Claim(ClaimTypes.Role,permissionMenu[0].RoleTitle),
                    new Claim(ClaimTypes.Name,(from temp in permissionMenu where temp.UserID != "" select temp.UserID).FirstOrDefault())
                }, DefaultAuthenticationTypes.ApplicationCookie);

                Session["UserID"] = (from temp in permissionMenu where temp.UserID != "" select temp.UserID).FirstOrDefault();  // permissionMenu[0].UserID.ToString().Trim();
                Session["User"] = (from temp in permissionMenu where temp.UserName != "" select temp.UserName).FirstOrDefault();
                Session["RoleTitle"] = (from temp in permissionMenu where temp.RoleTitle != "" select temp.RoleTitle).FirstOrDefault();
                Session["OwnerLevelID"] = (from temp in permissionMenu where temp.OwnerLevelID != "" select temp.OwnerLevelID).FirstOrDefault();
                Session["OwnerID"] = (from temp in permissionMenu where temp.OwnerID != "" select temp.OwnerID).FirstOrDefault();
                Session["SEC_Menu"] = (from temp in permissionMenu[0].AccessMenu
                                       select new SEC_Menu
                                       {
                                           MenuID = temp.MenuID,
                                           MenuTitle = temp.MenuTitle,
                                           ParentMenuID = temp.ParentMenuID,
                                           ParentMenu = temp.ParentMenu,
                                           MenuUrl = temp.MenuUrl,
                                           MenuIcon = temp.MenuIcon,
                                           MenuOrder = temp.MenuOrder,
                                           PermissionClass = temp.PermissionClass
                                       }).ToList();
                Session["PasswordReset"] =-1;
                _roleSetupService.GetRoleByRoleId(roleId, out role);
                if (role.PassResetInDays > 0)
                {
                    int remainingDays = ((from temp in permissionMenu where temp.UserID != "" select temp.PasswordResetDate).FirstOrDefault().AddDays(role.PassResetInDays) -DateTime.Now.Date ).Days;

                    if (remainingDays > 0 && remainingDays<6)
                    {
                        //notification 
                        Session["PasswordReset"] = remainingDays;
                    }
                    else if (remainingDays < 1)
                    {
                        //password change 
                        Session["PasswordReset"] = 0;
                    }

                }              
                HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
            }
            else
            {
                return "E401";
            }


            ViewBag.LoggID = Session["UserID"].ToString();
            ViewBag.LoggResult = "";
            ViewBag.LoggAction = "Login";
            ViewBag.LookupTable = "SEC_User";

            return returnUrl;
        }
        
        [HttpGet]
        [Authorize]
       // [SILLogAttribute]
        [ActionName("Logout")]        
        public async Task<ActionResult> Logout()
        {
            Session.Clear();
            var ctx = await Task.Run(() => Request.GetOwinContext());
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
          
            return RedirectToLocal("/SecurityModule/Account/Login");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {                
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<dynamic> GetAllRolDrpDwn()
        {
            List<AspNetRole> role = null;
            await Task.Run(() => _roleService.GetAllRoles("0", out role));
            var result = role.Select(x => new
            {
                x.Id,
                x.Name
            });

            return Json(result , JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        [OutputCache(Duration = 2000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public string GetDynamicMenu(string url)
        {           
            List<SEC_Menu> lstMenu = (List<SEC_Menu>)Session["SEC_Menu"];
            if (lstMenu == null)
            {
                return null;
            }
            return _menuService.GenerateMenu(lstMenu).ToString();      
        }

        [HttpGet]
        [Authorize]
        public async Task<string> GetActionPermission(string url)
        {
            string _actions = "";
            List<SEC_Menu> _menu = (List<SEC_Menu>)Session["SEC_Menu"];
            string _parentMenuID = _menu.Where(ob => ob.MenuUrl.ToLower() == url.ToLower()).FirstOrDefault().MenuID;

            if (!string.IsNullOrEmpty(_parentMenuID))
            {
                var _action = await Task.Run(() => _menu.Where(ob => ob.PermissionClass != "" && ob.ParentMenuID == _parentMenuID));

                foreach (var item in _action)
                {
                    _actions += item.PermissionClass + "|";
                }
            }

            return _actions;
        }
        
        public async Task<JsonResult> GetDashboardInfo(string dateFrom, string dateTo)
        {
            var dashboardInfo = await Task.Run(() => _dashboardService.GetDashboardInfo(_userId, string.IsNullOrEmpty(dateFrom) ? (DateTime?)null : DataValidation.DateTimeConversion(dateFrom), string.IsNullOrEmpty(dateTo)? (DateTime?) null : DataValidation.DateTimeConversion(dateTo)));

            return Json(dashboardInfo, JsonRequestBehavior.AllowGet);
        }
    }
}