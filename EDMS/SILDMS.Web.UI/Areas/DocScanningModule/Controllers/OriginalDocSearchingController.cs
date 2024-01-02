using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.SecurityModule;
using SILDMS.Service.DocProperty;
using SILDMS.Service.MultiDocScan;
using SILDMS.Service.OriginalDocSearching;
using SILDMS.Service.OwnerProperIdentity;
using SILDMS.Service.Server;
using SILDMS.Utillity;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using WebGrease.Css.Extensions;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Models;
using System.Net;
using System.IO;
using System.Web;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Newtonsoft.Json;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class OriginalDocSearchingController : Controller
    {
        private readonly IMultiDocScanService _multiDocScanService;
        private readonly IOwnerProperIdentityService _ownerProperIdentityService;
        private readonly IOriginalDocSearchingService _originalDocSearchingService;
        private readonly IServerService _serverService;
        private readonly IDocPropertyService _docPropertyService;
        private ValidationResult respStatus = new ValidationResult();

        private string outStatus = string.Empty;
        private readonly string UserID = string.Empty;
        public OriginalDocSearchingController(IOriginalDocSearchingService originalDocSearchingService, IMultiDocScanService multiDocScanService, 
            IOwnerProperIdentityService ownerProperIdentityRepository, IDocPropertyService docPropertyService, IServerService serverService)
        {
            _originalDocSearchingService = originalDocSearchingService;
            _multiDocScanService = multiDocScanService;
            _ownerProperIdentityService = ownerProperIdentityRepository;
            _docPropertyService = docPropertyService;
            _serverService = serverService;
            UserID = SILAuthorization.GetUserID();
        }
        
        //[Authorize]
        //[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        //public async Task<dynamic> GetDocPropIdentityForSelectedDocTypes(string _OwnerID, string _DocCategoryID,
        //     string _DocTypeID, string _DocPropertyID, string _SearchBy,int page,int size)
        //{

        //    List<DocSearch> lstDocSearch = null;
        //    if (!string.IsNullOrEmpty(_OwnerID) && !string.IsNullOrEmpty(_DocCategoryID) && !string.IsNullOrEmpty(_DocTypeID) && !string.IsNullOrEmpty(_DocPropertyID) && !string.IsNullOrEmpty(_SearchBy))
        //    {
        //       // await Task.Run(() => _originalDocSearchingService.GetOriginalDocBySearchParam(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, UserID, out lstDocSearch));
        //    }

        //    var Totalresult = (from r in lstDocSearch
        //        group r by new
        //        {
        //            r.DocumentID,
        //            r.DocDistributionID
        //        }
        //        into g
        //        select new
        //        {
        //            DocumentID = g.Key.DocumentID,
        //            FileCodeName = g.Select(o => o.FileCodeName).FirstOrDefault(),                 
        //            DocPropIdentifyID = String.Join(",", g.Select(o => o.DocPropIdentifyID).Distinct()),
        //            DocPropIdentifyName = String.Join(",", g.Select(o => o.DocPropIdentifyName).Distinct()),
        //            MetaValue = SILDMS.Utillity.DMSUtility.IdentifyPropertySeparator(String.Join(", ", g.Select(o => o.MetaValue)),g.Select(o=>o.DocPropIdentifyID).Distinct().Count()),
        //            FileServerURL = g.Select(o => o.FileServerURL).FirstOrDefault(),
        //            ServerIP = g.Select(o => o.ServerIP).FirstOrDefault(),
        //            ServerPort = g.Select(o => o.ServerPort).FirstOrDefault(),
        //            FtpUserName = g.Select(o => o.FtpUserName).FirstOrDefault(),
        //            FtpPassword = g.Select(o => o.FtpPassword).FirstOrDefault(),
        //            DocDistributionID = g.Select(o => o.DocDistributionID).FirstOrDefault(),

        //            OwnerID = g.Select(o => o.OwnerID).FirstOrDefault(),
        //            DocCategoryID = g.Select(o => o.DocCategoryID).FirstOrDefault(),
        //            DocTypeID = g.Select(o => o.DocTypeID).FirstOrDefault(),
        //            DocPropertyID = g.Select(o => o.DocPropertyID).FirstOrDefault(),
        //            DocPropertyName = g.Select(o => o.DocPropertyName).FirstOrDefault()
        //        }).ToList();

        //    var totalPages = Totalresult.Count();

        //    var content = Totalresult.Skip((page - 1) * size).Take(size);

        //    return Json(new { content, totalPages },
        //        JsonRequestBehavior.AllowGet);
        //}

        //[Authorize]
        //[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        //public async Task<dynamic> GetDocPropIdentityForSelectedDocTypesForVersion(string _OwnerID, string _DocCategoryID,
        //     string _DocTypeID, string _DocPropertyID, string _SearchBy,int page,int size)
        //{
        //    List<DocSearch> lstDocSearch = null;
        //    if (!string.IsNullOrEmpty(_OwnerID) && !string.IsNullOrEmpty(_DocCategoryID) && !string.IsNullOrEmpty(_DocTypeID) && !string.IsNullOrEmpty(_DocPropertyID) && !string.IsNullOrEmpty(_SearchBy))
        //    {
        //       // await Task.Run(() => _originalDocSearchingService.GetOriginalDocBySearchParam(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, UserID,page,null,null,null,null, out lstDocSearch));
        //    }

        //    var result = (from r in lstDocSearch
        //                       group r by new
        //                       {
        //                           r.DocumentID,
        //                           r.DocDistributionID
        //                       }
        //                           into g
        //                           select new
        //                           {
        //                               DocumentID = g.Key.DocumentID,
        //                               FileCodeName = g.Select(o => o.FileCodeName).FirstOrDefault(),
        //                               MetaValue = String.Join(", ", g.Select(o => o.MetaValue)),
        //                               DocPropIdentifyID = String.Join(",", g.Select(o => o.DocPropIdentifyID).Distinct()),
        //                               DocPropIdentifyName = String.Join(",", g.Select(o => o.DocPropIdentifyName).Distinct()),
        //                               FileServerURL = g.Select(o => o.FileServerURL).FirstOrDefault(),
        //                               ServerIP = g.Select(o => o.ServerIP).FirstOrDefault(),
        //                               ServerPort = g.Select(o => o.ServerPort).FirstOrDefault(),
        //                               FtpUserName = g.Select(o => o.FtpUserName).FirstOrDefault(),
        //                               FtpPassword = g.Select(o => o.FtpPassword).FirstOrDefault(),
        //                               DocDistributionID = g.Select(o => o.DocDistributionID).FirstOrDefault(),

        //                               OwnerID = g.Select(o => o.OwnerID).FirstOrDefault(),
        //                               DocCategoryID = g.Select(o => o.DocCategoryID).FirstOrDefault(),
        //                               DocTypeID = g.Select(o => o.DocTypeID).FirstOrDefault(),
        //                               DocPropertyID = g.Select(o => o.DocPropertyID).FirstOrDefault(),
        //                               DocPropertyName = g.Select(o => o.DocPropertyName).FirstOrDefault()
        //                           }).ToList();

        //    var totalPages = result.Count();

        //    var content = result.Skip((page - 1) * size).Take(size);

        //    return Json(new { content, totalPages },
        //        JsonRequestBehavior.AllowGet);
        //}

        //[SILLogAttribute]
        public async Task<dynamic> UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo)
        {
            Session["UserID"].ToString();
            respStatus = await Task.Run(() => _originalDocSearchingService.UpdateDocMetaInfo(_modelDocumentsInfo, UserID, out outStatus));

            ViewBag.LoggID = _modelDocumentsInfo.DocMetaValues.Count > 0 ? _modelDocumentsInfo.DocMetaValues[0].DocumentID : ""; //_modelDocumentsInfo.DocumentID;
            ViewBag.LoggResult = "";
            ViewBag.LoggAction = "Update Document Attribute";
            ViewBag.LookupTable = "DSM_Documents";

            return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
        }
       // [SILLogAttribute]
        public async Task<dynamic> UpdateDocMetaInfoWithMailNotifyDate(DocumentsInfo _modelDocumentsInfo)
        {
            Session["UserID"].ToString();
            respStatus = await Task.Run(() => _originalDocSearchingService.UpdateDocMetaInfo(_modelDocumentsInfo, UserID, out outStatus));
            if (_modelDocumentsInfo.DocMetaValues[0].Remarks != null && _modelDocumentsInfo.DocMetaValues[0].VersionMetaValue != null)
            {
                await Task.Run(() => _originalDocSearchingService.UpdateDocMailNotifyAndExpDate(UserID,_modelDocumentsInfo.DocMetaValues[0].DocumentID, _modelDocumentsInfo.DocMetaValues[0].Remarks, _modelDocumentsInfo.DocMetaValues[0].VersionMetaValue, out outStatus));
            }
            ViewBag.LoggID = _modelDocumentsInfo.DocMetaValues.Count > 0 ? _modelDocumentsInfo.DocMetaValues[0].DocumentID : ""; //_modelDocumentsInfo.DocumentID;
            ViewBag.LoggResult = "";
            ViewBag.LoggAction = "Update Document Attribute";
            ViewBag.LookupTable = "DSM_Documents";


            return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
        }
       // [SILLogAttribute]
        public async Task<dynamic> DeleteDocument(string _DocumentID, string _DocDistributionID, string _DocumentType)
        {
            Session["UserID"].ToString();
            respStatus = await Task.Run(() => _originalDocSearchingService.DeleteDocument(_DocumentID, _DocDistributionID, _DocumentType, UserID, out outStatus));

            ViewBag.LoggID = _DocumentID;
            ViewBag.LoggResult = "";
            ViewBag.LoggAction = "Delete Document";
            ViewBag.LookupTable = "DSM_Documents";

            return Json(respStatus, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetDocPropIdentityForSpecificDocType(string _DocumentID,string _DocDistributionID)
        {
            List<OriginalDocMeta> metaList = null;
            await Task.Run(() => _originalDocSearchingService.GetOriginalDocMeta(_DocumentID,_DocDistributionID, out metaList));

            return Json(metaList, JsonRequestBehavior.AllowGet);
        }

     
        [Authorize]
        public async Task<dynamic> GetDocumentsBySearchParam(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, 
            string _SearchBy,int page = 1, int itemsPerPage = 5, string sortBy = "", bool reverse = false, string attribute=null, string search = null)
        {
            List<DocSearch> lstDocSearch = null;
            respStatus = await Task.Run(() => _originalDocSearchingService.GetOriginalDocBySearchParam(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, UserID,page,itemsPerPage,sortBy,reverse,attribute,search, out lstDocSearch));
            
            var totalPages = lstDocSearch.Select(o => o.TotalCount).FirstOrDefault();

            return Json(new { respStatus, lstDocSearch, totalPages }, JsonRequestBehavior.AllowGet);
        }

       
        [Authorize]
        public async Task<dynamic> GetDocumentsBySearchParamV2(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, 
            string _SearchBy, int page = 1, int itemsPerPage = 5, string sortBy = "", bool reverse = false, string attribute = null, string search = null, string searchPrevious = "", string searchType = null)
        {
            List<dynamic> lstDocSearch = new List<dynamic>();
            List<string> lstAttributeSearch = new List<string>();

            var pageCurrent = 1;
            if (String.IsNullOrEmpty(search) && String.IsNullOrEmpty(searchPrevious) && search == searchPrevious)
            {
                pageCurrent = page;
            }

            var attributeCount = DataHelper.DataCount(DataHelper.GetData(DataHelper.GenerateQueryForDocumentPropertyAttributeData(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID)));
            attributeCount = attributeCount > 1 ? attributeCount : 1;
            
            await Task.Run(() => _originalDocSearchingService.GetOriginalDocBySearchParamV2(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, searchType, out lstDocSearch, out lstAttributeSearch));
            
            var itemsTotal = lstDocSearch.Select(o => o.TotalCount).FirstOrDefault();
            var rowsTotal = (itemsTotal > 0 && itemsTotal > attributeCount) ? itemsTotal / attributeCount : 1;

            int pagesTotal = 1;
            if (attributeCount != 0 && rowsTotal != 0 && !(attributeCount > rowsTotal))
            {
                //pagesTotal = ((rowsTotal - 1) + itemsPerPage) / itemsPerPage;
                pagesTotal = (((rowsTotal - 1) / itemsPerPage) + 1); /*this will always get at least 1 page, even for 0 count*/
            }

            lstDocSearch = lstDocSearch.AsEnumerable().Skip(page >= 2 ? ((page - 1) * itemsPerPage) : 0).Take(itemsPerPage).ToList();

            return Json(new {lstDocSearch, lstAttributeSearch, pageCurrent, itemsPerPage, rowsTotal, search, searchPrevious, searchType}, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetDocumentsBySearchParamForVersion(string _OwnerID, string _DocCategoryID,string _DocTypeID, string _DocPropertyID, 
            string _SearchBy, int page = 1, int itemsPerPage = 5, string sortBy = "", bool reverse = false, string attribute = null, string search = null)
        {
            List<DocSearch> lstDocSearch = null;

            respStatus = await Task.Run(() => _originalDocSearchingService.GetDocumentsBySearchParamForVersion(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out lstDocSearch));
            bool mail = _docPropertyService.GetMailNotificationStatusForindividualDocProperty(_DocPropertyID);
            var totalPages = lstDocSearch.Select(o => o.TotalCount).FirstOrDefault();

            return Json(new { respStatus, lstDocSearch, totalPages, mail }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<dynamic> Marge(string _OwnerID, string _DocCategoryID,
             string _DocTypeID, string _DocPropertyID, string _SearchBy, int page = 1, int itemsPerPage = 5, string sortBy = "", bool reverse = false, string attribute = null, string search = null)
        {
            List<DocSearch> docList = new List<DocSearch>();
            await Task.Run(() => _originalDocSearchingService.GetOriginalDocBySearchParam(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out docList));

            if (docList.Count > 0)
            {
                string filePath;
                string trnsfrpth;
                string assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

                if (!Directory.Exists(Server.MapPath("~/Buffer")))
                {
                    string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string folderName = Path.Combine(projectPath, Server.MapPath("~/Buffer"));
                    Directory.CreateDirectory(folderName);
                }

                if (!Directory.Exists(Server.MapPath("~/Buffer/bufferPdf")))
                {
                    string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string folderName = Path.Combine(projectPath, Server.MapPath("~/Buffer/bufferPdf"));
                    Directory.CreateDirectory(folderName);
                }

                if (!Directory.Exists(Server.MapPath("~/Buffer/inputPdf")))
                {
                    string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string folderName = Path.Combine(projectPath, Server.MapPath("~/Buffer/inputPdf"));
                    Directory.CreateDirectory(folderName);
                }

                using (PdfDocument outPdf = new PdfDocument())
                {
                    foreach (var doc in docList)
                    {
                        try
                        {
                            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + doc.ServerIP + "/");
                            ftpRequest.Credentials = new NetworkCredential(doc.FtpUserName, doc.FtpPassword);
  
                            filePath = "ftp://" + doc.ServerIP + "/" + doc.FileServerURL + "/" + doc.DocumentID + ".pdf";

                            bool exist = CheckIfFileExistsOnServer(filePath, doc.FtpUserName, doc.FtpPassword);
                            if (exist)
                            {
                                using (WebClient ftpClient = new WebClient())
                                {
                                    ftpClient.Credentials = new NetworkCredential(doc.FtpUserName, doc.FtpPassword);
                                    string path = filePath;
                                    trnsfrpth = Server.MapPath("~/Buffer/bufferPdf/" + doc.DocumentID + ".pdf");
                                    ftpClient.DownloadFile(path, trnsfrpth);
                                }
                                using (PdfDocument temp = PdfReader.Open(trnsfrpth, PdfDocumentOpenMode.Import))
                                {
                                    CopyPages(temp, outPdf);
                                }
                            }
                            else
                            {
                                // Failed
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                    }
                    if (Directory.GetFiles(Server.MapPath("~/Buffer/bufferPdf/")).Length != 0)
                    {
                        if (Directory.GetFiles(Server.MapPath("~/Buffer/inputPdf/")).Length != 0)
                        {
                            Array.ForEach(Directory.GetFiles(Server.MapPath("~/Buffer/inputPdf/")), System.IO.File.Delete);
                            outPdf.Save(Server.MapPath("~/Buffer/inputPdf/doc.pdf"));
                            Array.ForEach(Directory.GetFiles(Server.MapPath("~/Buffer/bufferPdf/")), System.IO.File.Delete);
                        }
                        else
                        {
                            outPdf.Save(Server.MapPath("~/Buffer/inputPdf/doc.pdf"));
                            Array.ForEach(Directory.GetFiles(Server.MapPath("~/Buffer/bufferPdf/")), System.IO.File.Delete);
                        }
                    }
                    else
                    {
                        return Json(new { Message = "Document Not Found" }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { Message = "Document Merge Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "Document Not Found" }, JsonRequestBehavior.AllowGet);
            } 
        }

        [Authorize]
        private bool CheckIfFileExistsOnServer(string filePath, string username, string password)
        {
            var request = (FtpWebRequest)WebRequest.Create(filePath);
            request.Credentials = new NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }

            return false;
        }

        [Authorize]
        private void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }

        public ActionResult GetPdf()
        {
            string fileName = "doc.pdf";
            string filePath = Path.Combine(Server.MapPath("~/Buffer/inputPdf"), fileName);

            if (System.IO.File.Exists(filePath))
            {
                byte[] content = System.IO.File.ReadAllBytes(filePath);
                HttpContext context = System.Web.HttpContext.Current;

                Array.ForEach(Directory.GetFiles(Server.MapPath("~/Buffer/inputPdf/")), System.IO.File.Delete);

                context.Response.BinaryWrite(content);
                context.Response.ContentType = "application/pdf";
                context.Response.AppendHeader("Content-Disposition", "attachment; filename=MassDoc.pdf");
                context.Response.End();

                return View();
            }
            else
            {
                ViewBag.Title = "No Document Found";
                return View("GetPdf");
            }
        }

        public async Task<dynamic> GetPdf1(string serverIP, string ftpPort, string ftpUserName, string ftpPassword, string serverURL, string documentID, bool isObsolete, bool isSecured)
        {
            string fileName = "doc.pdf";
            string filePath = Path.Combine(Server.MapPath("~/Buffer/inputPdf"), fileName);

            if (System.IO.File.Exists(filePath))
            {
                byte[] content = System.IO.File.ReadAllBytes(filePath);
                //HttpContext context = System.Web.HttpContext.Current;

                //Array.ForEach(Directory.GetFiles(Server.MapPath("~/Buffer/inputPdf/")), System.IO.File.Delete);

                //context.Response.BinaryWrite(content);
                //context.Response.ContentType = "application/pdf";
                //context.Response.AppendHeader("Content-Disposition", "attachment; filename=MassDoc.pdf");
                //context.Response.End();

                //return View();

                Response.AppendHeader("Content-Disposition", documentID + ".pdf");
                Response.OutputStream.Write(content, 0, content.Length);

                return new HttpStatusCodeResult(200, "OK!");
            }
            else
            {
                return new HttpStatusCodeResult(401, "NOT FOUND!");
            }

        }


        [Authorize]
        public async Task<dynamic> GetMailNotifyAndExpDate(string DocumentId)
        {
            DSM_VM_Property NotifyAndExpDate = new DSM_VM_Property();
            await Task.Run(() => _originalDocSearchingService.GetMailNotifyAndExpDate(DocumentId,out NotifyAndExpDate));
            

            return Json( NotifyAndExpDate , JsonRequestBehavior.AllowGet);
        }
    }
}