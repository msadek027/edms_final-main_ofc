using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SILDMS.Model.DocScanningModule;
using SILDMS.Service.AutoValueSetup;
using SILDMS.Service.OwnerProperIdentity;
using SILDMS.Service.VersioningOfOriginalDoc;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using SILDMS.Web.UI.Areas.SecurityModule;
using Newtonsoft.Json;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class VersioningOfOriginalDocController : Controller
    {
        private readonly IVersioningOfOriginalDocService _versioningOfOriginalDocService;
        readonly IOwnerProperIdentityService _DocPropIdentityService;
        private readonly IAutoValueSetupService _autoValueSetupService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult respStatus = new ValidationResult();
        private readonly string UserID = string.Empty;
        private string action = "";

        public VersioningOfOriginalDocController(ILocalizationService localizationService, IVersioningOfOriginalDocService versioningOfOriginalDocService, IOwnerProperIdentityService docPropIdentityService, IAutoValueSetupService autoValueSetupService)
        {
            UserID = SILAuthorization.GetUserID();
            _localizationService = localizationService;
            _versioningOfOriginalDocService = versioningOfOriginalDocService;
            _DocPropIdentityService = docPropIdentityService;
            _autoValueSetupService = autoValueSetupService;
        }

        [HttpPost]
        [Authorize]
       // [SILLogAttribute]
        public async Task<dynamic> AddVersionDocumentInfo(DocumentsInfo _modelDocumentsInfo)
        {
            DSM_DocPropIdentify objDocPropIdentifies = new DSM_DocPropIdentify();

            if (ModelState.IsValid)
            {
                action = "add";
                _modelDocumentsInfo.SetBy = UserID;
                _modelDocumentsInfo.ModifiedBy = _modelDocumentsInfo.SetBy;
                _modelDocumentsInfo.UploaderIP = GetIPAddress.LocalIPAddress();
                
                if (_modelDocumentsInfo.PageName == "VersionOfOriginalDoc")
                {
                    _modelDocumentsInfo.ConfigureColumnIds = _autoValueSetupService.GetConfigureColumnList
                    ("/Home/VersioningOriginalDoc", _modelDocumentsInfo.OwnerID,
                    _modelDocumentsInfo.DocCategoryID,
                    _modelDocumentsInfo.DocTypeID,
                    _modelDocumentsInfo.DocPropertyID, String.Join(",", _modelDocumentsInfo.DocMetaValues.Select(x => x.DocPropIdentifyID)));
                }
                else if (_modelDocumentsInfo.PageName == "VersionOfVersionDoc")
                {
                    _modelDocumentsInfo.ConfigureColumnIds = _autoValueSetupService.GetConfigureColumnList
                    ("/Home/VersioningVersionedDoc", _modelDocumentsInfo.OwnerID,
                    _modelDocumentsInfo.DocCategoryID,
                    _modelDocumentsInfo.DocTypeID,
                    _modelDocumentsInfo.DocPropertyID, String.Join(",", _modelDocumentsInfo.DocMetaValues.Select(x => x.DocPropIdentifyID)));
                }

                respStatus.Message = "Success";
                respStatus = await Task.Run(() => _versioningOfOriginalDocService.AddVersionDocumentInfo(_modelDocumentsInfo, action, out objDocPropIdentifies));

                try
                {
                    FolderGenerator.MakeFTPDir(objDocPropIdentifies.ServerIP, objDocPropIdentifies.ServerPort, objDocPropIdentifies.FileServerUrl, objDocPropIdentifies.FtpUserName, objDocPropIdentifies.FtpPassword);
                }
                catch (Exception e)
                {
                    
                }

                ViewBag.LoggID = _modelDocumentsInfo.DocumentID;
                ViewBag.LoggResult = "";//JsonConvert.SerializeObject(_modelDocumentsInfo).ToString();
                ViewBag.LoggAction = "Add Documents Version";
                ViewBag.LookupTable = "DSM_Documents";

                return Json(new { Message = respStatus.Message, result = objDocPropIdentifies }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var errors = ModelState.Values.SelectMany(v => v.Errors);
                respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));

                return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public async Task<dynamic> GetDocPropIdentityForSpecificDocType(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID)
        {
            List<DSM_DocPropIdentify> objDocPropIdentifies = null;

            await Task.Run(() => _DocPropIdentityService.GetDocPropIdentify(UserID, "", out objDocPropIdentifies));

            var result = (from dp in objDocPropIdentifies
                          where dp.OwnerID == _OwnerID & dp.DocCategoryID == _DocCategoryID & dp.DocTypeID == _DocTypeID & dp.DocPropertyID == _DocPropertyID & dp.Status == 1 & dp.IsRequired == 1
                          orderby dp.IdentificationSL
                          select new DSM_DocPropIdentify
                          {
                              DocPropIdentifyID = dp.DocPropIdentifyID,
                              DocPropertyID = dp.DocPropertyID,
                              DocTypeID = dp.DocTypeID,
                              DocCategoryID = dp.DocCategoryID,
                              OwnerID = dp.OwnerID,
                              IdentificationCode = dp.IdentificationCode,
                              IdentificationSL = dp.IdentificationSL,
                              IdentificationAttribute = dp.IdentificationAttribute,
                              AttributeGroup = dp.AttributeGroup,
                              IsRequired = dp.IsRequired,
                              IsAuto = dp.IsAuto,
                              IsRestricted = dp.IsRestricted,
                              Remarks = dp.Remarks,
                              Status = dp.Status
                          }).ToList();

            return Json(new { Msg = "", result }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> DeleteVersionDocumentInfo(string _DocumentIDs)
        {
            respStatus = await Task.Run(() => _versioningOfOriginalDocService.DeleteVersionDocumentInfo(_DocumentIDs));
            return Json(null, JsonRequestBehavior.AllowGet);
        }

	}
}