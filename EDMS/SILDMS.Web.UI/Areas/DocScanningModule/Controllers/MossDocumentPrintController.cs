
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Net;
using SILDMS.Service.MultiDocScan;
using SILDMS.Service.OwnerProperIdentity;
using SILDMS.Service.OriginalDocSearching;
using SILDMS.Service.Server;
using SILDMS.Service.DocProperty;
using SILDMS.Web.UI.Areas.SecurityModule.Models;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class MossDocumentPrintController : Controller
    {
        private readonly IMultiDocScanService _multiDocScanService;
        private readonly IOwnerProperIdentityService _ownerProperIdentityService;
        private readonly IOriginalDocSearchingService _originalDocSearchingService;
        private readonly IServerService _serverService;
        private readonly IDocPropertyService _docPropertyService;
        private ValidationResult respStatus = new ValidationResult();

        private string outStatus = string.Empty;
        private readonly string UserID = string.Empty;
        public MossDocumentPrintController(IOriginalDocSearchingService originalDocSearchingService,
            IMultiDocScanService multiDocScanService,
            IOwnerProperIdentityService ownerProperIdentityRepository,
            IDocPropertyService docPropertyService, IServerService serverService)
        {
            _originalDocSearchingService = originalDocSearchingService;
            _multiDocScanService = multiDocScanService;
            _ownerProperIdentityService = ownerProperIdentityRepository;
            _docPropertyService = docPropertyService;
            _serverService = serverService;
            UserID = SILAuthorization.GetUserID();
        }
        //
        // GET: /DocScanningModule/MossDocumentPrint/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexNew()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadHandler(string ExelType)
        {
            HttpPostedFile httpPostedFileBase = System.Web.HttpContext.Current.Request.Files[0];
            if (httpPostedFileBase != null)
            {
                string[] file = httpPostedFileBase.FileName.Split('.');
                if (file.Length > 0)
                {
                    if ((file[file.Length - 1].ToString()) == "xlsx" || (file[file.Length - 1].ToString() == "xls"))
                    {
                        DataTable dt;
                        ExcelFileReader xlReader = new ExcelFileReader();
                        dt = xlReader.GetExcelDataTable(HttpContext.Request.Files[0]);
                        if (ExelType == "TR")
                        {
                            TempData["ExcelData"] = dt;
                            return Json(new { FileName = httpPostedFileBase.FileName, AttributeValueCount = dt.Rows.Count, Code = "200" }, JsonRequestBehavior.AllowGet);
                        }                       
                        else
                        {
                            return null;
                        }
                    }

                }
            }


            else
            {

                return Json(new { Code = "1" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Code = "0" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetOriginalDocBySearchFromList(string _OwnerID, string _DocCategoryID,
             string _DocTypeID, string _DocPropertyID, string _SearchBy, int page = 1, int itemsPerPage = 5, string sortBy = "", bool reverse = false, string attribute = null, string search = null)
        {
            string value = string.Empty;
            if (TempData["ExcelData"] != null)
            {
                List<String> ValueList = new List<string>();
                DataTable dt = (DataTable)TempData["ExcelData"];
                ValueList = dt.AsEnumerable().Select(reader => reader["AttributeValue"].ToString()).ToList();
                value = String.Join(",", ValueList);
            }

            List<DocSearch> docList = new List<DocSearch>();
            respStatus = await Task.Run(() => _originalDocSearchingService.GetOriginalDocBySearchFromList(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, value, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out docList));
            
            var totalPages = docList.Select(o => o.TotalCount).FirstOrDefault();
            return Json(new { respStatus, docList, totalPages }, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpPost]
        public async Task<dynamic> Marge(string _OwnerID, string _DocCategoryID,
             string _DocTypeID, string _DocPropertyID, string _SearchBy, int page = 1, int itemsPerPage = 5, string sortBy = "", bool reverse = false, string attribute = null, string Docs = null, string search = null)
        {
            string value = string.Empty;
            if (TempData["ExcelData"] != null)
            {
                List<String> ValueList = new List<string>();
                DataTable dt = (DataTable)TempData["ExcelData"];
                ValueList = dt.AsEnumerable().Select(reader => reader["AttributeValue"].ToString()).ToList();
                value = String.Join(",", ValueList);
            }

            List<DocSearch> docList = new List<DocSearch>();
            await Task.Run(() => _originalDocSearchingService.GetOriginalDocBySearchForPrint(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, Docs, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out docList));


            if (docList.Count > 0)
            {
                string filePath;
                string trnsfrpth;
                string path1;
                String assemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

                if (!Directory.Exists(Server.MapPath("~/Buffer")))
                {
                    string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string folderName = Path.Combine(projectPath, Server.MapPath("~/Buffer"));
                    System.IO.Directory.CreateDirectory(folderName);
                }

                if (!Directory.Exists(Server.MapPath("~/Buffer/bufferPdf")))
                {
                    string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string folderName = Path.Combine(projectPath, Server.MapPath("~/Buffer/bufferPdf"));
                    System.IO.Directory.CreateDirectory(folderName);
                }

                if (!Directory.Exists(Server.MapPath("~/Buffer/inputPdf")))
                {
                    string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string folderName = Path.Combine(projectPath, Server.MapPath("~/Buffer/inputPdf"));
                    System.IO.Directory.CreateDirectory(folderName);
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
                                    ftpClient.Credentials = new System.Net.NetworkCredential(doc.FtpUserName, doc.FtpPassword);
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
	}
}