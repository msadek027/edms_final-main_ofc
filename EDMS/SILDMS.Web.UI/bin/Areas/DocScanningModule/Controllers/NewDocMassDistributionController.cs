using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using SILDMS.Model.DocScanningModule;
using SILDMS.Service.DocDistribution;
using SILDMS.Service.OriginalDocSearching;
using SILDMS.Service.OwnerProperIdentity;
using SILDMS.Service.VersionDocSearching;
using SILDMS.Utillity;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static System.Net.Mime.MediaTypeNames;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class NewDocMassDistributionController : Controller
    {
        private ValidationResult _respStatus = new ValidationResult();
        private readonly Utillity.Localization.ILocalizationService _localizationService;
        private readonly IDocDistributionService _docDistributionService;
        private readonly IOwnerProperIdentityService _docPropertyIdentityService;
        private readonly IVersionDocSearchingService _versionDocScanService;
        private readonly IOriginalDocSearchingService _originalDocSearchingService;
        private readonly string UserID = string.Empty;
        private string outStatus = string.Empty;

        public NewDocMassDistributionController(IOriginalDocSearchingService originalDocSearchingService, 
            IVersionDocSearchingService versionDocScanService, IDocDistributionService docDistributionService, IOwnerProperIdentityService docPropertyIdentityService, 
            SILDMS.Utillity.Localization.ILocalizationService localizationService)
        {
            _originalDocSearchingService = originalDocSearchingService;
            _versionDocScanService = versionDocScanService;
            _docDistributionService = docDistributionService;
            _docPropertyIdentityService = docPropertyIdentityService;
            _localizationService = localizationService;
            UserID = SILAuthorization.GetUserID();
        }

        [HttpPost]
        [Authorize]
        public string UploadHandler()
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
                        TempData["ExcelData"] = dt;

                        return httpPostedFileBase.FileName;
                    }
                }
            }
            else
            {
                return "1";
            }

            return "0";
        }


        [HttpPost]
        [Authorize]
        [SILLogAttribute]
        public async Task<dynamic> AddDocumentInfo(DocumentsInfo modelDocumentsInfo, string selectedPropId, DocMetaValue docMetaValues)
        {
            string value = string.Empty;
            try
            {
                if (TempData["ExcelData"] != null)
                {
                    List<String> ValueList = new List<string>();
                    DataTable dt = (DataTable)TempData["ExcelData"];
                    ValueList = dt.AsEnumerable().Select(reader => reader["AttributeValue"].ToString()).ToList();
                    value = String.Join(",", ValueList);
                }

                List<DocSearch> docList = new List<DocSearch>();
                if (value == "")
                {
                    return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
                }
                else 
                {
                    await Task.Run(() => _docDistributionService.GetDoc(modelDocumentsInfo.Owner.OwnerID, modelDocumentsInfo.DocCategory.DocCategoryID, modelDocumentsInfo.DocType.DocTypeID, modelDocumentsInfo.DocProperty.DocPropertyID, value, UserID, out docList));
                }
                   
                
                if (docList.Count > 0)
                {
                    string filePath;
                    string trnsfrpth;
                    string path1;
                    string ftppath;
                    String assemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

                    if (!Directory.Exists(Server.MapPath("~/Buffer")))
                    {
                        string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                        string folderName = Path.Combine(projectPath, Server.MapPath("~/Buffer"));
                        System.IO.Directory.CreateDirectory(folderName);
                    }

                    if (!Directory.Exists(Server.MapPath("~/Buffer/mergePdf")))
                    {
                        string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                        string folderName = Path.Combine(projectPath, Server.MapPath("~/Buffer/mergePdf"));
                        System.IO.Directory.CreateDirectory(folderName);
                    }

                    if (!Directory.Exists(Server.MapPath("~/Buffer/mergeInputPdf")))
                    {
                        string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                        string folderName = Path.Combine(projectPath, Server.MapPath("~/Buffer/mergeInputPdf"));
                        System.IO.Directory.CreateDirectory(folderName);
                    }
 
                    var idx = -1;
                    foreach (var doc in docList)
                    {
                        using (PdfDocument outPdf = new PdfDocument())
                        {
                            idx++;
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
                                        trnsfrpth = Server.MapPath("~/Buffer/mergePdf/" + doc.DocumentID + ".pdf");
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

                                trnsfrpth = Server.MapPath("~/Buffer/mergePdf/Test.pdf");
                                using (PdfDocument temp = PdfReader.Open(trnsfrpth, PdfDocumentOpenMode.Import))
                                {
                                    CopyPages(temp, outPdf);
                                }


                                if (Directory.GetFiles(Server.MapPath("~/Buffer/mergePdf/")).Length != 0)
                                {
                                    outPdf.Save(Server.MapPath("~/Buffer/mergeInputPdf/doc_" + idx + ".pdf"));

                                    await _docDistributionService.ServerFileUpload(docList[idx], Server.MapPath("~/Buffer/mergeInputPdf/doc_" + idx + ".pdf"));
                                    _docDistributionService.ServerFileDelete(Server.MapPath("~/Buffer/mergePdf/" + docList[idx].DocumentID + ".pdf"));
                                    _docDistributionService.ServerFileDelete(Server.MapPath("~/Buffer/mergeInputPdf/doc_" + idx + ".pdf"));

                                }
                                else
                                {
                                    return Json(new { Message = "Document Not Found" }, JsonRequestBehavior.AllowGet);
                                }

                            }
                            catch (Exception ex)
                            {
                                continue;
                            }

                        }
                    }
                    return Json(new { Message = "Document Merge Successfully" }, JsonRequestBehavior.AllowGet);
                }
            
                else
                {
                    return Json(new { Message = "Document Not Found" }, JsonRequestBehavior.AllowGet);
                }
               


            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
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

        public async Task<dynamic> UploadOtherFiles()
        {
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];

            try
            {
                byte[] data;
                using (Stream inputStream = file.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }

                    data = memoryStream.ToArray();
                    string trnsfrpth = Server.MapPath("~/Buffer/mergePdf/Test.pdf");
                    System.IO.File.WriteAllBytes(trnsfrpth, data);
                }

                //  

                //await ftpstream.WriteAsync(data, 0, data.Length);
                //ftpstream.Close();

                return new HttpStatusCodeResult(200, "OK!");
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500, "OK!");
            }
        }

    }
}