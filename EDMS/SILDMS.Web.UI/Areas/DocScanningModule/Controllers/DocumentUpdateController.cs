using SILDMS.Model.DocScanningModule;
using SILDMS.Service.DocumentUpdate;
using SILDMS.Service.MultiDocScan;
using SILDMS.Service.OriginalDocSearching;
using SILDMS.Service.Server;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using SILDMS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class DocumentUpdateController : Controller
    {
        private readonly IDocumentUpdateService _documentUpdateService;
        private readonly IOriginalDocSearchingService _originalDocSearchingService;
        private readonly IServerService _serverService;
        private readonly IMultiDocScanService _multiDocScanService;
        private readonly ILocalizationService _localizationService;
        private string outStatus = string.Empty;
        private string action = "";
        private readonly string UserID = string.Empty;
        private ValidationResult respStatus = new ValidationResult();
        public DocumentUpdateController(IOriginalDocSearchingService originalDocSearchingService, IDocumentUpdateService documentUpdateService, IMultiDocScanService multiDocScanService, IServerService serverService, ILocalizationService localizationService)
        {
            _originalDocSearchingService = originalDocSearchingService;
            _documentUpdateService = documentUpdateService;
            _multiDocScanService = multiDocScanService;
            _serverService = serverService;
            _localizationService = localizationService;
            UserID = SILAuthorization.GetUserID();
        }
        //
        // GET: /DocScanningModule/DocumentUpdate/
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<dynamic> GetDocPropIdentityForSpecificDocType(string _DocumentID, string _DocDistributionID)
        {
            List<OriginalDocMeta> metaList = null;
            await Task.Run(() => _documentUpdateService.GetOriginalDocMeta(_DocumentID, _DocDistributionID, out metaList));

            return Json(metaList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
       // [SILLogAttribute]
        public async Task<dynamic> UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo)
        {
            Session["UserID"].ToString();
            respStatus = await Task.Run(() => _documentUpdateService.UpdateDocMetaInfo(_modelDocumentsInfo, UserID, out outStatus));

            //ViewBag.LoggID = _modelDocumentsInfo.DocMetaValues.Count > 0 ? _modelDocumentsInfo.DocMetaValues[0].DocumentID : ""; //_modelDocumentsInfo.DocumentID;
            //ViewBag.LoggResult = "";
            //ViewBag.LoggAction = "Update Document Attribute";
            //ViewBag.LookupTable = "DSM_Documents";

            return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
       // [SILLogAttribute]

        public async Task<dynamic> UpdateDocMetaInfoWithMailNotifyDate(DocumentsInfo _modelDocumentsInfo)
        {
            
            respStatus = await Task.Run(() => _originalDocSearchingService.UpdateDocMetaInfo(_modelDocumentsInfo, UserID, out outStatus));
            if (_modelDocumentsInfo.DocMetaValues[0].Remarks != null && _modelDocumentsInfo.DocMetaValues[0].VersionMetaValue != null)
            {
                await Task.Run(() => _originalDocSearchingService.UpdateDocMailNotifyAndExpDate(UserID, _modelDocumentsInfo.DocMetaValues[0].DocumentID, _modelDocumentsInfo.DocMetaValues[0].Remarks, _modelDocumentsInfo.DocMetaValues[0].VersionMetaValue, out outStatus));
            }
            //ViewBag.LoggID = _modelDocumentsInfo.DocMetaValues.Count > 0 ? _modelDocumentsInfo.DocMetaValues[0].DocumentID : ""; //_modelDocumentsInfo.DocumentID;
            //ViewBag.LoggResult = "";
            //ViewBag.LoggAction = "Update Document Attribute";
            //ViewBag.LookupTable = "DSM_Documents";


            return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
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
        public async Task<dynamic> DeleteFtpDocuments(string serverIP, string uri, string userName, string password)
        {
            var status = FolderGenerator.DeleteFileFromFTP(serverIP, uri, userName, password);
            return Json(status, JsonRequestBehavior.AllowGet);
        }
	}
}