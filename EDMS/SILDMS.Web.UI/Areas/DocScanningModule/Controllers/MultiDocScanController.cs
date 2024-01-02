using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using SILDMS.Model.DocScanningModule;
using SILDMS.Service.AutoValueSetup;
using SILDMS.Service.DocProperty;
using SILDMS.Service.MultiDocScan;
using SILDMS.Service.OwnerProperIdentity;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System.IO;
using System.Web;
using System.Net;
using iTextSharp.text.pdf;
using SILDMS.Service.Users;
using iTextSharp.text;
using SILDMS.Model.SecurityModule;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class MultiDocScanController : Controller
    {
        private readonly IMultiDocScanService _multiDocScanService;
        private readonly IOwnerProperIdentityService _ownerProperIdentityService;
        private readonly IAutoValueSetupService _autoValueSetupService;
        readonly IUserService _userService;
        private readonly IDocPropertyService _docPropertyService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult respStatus = new ValidationResult();
        private readonly string UserID = string.Empty;
        private readonly string OwnerID = string.Empty;
        private readonly string RoleTitle = string.Empty;

        private string action = "";

        public MultiDocScanController(IMultiDocScanService multiDocScanService, IOwnerProperIdentityService ownerProperIdentityRepository,
            IDocPropertyService docPropertyService, IAutoValueSetupService autoValueSetupService, IUserService UserService, ILocalizationService localizationService)
        {
            _multiDocScanService = multiDocScanService;
            _ownerProperIdentityService = ownerProperIdentityRepository;
            _docPropertyService = docPropertyService;
            _localizationService = localizationService;
            _autoValueSetupService = autoValueSetupService;
            _userService = UserService;

            UserID = SILAuthorization.GetUserID();
            OwnerID = SILAuthorization.GetOwnerID();
            RoleTitle = SILAuthorization.GetRoleTitle();
        }

        [SILAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<dynamic> GetDocPropIdentityForSelectedDocTypes(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _SelectedPropID)
        {
            List<DSM_DocPropIdentify> objDocPropIdentifies = new List<DSM_DocPropIdentify>();
            await Task.Run(() => _multiDocScanService.GetDocPropIdentityForSelectedDocTypes(UserID, _OwnerID, _DocCategoryID, _SelectedPropID, out objDocPropIdentifies));
            return Json(new { Msg = "", objDocPropIdentifies }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetDocumentProperty(string _DocCategoryID, string _OwnerID, string _DocTypeID)
        {
            List<DSM_DocProperty> objDsmDocProperties = null;
            List<DSM_DocPropIdentify> objDocPropIdentifies = null;

            await Task.Run(() => _docPropertyService.GetDocProperty("", UserID, out objDsmDocProperties));
            await Task.Run(() => _ownerProperIdentityService.GetDocPropIdentify(UserID, "", out objDocPropIdentifies));

            var joinResult = (from dp in objDsmDocProperties
                              where dp.OwnerID == _OwnerID & dp.DocCategoryID == _DocCategoryID & dp.DocTypeID == _DocTypeID & dp.Status == 1
                              join dpi in objDocPropIdentifies on dp.DocPropertyID equals dpi.DocPropertyID into docPropIdentities
                              from dpi in docPropIdentities.DefaultIfEmpty()
                              select new
                              {
                                  DocPropertyID = dp.DocPropertyID,
                                  DocPropertyName = dp.DocPropertyName,
                                  email = dp.email,
                                  sms = dp.sms,
                                  obsulate = dp.obsulate,
                                  IsSelected = false
                              }).ToList();

            var result = (from s in joinResult
                          group s by new
                          {
                              s.DocPropertyID
                          }
                              into g
                              select new
                              {
                                  DocPropertyID = g.Select(p => p.DocPropertyID).FirstOrDefault(),
                                  DocPropertyName = g.Select(x => x.DocPropertyName).FirstOrDefault(),
                                  email = g.Select(x => x.email).FirstOrDefault(),
                                  sms = g.Select(x => x.sms).FirstOrDefault(),
                                  obsulate = g.Select(x => x.obsulate).FirstOrDefault(),
                                  IsSelected = false
                              }).ToList();

            return Json(new { Msg = "", result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        //[SILLogAttribute]
        public async Task<dynamic> AddDocumentInfo(DocumentsInfo _modelDocumentsInfo, List<DSM_VM_Property> _selectedPropID, List<DocMetaValue> _docMetaValues, bool _otherUpload, string _extentions, bool IsSecured)
        {
            List<DSM_DocPropIdentify> objDocPropIdentifies = null;

            if (ModelState.IsValid)
            {
                action = "add";
                _modelDocumentsInfo.SetBy = UserID;
                _modelDocumentsInfo.ModifiedBy = _modelDocumentsInfo.SetBy;
                _modelDocumentsInfo.UploaderIP = GetIPAddress.LocalIPAddress();

                var _docPropIdentifyIDs = string.Join(",", _docMetaValues.Select(x => x.DocPropIdentifyID));

                respStatus.Message = "Success";
                respStatus = await Task.Run(() => _multiDocScanService.AddDocumentInfo(_modelDocumentsInfo, _selectedPropID, _docMetaValues, _otherUpload, _extentions, IsSecured, action, out objDocPropIdentifies));

                var DistinctDocIDs = (from s in objDocPropIdentifies
                                      group s by new
                                      {
                                          s.DocumentID
                                      }
                                          into g
                                          select new
                                          {
                                              DocPropID = g.Select(p => p.DocPropertyID).FirstOrDefault(),
                                              DocumentID = g.Select(p => p.DocumentID).FirstOrDefault(),
                                              FileCodeName = g.Select(p => p.FileCodeName).FirstOrDefault(),
                                              FileServerUrl = g.Select(x => x.FileServerUrl).FirstOrDefault()
                                          }).ToList();

                foreach (var item in DistinctDocIDs)
                {
                    try
                    {
                        FolderGenerator.MakeFTPDir(objDocPropIdentifies.FirstOrDefault().ServerIP, objDocPropIdentifies.FirstOrDefault().ServerPort, item.FileServerUrl, objDocPropIdentifies.FirstOrDefault().FtpUserName, objDocPropIdentifies.FirstOrDefault().FtpPassword);
                    }
                    catch (Exception e)
                    {

                    }
                }

                ViewBag.LoggID = DistinctDocIDs.Count > 0 ? DistinctDocIDs[0].DocumentID : "";
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Add Document";
                ViewBag.LookupTable = "DSM_Documents";

                return Json(new { Message = respStatus.Message, result = objDocPropIdentifies, DistinctID = DistinctDocIDs }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var errors = ModelState.Values.SelectMany(v => v.Errors);
                respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));
                return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<dynamic> DeleteDocumentInfo(string _DocumentIDs)
        {
            respStatus = await Task.Run(() => _multiDocScanService.DeleteDocumentInfo(_DocumentIDs));
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> IsRevesionNoValid(string _OwnerID, string _DocCatID, string _DocTypID, string _DocPropertyID, string _DocPropIdentifyID, string _MetaValue, string _revesionNo)
        {
            respStatus = await Task.Run(() => _multiDocScanService.IsRevesionNoValid(UserID, _OwnerID, _DocCatID, _DocTypID, _DocPropertyID, _DocPropIdentifyID, _MetaValue, _revesionNo));
            return Json(respStatus, JsonRequestBehavior.AllowGet);
        }

        //This Is a Test Controller For saving pdf to FTP
        //public async Task<dynamic> FilePassWord(string serverIP, string ftpPort, string ftpUserName, string ftpPassword, string serverURL, string documentID, string RemoteFile)
        //{
        //    byte[] bytes = Convert.FromBase64String(RemoteFile);

        //    //string WorkingFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //    //string OutputFile = Path.Combine(WorkingFolder, "Test_enc.pdf");

        //    //using (MemoryStream input = new MemoryStream(bytes))
        //    //{
        //    //    using (Stream output = new FileStream(OutputFile, FileMode.Create, FileAccess.Write, FileShare.None))
        //    //    {
        //    //        PdfReader reader = new PdfReader(input);
        //    //        PdfEncryptor.Encrypt(reader, output, true, "ABC", null, PdfWriter.ALLOW_SCREENREADERS);
        //    //    }
        //    //}

        //    using (MemoryStream input = new MemoryStream(bytes))
        //    {
        //        using (MemoryStream output = new MemoryStream())
        //        {
        //            PdfDocument document = PdfReader.Open(input);
        //            PdfSecuritySettings securitySettings = document.SecuritySettings;

        //            securitySettings.UserPassword = "user";
        //            securitySettings.OwnerPassword = "owner";
        //            // Don't use 40 bit encryption unless needed for compatibility reasons
        //            //securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted40Bit;

        //            // Restrict some rights.
        //            securitySettings.PermitAccessibilityExtractContent = false;
        //            securitySettings.PermitAnnotations = false;
        //            securitySettings.PermitAssembleDocument = false;
        //            securitySettings.PermitExtractContent = false;
        //            securitySettings.PermitFormsFill = true;
        //            securitySettings.PermitFullQualityPrint = false;
        //            securitySettings.PermitModifyDocument = true;
        //            securitySettings.PermitPrint = false;

        //            document.Save(output);

        //            bytes = output.ToArray();
        //            try
        //            {
        //                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create("ftp://" + serverIP + "/"+serverURL +"/"+documentID+ ".pdf");
        //                ftp.Credentials = new NetworkCredential(ftpUserName,ftpPassword);
        //                ftp.Proxy = null;
        //                ftp.KeepAlive = true;
        //                ftp.UseBinary = true;
        //                ftp.Method = WebRequestMethods.Ftp.UploadFile;
        //                Stream ftpstream = ftp.GetRequestStream();
        //                ftpstream.Write(bytes, 0, bytes.Length);
        //                ftpstream.Close();
        //                return new HttpStatusCodeResult(200, "OK!");
        //            }
        //            catch (Exception)
        //            {
        //                return new HttpStatusCodeResult(500, "OK!");
        //            }
        //        }
        //    }
        //}

        //public async Task<dynamic> GetFilePassWord(string serverIP, string ftpPort, string ftpUserName, string ftpPassword, string serverURL, string documentID)
        //{
        //    using (WebClient request = new WebClient())
        //    {
        //        request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
        //        byte[] fileData = request.DownloadData("ftp://" + serverIP + "/" + serverURL + "/" + documentID + ".pdf");

        //        using (MemoryStream input = new MemoryStream(fileData))
        //        {
        //            using (MemoryStream output = new MemoryStream())
        //            {
        //                PdfDocument doc = PdfReader.Open(input, "owner", PdfDocumentOpenMode.Import);
        //                PdfDocument docOut = new PdfDocument();
        //                foreach (PdfPage page in doc.Pages)
        //                {
        //                    docOut.AddPage(page);
        //                }
        //                docOut.Save(output);
        //                Response.AppendHeader("Content-Disposition", documentID + ".pdf");
        //                Response.OutputStream.Write(output.ToArray(), 0, output.ToArray().Length);
        //            }    
        //        }
        //    }
        //
        //    return new HttpStatusCodeResult(200, "OK!");
        //}

        public async Task<dynamic> UploadOtherFiles(string serverIP, string ftpPort, string ftpUserName, string ftpPassword, string serverURL, string documentID, string Ext)
        {
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];

            try
            {
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create("ftp://" + serverIP + "/" + serverURL + "/" + documentID + "." + Ext);
                ftp.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                ftp.Proxy = null;
                ftp.KeepAlive = true;
                ftp.UseBinary = true;
                ftp.Method = WebRequestMethods.Ftp.UploadFile;
                Stream ftpstream = ftp.GetRequestStream();

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
                }

                await ftpstream.WriteAsync(data, 0, data.Length);
                ftpstream.Close();

                return new HttpStatusCodeResult(200, "OK!");
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500, "OK!");
            }
        }

        public async Task<dynamic> GetFilePassWord_r(string serverIP, string ftpPort, string ftpUserName, string ftpPassword, string serverURL, string documentID, bool isObsolete, bool isSecured)
        {
            string watermarkImagePath = Server.MapPath("~/Stamp/obsolete.png");
            byte[] fileData;
            try
            {
                using (WebClient request = new WebClient())
                {
                    request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                    fileData = await request.DownloadDataTaskAsync("ftp://" + serverIP + "/" + serverURL + "/" + documentID + ".pdf");
                }

                PdfReader reader;

                if (!isSecured && !isObsolete)
                {
                    Response.AppendHeader("Content-Disposition", documentID + ".pdf");
                    Response.OutputStream.Write(fileData, 0, fileData.Length);

                    return new HttpStatusCodeResult(200, "OK!");
                }
                else if (isSecured && !isObsolete)
                {
                    reader = new PdfReader(fileData, new System.Text.ASCIIEncoding().GetBytes("secret"));

                    using (MemoryStream output = new MemoryStream())
                    {
                        PdfStamper stamper = new PdfStamper(reader, output);
                        stamper.Dispose();
                        stamper.Close();
                        var buffer = output.ToArray();

                        Response.AppendHeader("Content-Disposition", documentID + ".pdf");
                        Response.OutputStream.Write(buffer, 0, buffer.Length);
                    }

                    return new HttpStatusCodeResult(200, "OK!");
                }
                else if (isSecured && isObsolete)
                {
                    reader = new PdfReader(fileData, new System.Text.ASCIIEncoding().GetBytes("secret"));

                    var img = iTextSharp.text.Image.GetInstance(watermarkImagePath);
                    img.SetAbsolutePosition(0, 450);
                    PdfContentByte waterMark;

                    using (MemoryStream output = new MemoryStream())
                    {
                        using (PdfStamper stamper = new PdfStamper(reader, output))
                        {
                            int pages = reader.NumberOfPages;
                            for (int i = 1; i <= pages; i++)
                            {
                                waterMark = stamper.GetOverContent(i);
                                waterMark.AddImage(img);
                            }
                        }

                        var buffer = output.ToArray();
                        Response.AppendHeader("Content-Disposition", documentID + ".pdf");
                        Response.OutputStream.Write(buffer, 0, buffer.Length);
                    }

                    return new HttpStatusCodeResult(200, "OK!");
                }
                else if (!isSecured && isObsolete)
                {
                    reader = new PdfReader(fileData);
                    var img = iTextSharp.text.Image.GetInstance(watermarkImagePath);
                    img.SetAbsolutePosition(0, 450);
                    PdfContentByte waterMark;

                    using (MemoryStream output = new MemoryStream())
                    {
                        using (PdfStamper stamper = new PdfStamper(reader, output))
                        {
                            int pages = reader.NumberOfPages;
                            for (int i = 1; i <= pages; i++)
                            {
                                waterMark = stamper.GetOverContent(i);
                                waterMark.AddImage(img);
                            }
                        }

                        var buffer = output.ToArray();
                        Response.AppendHeader("Content-Disposition", documentID + ".pdf");
                        Response.OutputStream.Write(buffer, 0, buffer.Length);
                    }

                    return new HttpStatusCodeResult(200, "OK!");
                }
                else
                {
                    return new HttpStatusCodeResult(401, "NOT FOUND!");
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(401, "NOT FOUND!");
            }
           
        }

        public async Task<dynamic> FilePassWord_r(string serverIP, string ftpPort, string ftpUserName, string ftpPassword, string serverURL, string documentID, string RemoteFile)
        {
            byte[] bytes = Convert.FromBase64String(RemoteFile);
            using (MemoryStream output = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes);
                PdfEncryptor.Encrypt(reader, output, true, "secret", "secret", 0);

                bytes = output.ToArray();

                try
                {
                    FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create("ftp://" + serverIP + "/" + serverURL + "/" + documentID + ".pdf");
                    ftp.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                    ftp.Proxy = null;
                    ftp.KeepAlive = true;
                    ftp.UseBinary = true;
                    ftp.Method = WebRequestMethods.Ftp.UploadFile;
                    Stream ftpstream = ftp.GetRequestStream();
                    await ftpstream.WriteAsync(bytes, 0, bytes.Length);
                    ftpstream.Close();

                    //using (System.IO.FileStream outputrr = new System.IO.FileStream(@"D:\MyOutput.pdf", FileMode.Create))
                    //{
                    //    await outputrr.WriteAsync(bytes, 0, bytes.Length);
                    //}

                    return new HttpStatusCodeResult(200, "OK!");
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(500, "OK!");
                }

            }
        }

       // [SILLogAttribute]
        public async Task<dynamic> GetInformationCopy(string serverIP, string ftpPort, string ftpUserName, string ftpPassword, string serverURL, string documentID, bool InformationCopy, bool isObsolete, bool isSecured)
        {

            string watermarkImagePathInfo = Server.MapPath("~/Stamp/InformationCopy.png");
            string watermarkImagePathObs = Server.MapPath("~/Stamp/obsolete.png");
            byte[] fileData;

            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                fileData = await request.DownloadDataTaskAsync("ftp://" + serverIP + "/" + serverURL + "/" + documentID + ".pdf");
            }

            SEC_User obUser = new SEC_User();
            await Task.Run(() => _userService.GetAUser(UserID, OwnerID, out obUser));

            DSM_DocProperty obProp = new DSM_DocProperty();
            await Task.Run(() => _docPropertyService.GetInfoValidPeriod(UserID, documentID, out obProp));

            PdfReader reader;

            string roleTitle = RoleTitle;

            if (InformationCopy != true && !isObsolete)
            {
                reader = new PdfReader(fileData);
                PdfContentByte waterMark;

                using (MemoryStream output = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, output))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            waterMark = stamper.GetOverContent(i);

                            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            waterMark.SetColorFill(BaseColor.BLUE);
                            waterMark.SetFontAndSize(bf, 12);

                            Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                            if (currentPageRectangle.Width > currentPageRectangle.Height)
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName;
                                waterMark.ShowTextAligned(3, text, 60, 560, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Printed On " + DateTime.Now;
                                waterMark.ShowTextAligned(3, text, 60, 540, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 530, 0);
                                waterMark.EndText();
                            }
                            else
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName;
                                waterMark.ShowTextAligned(3, text, 60, 830, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Printed On " + DateTime.Now;
                                waterMark.ShowTextAligned(3, text, 60, 810, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 790, 0);
                                waterMark.EndText();
                            }

                        }
                    }

                    var buffer = output.ToArray();
                    Response.AppendHeader("Content-Disposition", documentID + ".pdf");
                    Response.OutputStream.Write(buffer, 0, buffer.Length);
                }

                ViewBag.LoggID = documentID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Download Document";
                ViewBag.LookupTable = "DSM_Documents";

                return new HttpStatusCodeResult(200, "OK!");
            }
            else if (InformationCopy != true && isObsolete)
            {
                reader = new PdfReader(fileData);
                var imgObs = iTextSharp.text.Image.GetInstance(watermarkImagePathObs);
                imgObs.SetAbsolutePosition(0, 450);
                PdfContentByte waterMark;

                using (MemoryStream output = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, output))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            waterMark = stamper.GetOverContent(i);
                            waterMark.AddImage(imgObs);

                            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            waterMark.SetColorFill(BaseColor.BLUE);
                            waterMark.SetFontAndSize(bf, 12);

                            Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                            if (currentPageRectangle.Width > currentPageRectangle.Height)
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName;
                                waterMark.ShowTextAligned(3, text, 60, 570, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Printed On " + DateTime.Now;
                                waterMark.ShowTextAligned(3, text, 60, 540, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 530, 0);
                                waterMark.EndText();
                            }
                            else
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName;
                                waterMark.ShowTextAligned(3, text, 60, 830, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Printed On " + DateTime.Now;
                                waterMark.ShowTextAligned(3, text, 60, 810, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 790, 0);
                                waterMark.EndText();
                            }
                        }
                    }

                    var buffer = output.ToArray();
                    Response.AppendHeader("Content-Disposition", documentID + ".pdf");
                    Response.OutputStream.Write(buffer, 0, buffer.Length);
                }

                ViewBag.LoggID = documentID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Download Document";
                ViewBag.LookupTable = "DSM_Documents";

                return new HttpStatusCodeResult(200, "OK!");
            }
            else if (InformationCopy == true && !isObsolete)
            {
                reader = new PdfReader(fileData);
                var imgInfo = iTextSharp.text.Image.GetInstance(watermarkImagePathInfo);
                imgInfo.SetAbsolutePosition(200, 250);
                PdfContentByte waterMark;

                using (MemoryStream output = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, output))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            waterMark = stamper.GetOverContent(i);
                            waterMark.AddImage(imgInfo);

                            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            waterMark.SetColorFill(BaseColor.BLUE);
                            waterMark.SetFontAndSize(bf, 12);

                            Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                            if (currentPageRectangle.Width > currentPageRectangle.Height)
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName;
                                waterMark.ShowTextAligned(3, text, 60, 570, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Printed On " + DateTime.Now;
                                waterMark.ShowTextAligned(3, text, 60, 540, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 530, 0);
                                waterMark.EndText();
                            }
                            else
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName;
                                waterMark.ShowTextAligned(3, text, 60, 830, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Printed On " + DateTime.Now;
                                waterMark.ShowTextAligned(3, text, 60, 810, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 790, 0);
                                waterMark.EndText();
                            }
                        }
                    }

                    var buffer = output.ToArray();
                    Response.AppendHeader("Content-Disposition", documentID + ".pdf");
                    Response.OutputStream.Write(buffer, 0, buffer.Length);
                }

                ViewBag.LoggID = documentID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Download Document";
                ViewBag.LookupTable = "DSM_Documents";

                return new HttpStatusCodeResult(200, "OK!");
            }
            else if (InformationCopy == true && isObsolete)
            {
                reader = new PdfReader(fileData);
                var imgInfo = iTextSharp.text.Image.GetInstance(watermarkImagePathInfo);
                var imgObs = iTextSharp.text.Image.GetInstance(watermarkImagePathObs);
                imgInfo.SetAbsolutePosition(200, 250);
                imgObs.SetAbsolutePosition(0, 450);
                PdfContentByte waterMark;

                using (MemoryStream output = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, output))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            waterMark = stamper.GetOverContent(i);
                            waterMark.AddImage(imgInfo);
                            waterMark.AddImage(imgObs);

                            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            waterMark.SetColorFill(BaseColor.BLUE);
                            waterMark.SetFontAndSize(bf, 12);

                            Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                            if (currentPageRectangle.Width > currentPageRectangle.Height)
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName;
                                waterMark.ShowTextAligned(3, text, 60, 570, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Printed On " + DateTime.Now;
                                waterMark.ShowTextAligned(3, text, 60, 540, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 530, 0);
                                waterMark.EndText();
                            }
                            else
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName;
                                waterMark.ShowTextAligned(3, text, 60, 830, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Printed On " + DateTime.Now;
                                waterMark.ShowTextAligned(3, text, 60, 810, 0);
                                waterMark.EndText();

                                waterMark.BeginText();
                                text = "Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 790, 0);
                                waterMark.EndText();
                            }
                        }
                    }

                    var buffer = output.ToArray();
                    Response.AppendHeader("Content-Disposition", documentID + ".pdf");
                    Response.OutputStream.Write(buffer, 0, buffer.Length);
                }

                ViewBag.LoggID = documentID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Download Document";
                ViewBag.LookupTable = "DSM_Documents";

                return new HttpStatusCodeResult(200, "OK!");
            }
            else
            {
                return new HttpStatusCodeResult(401, "NOT FOUND!");
            }

        }





        [HttpPost]
       // [SILLogAttribute]
        public string GetInformationCopyToPrint(string serverIP, string ftpPort, string ftpUserName, string ftpPassword, string serverURL, string documentID, bool isObsolete, bool isSecured, bool InformationCopy, bool Action, bool addTextAction,
            /*int PageFrom, int PageTo,string TextToAdd,float left,float top,float FontSize*/ List<TextAddProperty> TextAddPropertyCollection)
        {
            string watermarkImagePathInfo = Server.MapPath("~/Stamp/InformationCopy.png");
            string watermarkImagePathObs = Server.MapPath("~/Stamp/Obsolete.png");
            byte[] fileData=null, buffer;

            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                try {
                    fileData = request.DownloadData("ftp://" + serverIP + "/" + serverURL + "/" + documentID + ".pdf");
                }
                catch
                {
                    FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://" + serverIP + "/" + serverURL + "/");
                    ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                    ftpWebRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                    ftpWebRequest.UsePassive = true;
                    ftpWebRequest.UseBinary = true;
                    ftpWebRequest.KeepAlive = false;

                    List<DirectoryItem> returnValue = new List<DirectoryItem>();
                    string[] list = null;
                    List<string> finallist = new List<string>();
                    using (FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse())
                    using (StreamReader reader1 = new StreamReader(response.GetResponseStream()))
                    {
                        list = reader1.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < list.Length; i++)
                        {
                            string[] temporaryList = list[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < temporaryList.Length; j++)
                            {
                                if (temporaryList[j].Contains("."))
                                {
                                    finallist.Add(temporaryList[j]);
                                }
                            }
                        }
                    }
                    
                    for (int i = 0; i < finallist.Count; i++)
                    {
                        if (finallist[i].Contains(documentID))
                        {
                            string a = "ftp://" + serverIP + "/" + serverURL + "/" + finallist[i];
                            fileData = request.DownloadData("ftp://" + serverIP + "/" + serverURL + "/" + finallist[i]);
                            //return fileData;
                        }
                        else {
                            fileData = null;
                        }
                    }

                    
                }
                
            }

            SEC_User obUser = new SEC_User();
            _userService.GetAUser(UserID, OwnerID, out obUser);

            DSM_DocProperty obProp = new DSM_DocProperty();
            _docPropertyService.GetInfoValidPeriod(UserID, documentID, out obProp);

            PdfReader reader;

            string roleTitle = RoleTitle;


            if (InformationCopy != true && !isObsolete)
            {
                reader = new PdfReader(fileData);
                PdfContentByte waterMark;

                using (MemoryStream output = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, output))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            waterMark = stamper.GetOverContent(i);

                            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            waterMark.SetColorFill(BaseColor.BLUE);
                            waterMark.SetFontAndSize(bf, 12);

                        }

                        if (addTextAction)
                        {
                            for (int j = 0; j < TextAddPropertyCollection.Count; j++)
                            {
                                for (int i = TextAddPropertyCollection[j].PageFrom + 1; i <= TextAddPropertyCollection[j].PageTo + 1; i++)
                                {
                                    Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                                    waterMark = stamper.GetOverContent(i);
                                    //waterMark.AddImage(imgInfo);

                                    BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                    waterMark.SetColorFill(BaseColor.RED);
                                    waterMark.SetFontAndSize(bf, TextAddPropertyCollection[j].FontSize / 2);

                                    float x = Convert.ToSingle(TextAddPropertyCollection[j].left / 2.783);
                                    float y = Convert.ToSingle(TextAddPropertyCollection[j].top / 2.56);
                                    waterMark.BeginText();
                                    waterMark.ShowTextAligned(3, TextAddPropertyCollection[j].TextToAdd, x, currentPageRectangle.Height - y, 0);
                                    waterMark.EndText();

                                }
                            }



                        }
                    }

                    buffer = output.ToArray();
                }

                ViewBag.LoggID = documentID;
                ViewBag.LoggResult = "";
                ViewBag.LoggAction = "Print Document";
                ViewBag.LookupTable = "DSM_Documents";

                return Convert.ToBase64String(buffer);
            }

            if (InformationCopy != true && isObsolete)
            {
                reader = new PdfReader(fileData);
                var imgObs = iTextSharp.text.Image.GetInstance(watermarkImagePathObs);
                imgObs.SetAbsolutePosition(0, 450);
                PdfContentByte waterMark;

                using (MemoryStream output = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, output))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            waterMark = stamper.GetOverContent(i);
                            waterMark.AddImage(imgObs);
                            #region unnecessary for new business
                            //BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            //waterMark.SetColorFill(BaseColor.BLUE);
                            //waterMark.SetFontAndSize(bf, 12);

                            //Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                            //if (currentPageRectangle.Width > currentPageRectangle.Height)
                            //{
                            //    waterMark.BeginText();
                            //    string text = "Printed by " + obUser.UserFullName;
                            //    waterMark.ShowTextAligned(3, text, 60, 570, 0);
                            //    waterMark.EndText();

                            //    waterMark.BeginText();
                            //    text = "Printed On " + DateTime.Now;
                            //    waterMark.ShowTextAligned(3, text, 60, 540, 0);
                            //    waterMark.EndText();

                            //    waterMark.BeginText();
                            //    text = "Valid till " + obProp.InfoValidOn;
                            //    waterMark.ShowTextAligned(3, text, 60, 530, 0);
                            //    waterMark.EndText();
                            //}
                            //else
                            //{
                            //    waterMark.BeginText();
                            //    string text = "Printed by " + obUser.UserFullName;
                            //    waterMark.ShowTextAligned(3, text, 60, 830, 0);
                            //    waterMark.EndText();

                            //    waterMark.BeginText();
                            //    text = "Printed On " + DateTime.Now;
                            //    waterMark.ShowTextAligned(3, text, 60, 810, 0);
                            //    waterMark.EndText();

                            //    waterMark.BeginText();
                            //    text = "Valid till " + obProp.InfoValidOn;
                            //    waterMark.ShowTextAligned(3, text, 60, 790, 0);
                            //    waterMark.EndText();
                            //}
                            #endregion
                        }
                        if (addTextAction)
                        {
                            for (int j = 0; j < TextAddPropertyCollection.Count; j++)
                            {
                                for (int i = TextAddPropertyCollection[j].PageFrom + 1; i <= TextAddPropertyCollection[j].PageTo + 1; i++)
                                {
                                    Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                                    waterMark = stamper.GetOverContent(i);
                                    //waterMark.AddImage(imgInfo);

                                    BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                                    waterMark.SetColorFill(BaseColor.RED);
                                    waterMark.SetFontAndSize(bf, TextAddPropertyCollection[j].FontSize / 2);

                                    float x = Convert.ToSingle(TextAddPropertyCollection[j].left / 2.783);
                                    float y = Convert.ToSingle(TextAddPropertyCollection[j].top / 2.56);
                                    waterMark.BeginText();
                                    waterMark.ShowTextAligned(3, TextAddPropertyCollection[j].TextToAdd, x, currentPageRectangle.Height - y, 0);
                                    waterMark.EndText();

                                }
                            }
                        }
                    }

                    buffer = output.ToArray();
                }
                if (Action)
                {
                    ViewBag.LoggID = documentID;
                    ViewBag.LoggResult = "";
                    ViewBag.LoggAction = "Print Document";
                    ViewBag.LookupTable = "DSM_Documents";
                }
                else
                {
                    ViewBag.LoggID = documentID;
                    ViewBag.LoggResult = "";
                    ViewBag.LoggAction = "Download Document";
                    ViewBag.LookupTable = "DSM_Documents";
                }

                return Convert.ToBase64String(buffer);
            }
            else if (InformationCopy == true && !isObsolete)
            {
                reader = new PdfReader(fileData);
                var imgInfo = iTextSharp.text.Image.GetInstance(watermarkImagePathInfo);
                imgInfo.SetAbsolutePosition(200, 250);
                PdfContentByte waterMark;

                using (MemoryStream output = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, output))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            waterMark = stamper.GetOverContent(i);
                            waterMark.AddImage(imgInfo);

                            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            waterMark.SetColorFill(BaseColor.BLUE);
                            waterMark.SetFontAndSize(bf, 12);

                            Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                            if (currentPageRectangle.Width > currentPageRectangle.Height)
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName + " Printed On " + DateTime.Now + " Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 560, 0);
                                waterMark.EndText();

                            }
                            else
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName + " Printed On " + DateTime.Now + " Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 820, 0);
                                waterMark.EndText();

                                //waterMark.BeginText();
                                //text = "Printed On " + DateTime.Now;
                                //waterMark.ShowTextAligned(3, text, 60, 810, 0);
                                //waterMark.EndText();

                                //waterMark.BeginText();
                                //text = "Valid till " + obProp.InfoValidOn;
                                //waterMark.ShowTextAligned(3, text, 60, 790, 0);
                                //waterMark.EndText();
                            }
                        }

                        //if (addTextAction)
                        //{
                        //    for (int j = 0; j < TextAddPropertyCollection.Count; j++)
                        //    {
                        //        for (int i = TextAddPropertyCollection[j].PageFrom + 1; i <= TextAddPropertyCollection[j].PageTo + 1; i++)
                        //        {
                        //            Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                        //            waterMark = stamper.GetOverContent(i);
                        //            //waterMark.AddImage(imgInfo);

                        //            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        //            waterMark.SetColorFill(BaseColor.RED);
                        //            waterMark.SetFontAndSize(bf, TextAddPropertyCollection[j].FontSize / 2);

                        //            float x = Convert.ToSingle(TextAddPropertyCollection[j].left / 2.783);
                        //            float y = Convert.ToSingle(TextAddPropertyCollection[j].top / 2.56);
                        //            waterMark.BeginText();
                        //            waterMark.ShowTextAligned(3, TextAddPropertyCollection[j].TextToAdd, x, currentPageRectangle.Height - y, 0);
                        //            waterMark.EndText();

                        //        }
                        //    }
                        //}
                    }

                    buffer = output.ToArray();
                }

                if (Action)
                {
                    ViewBag.LoggID = documentID;
                    ViewBag.LoggResult = "";
                    ViewBag.LoggAction = "Print Document";
                    ViewBag.LookupTable = "DSM_Documents";
                }
                else
                {
                    ViewBag.LoggID = documentID;
                    ViewBag.LoggResult = "";
                    ViewBag.LoggAction = "Download Document";
                    ViewBag.LookupTable = "DSM_Documents";
                }


                return Convert.ToBase64String(buffer);
            }
            else if (InformationCopy == true && isObsolete)
            {
                reader = new PdfReader(fileData);
                var imgInfo = iTextSharp.text.Image.GetInstance(watermarkImagePathInfo);
                var imgObs = iTextSharp.text.Image.GetInstance(watermarkImagePathObs);
                imgInfo.SetAbsolutePosition(200, 250);
                imgObs.SetAbsolutePosition(0, 450);
                PdfContentByte waterMark;

                using (MemoryStream output = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, output))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            waterMark = stamper.GetOverContent(i);
                            waterMark.AddImage(imgInfo);
                            waterMark.AddImage(imgObs);

                            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                            waterMark.SetColorFill(BaseColor.BLUE);
                            waterMark.SetFontAndSize(bf, 12);

                            Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                            if (currentPageRectangle.Width > currentPageRectangle.Height)
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName + " Printed On " + DateTime.Now + " Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 560, 0);
                                waterMark.EndText();

                                //waterMark.BeginText();
                                //text = "Printed On " + DateTime.Now;
                                //waterMark.ShowTextAligned(3, text, 60, 540, 0);
                                //waterMark.EndText();

                                //waterMark.BeginText();
                                //text = "Valid till " + obProp.InfoValidOn;
                                //waterMark.ShowTextAligned(3, text, 60, 530, 0);
                                //waterMark.EndText();
                            }
                            else
                            {
                                waterMark.BeginText();
                                string text = "Printed by " + obUser.UserFullName + " Printed On " + DateTime.Now + " Valid till " + obProp.InfoValidOn;
                                waterMark.ShowTextAligned(3, text, 60, 820, 0);
                                waterMark.EndText();

                                //waterMark.BeginText();
                                //text = "Printed On " + DateTime.Now;
                                //waterMark.ShowTextAligned(3, text, 60, 810, 0);
                                //waterMark.EndText();

                                //waterMark.BeginText();
                                //text = "Valid till " + obProp.InfoValidOn;
                                //waterMark.ShowTextAligned(3, text, 60, 790, 0);
                                //waterMark.EndText();
                            }
                        }

                        //if (addTextAction)
                        //{
                        //    for (int j = 0; j < TextAddPropertyCollection.Count; j++)
                        //    {
                        //        for (int i = TextAddPropertyCollection[j].PageFrom + 1; i <= TextAddPropertyCollection[j].PageTo + 1; i++)
                        //        {
                        //            Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                        //            waterMark = stamper.GetOverContent(i);
                        //            //waterMark.AddImage(imgInfo);

                        //            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        //            waterMark.SetColorFill(BaseColor.RED);
                        //            waterMark.SetFontAndSize(bf, TextAddPropertyCollection[j].FontSize / 2);

                        //            float x = Convert.ToSingle(TextAddPropertyCollection[j].left / 2.783);
                        //            float y = Convert.ToSingle(TextAddPropertyCollection[j].top / 2.56);
                        //            waterMark.BeginText();
                        //            waterMark.ShowTextAligned(3, TextAddPropertyCollection[j].TextToAdd, x, currentPageRectangle.Height - y, 0);
                        //            waterMark.EndText();

                        //        }
                        //    }
                        //}
                    }

                    buffer = output.ToArray();
                }

                if (Action)
                {
                    ViewBag.LoggID = documentID;
                    ViewBag.LoggResult = "";
                    ViewBag.LoggAction = "Print Document";
                    ViewBag.LookupTable = "DSM_Documents";
                }
                else
                {
                    ViewBag.LoggID = documentID;
                    ViewBag.LoggResult = "";
                    ViewBag.LoggAction = "Download Document";
                    ViewBag.LookupTable = "DSM_Documents";
                }

                return Convert.ToBase64String(buffer);
            }
            else
            {
                return "error";
            }
        }


        public ActionResult downloadFile(string token)
        {
            string serverIP, ftpPort, ftpUserName, ftpPassword, serverURL, documentID;
            var base64EncodedBytes = System.Convert.FromBase64String(token);
            token= System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            string[] data = token.Split(new[] { "..|.." }, StringSplitOptions.None);
            serverIP=data[0]; ftpPort=data[1]; ftpUserName=data[2]; ftpPassword=data[3]; serverURL=data[4]; documentID=data[5];
            

            byte[] fileData = null;
            int track = 0;string name, ext;

            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
        
                try
                {
                    FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://" + serverIP + "/" + serverURL + "/");
                    ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                    ftpWebRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                    ftpWebRequest.UsePassive = true;
                    ftpWebRequest.UseBinary = true;
                    ftpWebRequest.KeepAlive = false;

                    List<DirectoryItem> returnValue = new List<DirectoryItem>();
                    string[] list = null;
                    List<string> finallist = new List<string>();
                    using (FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse())
                    using (StreamReader reader1 = new StreamReader(response.GetResponseStream()))
                    {
                        list = reader1.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < list.Length; i++)
                        {
                            string[] temporaryList = list[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < temporaryList.Length; j++)
                            {
                                if (temporaryList[j].Contains("."))
                                {
                                    finallist.Add(temporaryList[j]);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < finallist.Count; i++)
                    {
                        if (finallist[i].Contains(documentID))
                        {

                            fileData = request.DownloadData("ftp://" + serverIP + "/" + serverURL + "/" + finallist[i]);

                            track = i;
                        }

                    }
                    string[] prop = finallist[track].Split('.');
                    name = prop[0];
                    ext = prop[1];
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = finallist[track],
                        Inline = false,
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());
                    return File(fileData, "application / " + ext);
                }
                catch {
                    return RedirectToAction("BadRequest", "Home");
                }
               
            }
        }

            

          
           
        }
    }
    struct DirectoryItem
    {
        public Uri BaseUri;

        public string AbsolutePath
        {
            get
            {
                return string.Format("{0}/{1}", BaseUri, Name);
            }
        }
        public DateTime DateCreated;
        public bool IsDirectory;
        public string Name;
        public List<DirectoryItem> Items;
    }
