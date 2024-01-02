using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.SecurityModule;
using SILDMS.Service.DataLevelPermission;
using SILDMS.Service.DocProperty;
using SILDMS.Service.DocumentCategory;
using SILDMS.Service.DocumentType;
using SILDMS.Service.Owner;
using SILDMS.Service.OwnerLevelPermission;
using SILDMS.Service.OwnerProperIdentity;
using SILDMS.Service.Users;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule.Models;

namespace SILDMS.Web.UI.Areas.SecurityModule.Controllers
{
    public class DataLevelPermissionController : Controller
    {
        private ValidationResult respStatus = new ValidationResult();
        private string outStatus = string.Empty;
        private string action = "";

        private readonly IDataLevelPermissionService _dataLevelPermissionService;
        private readonly IOwnerLevelPermissionService _OwnerLevelPermissionService;
        private readonly IOwnerService _ownerService;
        private readonly IUserService _userService;
        readonly IOwnerProperIdentityService _ownerProperIdentityService;
        readonly IDocCategoryService _docCategoryService;
        readonly IDocTypeService _docTypeService;
        readonly IDocPropertyService _docPropertyService;
        private readonly ILocalizationService _localizationService;
        private readonly string UserID = string.Empty;

        public DataLevelPermissionController(IDataLevelPermissionService dataLevelPermissionService, IOwnerLevelPermissionService ownerLevelPermissionService, IOwnerService ownerService, 
            IOwnerProperIdentityService ownerProperIdentityRepository, IDocCategoryService docCategoryService, IDocTypeService docTypeService, IDocPropertyService docPropertyService, IUserService userService, ILocalizationService localizationService)
        {
            _dataLevelPermissionService = dataLevelPermissionService;
            _OwnerLevelPermissionService = ownerLevelPermissionService;
            _ownerService = ownerService;
            _userService = userService;
            _ownerProperIdentityService = ownerProperIdentityRepository;
            _docCategoryService = docCategoryService;
            _docTypeService = docTypeService;
            _docPropertyService = docPropertyService;
            _localizationService = localizationService;
            UserID = SILAuthorization.GetUserID();
        }

        [SILAuthorize]
        public ActionResult Index()
        {
            return View();
        }
        
        [Authorize] //changed by me : software/dept: distinct
        public async Task<dynamic> GetOwnersForSelectedUser(string _UserID)
        {
            List<SEC_UserOwnerAccess> secUserOwnerAccesses = new List<SEC_UserOwnerAccess>();
            List<DSM_Owner> dsmOwners = new List<DSM_Owner>();

            await Task.Run(() => _OwnerLevelPermissionService.GetAllUserOwnerAccess(_UserID, out secUserOwnerAccesses));
            await Task.Run(() => _ownerService.GetAllOwners("", UserID, out dsmOwners));
            
            var result = (from r in secUserOwnerAccesses
                          where r.UserID == _UserID
                          from ow in dsmOwners.Where(x => x.OwnerID == r.PermittedOwnerID).DefaultIfEmpty()
                          select new ddlDSMOwner
                          {
                              OwnerID = r.PermittedOwnerID,
                              OwnerName = ow.OwnerName
                          }).ToList();


            var distinctResult = result
                      .GroupBy(item => item.OwnerID)
                      .Select(group => group.First())
                      .ToList();


            //var result = (from r in secUserOwnerAccesses
            //              where r.UserID == _UserID
            //              from ow in dsmOwners.Where(x => x.OwnerID == r.PermittedOwnerID).DefaultIfEmpty()
            //              select new ddlDSMOwner
            //              {
            //                  OwnerID = r.PermittedOwnerID,
            //                  OwnerName = ow.OwnerName
            //              })
            //  .GroupBy(o => o.OwnerID)
            //  .Select(g => g.First())
            //  .ToList();

            return Json(distinctResult, JsonRequestBehavior.AllowGet);
        }

