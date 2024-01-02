using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SILDMS.Model.DocScanningModule;
using SILDMS.Service.AutoValueSetup;
using SILDMS.Service.DocProperty;
using SILDMS.Service.MultiDocScan;
using SILDMS.Service.OriginalDocSearching;
using SILDMS.Service.OwnerProperIdentity;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class DocModificationController : Controller
    {
        private ValidationResult respStatus = new ValidationResult();
        private readonly string UserID = string.Empty;
        private string action = "";

        private readonly IMultiDocScanService _multiDocScanService;
        private readonly IOwnerProperIdentityService _ownerProperIdentityService;
        private readonly IAutoValueSetupService _autoValueSetupService;

        private readonly IDocPropertyService _docPropertyService;
        private readonly ILocalizationService _localizationService;
        private readonly IOriginalDocSearchingService _originalDocSearchingService;

        public DocModificationController(IOriginalDocSearchingService originalDocSearchingService, IMultiDocScanService multiDocScanService, IOwnerProperIdentityService ownerProperIdentityRepository,
            IDocPropertyService docPropertyService,IAutoValueSetupService autoValueSetupService, ILocalizationService localizationService)
        {
            _originalDocSearchingService = originalDocSearchingService;
            _multiDocScanService = multiDocScanService;
            _ownerProperIdentityService = ownerProperIdentityRepository;
            _docPropertyService = docPropertyService;
            _localizationService = localizationService;
            _autoValueSetupService = autoValueSetupService;
            UserID = SILAuthorization.GetUserID();
        }

        [HttpPost]
        [Authorize]
        //[SILLogAttribute]
        public async Task<dynamic> UpdateDocumentInfo(DocumentsInfo _modelDocumentsInfo)
        {
            List<DSM_DocPropIdentify> objDocPropIdentifies = null;

            if (ModelState.IsValid)
            {
                action = "add";
                _modelDocumentsInfo.SetBy = UserID;
                _modelDocumentsInfo.ModifiedBy = _modelDocumentsInfo.SetBy;
                _modelDocumentsInfo.UploaderIP = GetIPAddress.LocalIPAddress();

                respStatus.Message = "Success";
                respStatus = await Task.Run(() => _multiDocScanService.UpdateDocumentInfo(_modelDocumentsInfo, action, out objDocPropIdentifies));

                foreach (var item in objDocPropIdentifies)
                {
                    try
                    {
                        FolderGenerator.MakeFTPDir(objDocPropIdentifies.FirstOrDefault().ServerIP, objDocPropIdentifies.FirstOrDefault().ServerPort, item.FileServerUrl, objDocPropIdentifies.FirstOrDefault().FtpUserName, objDocPropIdentifies.FirstOrDefault().FtpPassword);
                    }
                    catch (Exception e)
                    {

                    }
                }

                //ViewBag.LoggID = _modelDocumentsInfo.DocumentID;
                //ViewBag.LoggResult = "";
                //ViewBag.LoggAction = "Update Document";
                //ViewBag.LookupTable = "DSM_Documents";

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
            List<DSM_DocPropIdentify> docPropIdentifies = null;
            await Task.Run(() => _ownerProperIdentityService.GetDocPropIdentify(UserID, "", out docPropIdentifies));

            var result =   (from r in docPropIdentifies
                            where r.OwnerID == _OwnerID & r.DocCategoryID == _DocCategoryID & r.DocTypeID == _DocTypeID & r.DocPropertyID == _DocPropertyID & r.Status == 1
                            select new
                            {
                                r.DocPropIdentifyID,
                                r.IdentificationAttribute,
                            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> DeleteFtpDocuments(string serverIP, string uri, string userName, string password)
        {
            var status = FolderGenerator.DeleteFileFromFTP(serverIP, uri, userName, password);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

	}
}