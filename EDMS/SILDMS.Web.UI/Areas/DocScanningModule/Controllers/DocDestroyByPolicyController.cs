using MvcSiteMapProvider.Globalization;
using Newtonsoft.Json;
using SILDMS.Model.DocScanningModule;
using SILDMS.Service.DocDestroyPolicy;
using SILDMS.Service.DocProperty;
using SILDMS.Service.DocumentCategory;
using SILDMS.Service.DocumentType;
using SILDMS.Service.MultiDocScan;
using SILDMS.Service.OriginalDocSearching;
using SILDMS.Service.OwnerProperIdentity;
using SILDMS.Service.Server;
using SILDMS.Utillity;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class DocDestroyByPolicyController : Controller
    {
        private ValidationResult respStatus = new ValidationResult();
        private string outStatus = string.Empty;
        private string action = "";
        private readonly IDocDestroyPolicyService _destroyPolicyService;
        private readonly string UserID = string.Empty;

        public DocDestroyByPolicyController(IDocDestroyPolicyService destroyPolicyService)
        {
            _destroyPolicyService = destroyPolicyService;
            UserID = SILAuthorization.GetUserID();
        }
         [Authorize]
        public ActionResult Index(string _OwnerLavelID=null,string _OwerID=null,string _DocCategoryID=null,string _DocTypeID=null,string _DocPropertyID=null,string _DestroyPolicyID=null)
        {
            return View();
        }


        [Authorize]
        public async Task<dynamic> GetDocumentsByPolicy(string _DestroyPolicyID)
        {
            List<DocSearch> lstDocSearch = null;
            await Task.Run(() => _destroyPolicyService.GetDocUsingPolicy(_DestroyPolicyID, UserID, out lstDocSearch));

            var Totalresult = (from r in lstDocSearch
                                group r by new
                                {
                                    r.DocumentID,
                                }
                                    into g

                                    select new
                                    {
                                        DocumentID = g.Key.DocumentID,
                                        FileCodeName = g.Select(o => o.FileCodeName).FirstOrDefault(),
                                        MetaValue = String.Join(", ", g.Select(o => o.MetaValue)),
                                        DocPropIdentifyID = String.Join(",", g.Select(o => o.DocPropIdentifyID).Distinct()),
                                        DocPropIdentifyName = String.Join(",", g.Select(o => o.DocPropIdentifyName).Distinct()),
                                        FileServerURL = g.Select(o => o.FileServerURL).FirstOrDefault(),
                                        ServerIP = g.Select(o => o.ServerIP).FirstOrDefault(),
                                        ServerPort = g.Select(o => o.ServerPort).FirstOrDefault(),
                                        FtpUserName = g.Select(o => o.FtpUserName).FirstOrDefault(),
                                        FtpPassword = g.Select(o => o.FtpPassword).FirstOrDefault(),
                                        DocDistributionID = g.Select(o => o.DocDistributionID).FirstOrDefault(),
                                        OwnerID = g.Select(o => o.OwnerID).FirstOrDefault(),
                                        DocCategoryID = g.Select(o => o.DocCategoryID).FirstOrDefault(),
                                        DocTypeID = g.Select(o => o.DocTypeID).FirstOrDefault(),
                                        DocPropertyID = g.Select(o => o.DocPropertyID).FirstOrDefault(),
                                        DocPropertyName = g.Select(o => o.DocPropertyName).FirstOrDefault(),
                                        SetOn = g.Select(o => o.SetOn).FirstOrDefault(),
                                        DeleteOn =g.Select(o => o.DeleteOn).FirstOrDefault()
                                    }).ToList();

            var DocumentNature = lstDocSearch.First().DocumentNature;
            return new JsonDotNetResult(new { Totalresult, DocumentNature});
        }

        [Authorize]
        public async Task<dynamic> SaveImportantDocuments(List<DocSearch> Documents, string _DestroyPolicyID)
        {
            respStatus=await Task.Run(() => _destroyPolicyService.SaveImportantDocuments(String.Join(",",Documents.Select(o=>o.DocumentID)), _DestroyPolicyID, UserID, out outStatus));
            return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> DeleteDocuments(List<DocSearch> Documents, string _DestroyPolicyID, string _DocumentNature)
        {
            List<string> DeletedIds = new List<string>();
            foreach (var item in Documents)
            {
                if (DeleteFile(item.ServerIP, item.ServerPort, item.FtpUserName, item.FtpPassword, item.FileServerURL, item.DocumentID,_DocumentNature))
                {
                    DeletedIds.Add(item.DocumentID);
                }
            }


            respStatus = await Task.Run(() => _destroyPolicyService.DeleteDocuments(String.Join(",",DeletedIds), _DestroyPolicyID, UserID, out outStatus));
            return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
        }

        public class JsonDotNetResult : ActionResult
        {
            private object _obj { get; set; }
            public JsonDotNetResult(object obj)
            {
                _obj = obj;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                context.HttpContext.Response.AddHeader("content-type", "application/json");
                context.HttpContext.Response.Write(JsonConvert.SerializeObject(_obj));
            }
        }

        private bool DeleteFile(string ftpIp, string port, string username, string password, string fileServerUrl, string _DocumentID, string _DocNature, string _VersionNo = "")
        {
            string documentName;
            if (_DocNature == "Version")
            {
                documentName = _DocumentID + "_v_" + _VersionNo + ".pdf";
            }
            else
                documentName = _DocumentID + ".pdf";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpIp + "/" + fileServerUrl + "/" + documentName);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Proxy = null;

            request.Credentials = new NetworkCredential(username, password);

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
        }
	}
}