        [Authorize] //changed by me : TimeLimit
        public async Task<dynamic> GetDocCategoriesForSpecificUser(string _OwnerID, string _UserID)
        {
            List<DSM_DocCategory> objDsmDocCategories = null;
            List<SEC_UserDataAccess> secUserDataAccesses = null;
            
            await Task.Run(() => _docCategoryService.GetDocCategories("", UserID, out objDsmDocCategories));
            await Task.Run(() => _dataLevelPermissionService.GetAllUserDataAccess(_UserID, out secUserDataAccesses));

            var result =   (from ow in objDsmDocCategories
                            where ow.OwnerID == _OwnerID & ow.Status == 1
                            from uda in secUserDataAccesses.Where(x => x.DocCategoryID == ow.DocCategoryID & x.UserID == _UserID & x.Status == 1 & x.DocTypeID == "").DefaultIfEmpty()
                            select new
                            {
                                ow.DocCategoryID,
                                ow.DocCategoryName,
                                IsSelected = (uda != null),
                                //TimeLimit = (uda == null ? null : (uda.AccessTimeLimit == "01/01/0001" ? "" : uda.AccessTimeLimit)),
                                TimeLimit = "",
                                Remarks = (uda == null ? null : uda.Remarks)
                            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]//changed by me : TimeLimit
        public async Task<dynamic> GetDocumentTypeForSpecificUser(string _DocCategoryID, string _UserID)
        {
            List<DSM_DocType> objDsmDocTypes = null;
            List<SEC_UserDataAccess> secUserDataAccesses = null;

            await Task.Run(() => _docTypeService.GetDocTypes("", UserID, out objDsmDocTypes));
            await Task.Run(() => _dataLevelPermissionService.GetAllUserDataAccess(_UserID, out secUserDataAccesses));

            var result =   (from ow in objDsmDocTypes
                            where ow.DocCategoryID == _DocCategoryID & ow.Status == 1
                            from uda in secUserDataAccesses.Where(x => x.DocTypeID == ow.DocTypeID & x.UserID == _UserID & x.Status == 1 & x.DocPropertyID == "").DefaultIfEmpty()
                            select new
                            {
                                DocTypeID = ow.DocTypeID,
                                DocTypeName = ow.DocTypeName,
                                IsSelected = (uda != null),
                               // TimeLimit = (uda == null ? null : (uda.AccessTimeLimit).ToString()),
                                TimeLimit = "",
                                Remarks = (uda == null ? null : uda.Remarks)
                            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]//changed by me : TimeLimit
        public async Task<dynamic> GetDocumentPropertyForSpecificUser(string _UserID, string _DocTypeID)
        {
            List<DSM_DocProperty> objDsmDocProperties = null;
            List<SEC_UserDataAccess> secUserDataAccesses = null;

            await Task.Run(() => _docPropertyService.GetDocProperty("", UserID, out objDsmDocProperties));
            await Task.Run(() => _dataLevelPermissionService.GetAllUserDataAccess(_UserID, out secUserDataAccesses));

            var result =   (from ow in objDsmDocProperties
                            where ow.DocTypeID == _DocTypeID & ow.Status == 1
                            from uda in secUserDataAccesses.Where(x => x.DocPropertyID == ow.DocPropertyID & x.UserID == _UserID & x.Status == 1 & x.DocPropIdentifyID == "").DefaultIfEmpty()
                            select new
                            {
                                DocPropertyID = ow.DocPropertyID,
                                DocPropertyName = ow.DocPropertyName,
                                IsSelected = (uda != null),
                               // TimeLimit = (uda == null ? null : (uda.AccessTimeLimit).ToString()),
                                TimeLimit ="",
                                Remarks = (uda == null ? null : uda.Remarks)
                            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize] //changed by me : distinct
        public async Task<dynamic> GetDocPropIdentityForSpecificUser(string _UserID, string _DocPropertyID)
        {
            List<DSM_DocPropIdentify> objDocPropIdentifies = null;
            List<SEC_UserDataAccess> secUserDataAccesses = null;

            await Task.Run(() => _ownerProperIdentityService.GetDocPropIdentify(UserID, "", out objDocPropIdentifies));
            await Task.Run(() => _dataLevelPermissionService.GetAllUserDataAccess(_UserID, out secUserDataAccesses));

            var result =   (from ow in objDocPropIdentifies
                            where ow.DocPropertyID == _DocPropertyID & ow.Status == 1
                            from uda in secUserDataAccesses.Where(x => x.DocPropIdentifyID == ow.DocPropIdentifyID & x.UserID == _UserID & x.Status == 1).DefaultIfEmpty()
                            select new
                            {
                                DocPropIdentifyID = ow.DocPropIdentifyID,
                                IdentificationAttribute = ow.IdentificationAttribute,
                                IsSelected = (uda != null),
                                //TimeLimit = (uda == null ? null : (uda.AccessTimeLimit).ToString()),
                                TimeLimit ="",
                                Remarks = (uda == null ? null : uda.Remarks),
                                MetaValue = (uda == null ? null : uda.MetaValue)
                            }).ToList();

            var distinctResult = result
                        .GroupBy(item => item.DocPropIdentifyID)
                        .Select(group => group.First())
                        .ToList();

            return Json(distinctResult, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [SILLogAttribute]
        public async Task<dynamic> SetDataLevelPermission(SEC_UserOwnerAccess model)
        {
            if (ModelState.IsValid)
            {
                action = "add";
                model.SetBy = UserID;
                model.ModifiedBy = model.SetBy;
                respStatus = await Task.Run(() => _dataLevelPermissionService.SetDataLevelPermission(model, action, out outStatus));
                ViewBag.LoggID = model.UserOwnerAccessID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "UserOwnerAccess";
                ViewBag.LookupTable = "";
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
        public async Task<dynamic> GetEmployeeListForOwner(string _OwnerID)
        {
            List<SEC_User> userList = null;
            await Task.Run(() => _userService.GetAllUser("", _OwnerID, out userList));

            var result = (from r in userList
                          where r.OwnerID == _OwnerID && r.PermissionLevel == "Data" && r.Status == "1"
                          select new
                          {
                              UserID = r.UserID,
                              EmployeeID = r.EmployeeID,
                              UserFullName = r.UserFullName,
                              UserDesignation = r.UserDesignation,
                              JobLocation = r.JobLocation,
                              UserNo = r.UserNo,
                              UserName = r.UserName,
                              UserRole = r.RoleTitle,
                              PermissionLevel = r.PermissionLevel,
                              RoleTitle = r.RoleTitle,
                              SupervisorLevel = r.SupervisorLevel,
                              UserLevel = r.UserLevelID,
                              EnableOwnerSecurity = r.SecurityStatus == "Enabled" ? "1" : "0"
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

	}
}