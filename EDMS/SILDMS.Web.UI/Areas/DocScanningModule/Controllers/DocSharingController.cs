using System;
using System.Collections.Generic;
using System.Configuration;

using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using SILDMS.Utillity;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Net.Mime;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Service.DocSharing;
using SILDMS.Model.DocScanningModule;
using System.Data;
using System.Data.SqlClient;
using SILDMS.Web.UI.Areas.SecurityModule.Models;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class DocSharingController : Controller
    {
        private readonly IDocSharingService _docSharingService;
        private readonly string _userId;
        public DocSharingController(IDocSharingService docSharingService)
        {
            _docSharingService = docSharingService;
            _userId = SILAuthorization.GetUserID();
        }
        
        
        // GET: /DocScanningModule/DocShare/
        //public ActionResult SendMail(String toEmail, string ccAddress, string bccAddress, string Subj, string Message, string ServerIP, string FileServerURL, string FtpUserName, string FtpPassword, string DocumentID, DocumentsInfo modelDocumentsInfo, List<DocMetaValue> docMetaValues, string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID)
        //{

        //    int a = 1;
        //    HostAdd = ConfigurationManager.AppSettings["Host"].ToString();
        //    FromEmailid = ConfigurationManager.AppSettings["FromMail"].ToString();
        //    Password = ConfigurationManager.AppSettings["Password"].ToString();
        //    MailMessage mailMessage = new MailMessage();
        //    mailMessage.From = new MailAddress(FromEmailid); //From Email Id
        //    mailMessage.Subject = Subj; //Subject of Email
        //    mailMessage.Body = Message; //body or message of Email
        //    mailMessage.IsBodyHtml = true;
        //    string[] toMuliId = toEmail.Split(',');
        //    foreach (string toEMailId in toMuliId)
        //    {
        //        if (!string.IsNullOrEmpty(toEMailId))
        //        {
        //            mailMessage.To.Add(new MailAddress(toEMailId)); //adding multiple TO Email Id
        //        }
        //    }
        //    string[] CCId = ccAddress.Split(',');
        //    foreach (string ccEmail in CCId)
        //    {
        //        if (!string.IsNullOrEmpty(ccEmail))
        //        {
        //            mailMessage.CC.Add(new MailAddress(ccEmail)); //Adding Multiple CC email Id
        //        }
        //    }

        //    string[] bccid = bccAddress.Split(',');
        //    foreach (string bccEmailId in bccid)
        //    {
        //        if (!string.IsNullOrEmpty(bccEmailId))
        //        {
        //            mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
        //        }
        //    }

        //    #region
        //    using (PdfDocument outPdf = new PdfDocument())
        //    {
        //        string filePath;

        //        try
        //        {
        //            string filename = DocumentID + ".pdf";
        //            filePath = "ftp://" + ServerIP + "/" + FileServerURL + "/" + filename;
        //            bool exist = CheckIfFileExistsOnServer(filePath, FtpUserName, FtpPassword);
        //            if (exist)
        //            {
        //                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filePath);//"" 
        //                request.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
        //                request.Method = WebRequestMethods.Ftp.DownloadFile;
        //                Stream contentStream = request.GetResponse().GetResponseStream();
        //                Attachment attachment = new Attachment(contentStream, filename);
        //                mailMessage.Attachments.Add(attachment);
        //            }
        //            else
        //            {

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //continue;
        //            using (StreamWriter writer = new StreamWriter("C:\\Log\\log.txt", true))
        //            {
        //                writer.WriteLine("FTP Method Call :" + ex.Message);
        //            }
        //        }
          
        //    }
        //    if (TempData["Attachment"] == null)
        //    {
        //        HttpPostedFile httpPostedFileBase = (HttpPostedFile)TempData["Attachment"];
        //        Attachment data = new Attachment(Server.MapPath("~/Buffer/inputPdf/doc.pdf"));
        //        mailMessage.Attachments.Add(data);
        //    }
        //    else
        //    {
        //        HttpPostedFile httpPostedFileBase = (HttpPostedFile)TempData["Attachment"];
        //        var attachment = new Attachment(httpPostedFileBase.InputStream, httpPostedFileBase.FileName);
        //        mailMessage.Attachments.Add(attachment);
        //        Attachment data = new Attachment(Server.MapPath("~/Buffer/inputPdf/doc.pdf"));
        //        mailMessage.Attachments.Add(data);
        //    }
        //    #endregion

        //    SmtpClient smtp = new SmtpClient(); // creating object of smptpclient
        //    smtp.Host = HostAdd; //host of emailaddress for example smtp.gmail.com etc
        //    //network and security related credentials
        //    smtp.EnableSsl = true;
        //    NetworkCredential networkCred = new NetworkCredential();
        //    networkCred.UserName = mailMessage.From.Address;
        //    networkCred.Password = Password;
        //    smtp.UseDefaultCredentials = true;
        //    smtp.Credentials = networkCred;
        //    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
        //    ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

        //    try
        //    {
        //        smtp.Send(mailMessage);
        //        a = 2;
           

        //    }
        //    catch (Exception ex)
        //    {
        //        a = 0;
        //        using (StreamWriter writer = new StreamWriter("C:\\Log\\log.txt", true))
        //        {
        //            writer.WriteLine("During Main Send Method Call :" + ex.Message);
        //        }
        //    }

        //    return Json(new { Msg = a }, JsonRequestBehavior.AllowGet);
        //}

        public bool SavedSharedData(DocumentsInfo modelDocumentsInfo, List<DocMetaValue> docMetaValues, string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID,string ShareTo)
        {
            bool isTrue = false;
  
            modelDocumentsInfo.SetBy = _userId;
            modelDocumentsInfo.ModifiedBy = modelDocumentsInfo.SetBy;

            if (modelDocumentsInfo.DidtributionOf.Equals("Original"))
            {
                foreach (var metaValue in docMetaValues)
                {
                    string QryChk = @"Select * from DSM_Share_Documents Where DocumentID='" + metaValue.DocumentID + "' And OwnerID='" + _OwnerID + "' And DocCategoryID='" + _DocCategoryID + "' And DocTypeID='" + _DocTypeID + "' And DocPropertyID='" + _DocPropertyID + "' And ShareTo='" + ShareTo + "' And SetBy='" + modelDocumentsInfo.SetBy + "' ";
                    DataTable dtChk = CommandExecute(QryChk);
                    if (dtChk.Rows.Count <= 0)
                    {
                        string Qry = @"Insert Into DSM_Share_Documents(DocumentID,OwnerID,DocCategoryID,DocTypeID,DocPropertyID,FileCodeName,ServerID,FileServerUrl,UploaderIP,SetBy,ShareTo,Status) Values" +
                            " ('" + metaValue.DocumentID + "','" + _OwnerID + "','" + _DocCategoryID + "','" + _DocTypeID + "','" + _DocPropertyID + "','','" + metaValue.ServerID + "','" + metaValue.FileServerURL + "','','" + modelDocumentsInfo.SetBy + "','" + ShareTo + "','1') ";
                        DataTable dt = CommandExecute(Qry);
                        isTrue = true;
                    }
                    else
                    {
                        isTrue = true;
                    }
                }
            }
            else
            {
                foreach (var metaValue in docMetaValues)
                {
                    string QryChk = @"Select * from DSM_Share_DocumentsVersion Where DocVersionID='" + metaValue.DocVersionID + "' And VersionNo='" + metaValue.VersionNo + "' And DocumentID='" + metaValue.DocumentID + "' And OwnerID='" + _OwnerID + "' And DocCategoryID='" + _DocCategoryID + "' And DocTypeID='" + _DocTypeID + "' And DocPropertyID='" + _DocPropertyID + "' And ShareTo='" + ShareTo + "' And SetBy='" + modelDocumentsInfo.SetBy + "' ";
                    DataTable dtChk = CommandExecute(QryChk);
                    if (dtChk.Rows.Count <= 0)
                    {
                        string Qry = @"Insert Into DSM_Share_DocumentsVersion(DocVersionID,DocumentID,VersionNo,OwnerID,DocCategoryID,DocTypeID,DocPropertyID,FileCodeName,ServerID,FileServerUrl,UploaderIP,SetBy,ShareTo,Status) Values" +
                       " ('" + metaValue.DocVersionID + "','" + metaValue.DocumentID + "'," + metaValue.VersionNo + ",'" + _OwnerID + "','" + _DocCategoryID + "','" + _DocTypeID + "','" + _DocPropertyID + "','','" + metaValue.ServerID + "','" + metaValue.FileServerURL + "','','" + modelDocumentsInfo.SetBy + "','" + ShareTo + "','1') ";
                        DataTable dt = CommandExecute(Qry);
                        isTrue = true;
                    }
                    else
                    {
                        isTrue = true;
                    }
                }
            }
            return isTrue;
        }
        private DataTable CommandExecute(string Qry)
        {
            string connString = ConfigurationManager.ConnectionStrings["AuthContext"].ToString();
            DataTable dt = new DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(connString))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(Qry, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return dt;
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
        [SILLogAttribute]
        public ActionResult SendMailWithDownloadLink(String toEmail, string ccAddress, string bccAddress, string Subj, string Message, DocumentsInfo modelDocumentsInfo, List<DocMetaValue> docMetaValues, string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID)
        {
            

            int a = 1;
           // HostAdd = ConfigurationManager.AppSettings["Host"].ToString();
           //// HostAdd = "172.16.128.42";
           // FromEmailid = ConfigurationManager.AppSettings["FromMail"].ToString();
           // Password = ConfigurationManager.AppSettings["Password"].ToString();



            HostAdd = "smtp.gmail.com";
            // HostAdd = "172.16.128.42";
            FromEmailid = "engr.msadek027@gmail.com";
            Password = "ufkd zvot doer vftb";// "fgcc xclj zzqq bqrm";



            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(FromEmailid); //From Email Id
            mailMessage.Subject = Subj; //Subject of Email
            mailMessage.IsBodyHtml = true;
            string[] toMuliId = toEmail.Split(',');
            foreach (string toEMailId in toMuliId)
            {
                if (!string.IsNullOrEmpty(toEMailId))
                {
                    mailMessage.To.Add(new MailAddress(toEMailId)); //adding multiple TO Email Id
                }
            }
            if (ccAddress != "" && ccAddress != null)
            {
                string[] CCId = ccAddress.Split(',');
                foreach (string ccEmail in CCId)
                {
                    if (!string.IsNullOrEmpty(ccEmail))
                    {
                        mailMessage.CC.Add(new MailAddress(ccEmail)); //Adding Multiple CC email Id
                    }
                }
            }
            if (bccAddress != "" && bccAddress != null)
            {
                string[] bccid = bccAddress.Split(',');
                foreach (string bccEmailId in bccid)
                {
                    if (!string.IsNullOrEmpty(bccEmailId))
                    {
                        mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                    }
                }
            }
           
            string url;
            string vServerIP = docMetaValues[0].ServerIP;
            string vFileServerURL = docMetaValues[0].FileServerURL;

            string vFtpUserName = docMetaValues[0].FtpUserName;
            string vFtpPassword = docMetaValues[0].FtpPassword;
            try
            {
           
      

                FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create("ftp://" + vServerIP + "/" + vFileServerURL + "/");
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpWebRequest.Credentials = new NetworkCredential(vFtpUserName, vFtpPassword);
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
            
                   foreach (var metaValue in docMetaValues)
                    {  
                    for (int i = 0; i < finallist.Count; i++)
                        {
                            if (modelDocumentsInfo.DidtributionOf.Equals("Original"))
                            {
                                string tempDocumentID = metaValue.DocumentID + ".pdf";
                                if (finallist[i].Contains(tempDocumentID))
                                {
                                url = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/" + "DocSharing/DownloadLink?file=" + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(finallist[i] + "doctimexp" + DateTime.Now.AddHours(24)));
                                mailMessage.Body = mailMessage.Body + Message + "<h4>To Download File go to the link below :</h4>" + "<p><a href=" + url + ">Download</a></p>" + "<p>N.B:This link is valid for 24 hours only</p>";
                                }
                             }
                        if (modelDocumentsInfo.DidtributionOf.Equals("Version"))
                        {
                            var tempDocumentID = metaValue.DocVersionID + "_v_" + metaValue.VersionNo + ".pdf";
                            if (finallist[i].Contains(tempDocumentID))
                            {
                                url = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/" + "DocSharing/DownloadLink?file=" + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(finallist[i] + "doctimexp" + DateTime.Now.AddHours(24)));
                                mailMessage.Body = mailMessage.Body + Message + "<h4>To Download File go to the link below :</h4>" + "<p><a href=" + url + ">Download</a></p>" + "<p>N.B:This link is valid for 24 hours only</p>";
                            }
                        }
                     }
                   }               
            }
            catch (Exception ex)
            {
                //continue;
                using (StreamWriter writer = new StreamWriter("C:\\Log\\log.txt", true))
                {
                    writer.WriteLine("FTP Method Call :" + ex.Message);
                }
            }

       

            SmtpClient smtp = new SmtpClient(); // creating object of smptpclient
            smtp.Host = HostAdd; //host of emailaddress for example smtp.gmail.com etc/  network and security related credentials
            smtp.EnableSsl = true;
            NetworkCredential networkCred = new NetworkCredential();
            networkCred.UserName = mailMessage.From.Address;
            networkCred.Password = Password;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCred;
           // smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            smtp.Port = Convert.ToInt32("587");
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
   
            try
                {
                   smtp.Send(mailMessage);
                    ViewBag.LoggID = vFileServerURL;
                    ViewBag.LoggResult = "";
                    ViewBag.LoggAction = "Mail Sent";
                    ViewBag.LookupTable = "DSM_Documents";
                    a = 2;

               SavedSharedData(modelDocumentsInfo, docMetaValues, _OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, toEmail);
            }
                catch (Exception ex)
                {
                    a = 0;
                    using (StreamWriter writer = new StreamWriter("C:\\Log\\log.txt", true))
                    {
                        writer.WriteLine("During Main Send Method Call :" + ex.Message);
                    }
                }
                return Json(new { Msg = a }, JsonRequestBehavior.AllowGet);
            
        }

        [HttpGet]
        public ActionResult DownloadLink(string file)
        {

            //file = file.Replace(" ", "+");
            //int ModOfBase64String = file.Length % 4;
            //if (ModOfBase64String > 0)
            //{
            //    file += new string('=', 4 - ModOfBase64String);
            //}
            //file = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(file));// 
            var base64EncodedBytes = System.Convert.FromBase64String(file);
            file = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            //file = StringEncription.Decrypt(file,true);
            string[] data = file.Split(new[] { "doctimexp" }, StringSplitOptions.None);
            DateTime ExpirationDate = DateTime.Parse(data[1]);
            if (ExpirationDate > DateTime.Now)
            {
                DSM_Documents document = null;
                string[] filedata = data[0].Split(new[] { "." }, StringSplitOptions.None);
                _docSharingService.GetDocumentByDocumentId(filedata[0], out document);
                using (WebClient request = new WebClient())
                {
                    

                    request.Credentials = new NetworkCredential(document.FtpUserName, document.FtpPassword);
                    try
                    {
                        byte[] fileData = request.DownloadData("ftp://" + document.ServerIP + "/" + document.FileServerURL + "/" + data[0] );
                       
                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = data[0],
                            Inline = false,
                        };

                        Response.AppendHeader("Content-Disposition", cd.ToString());
                        return File(fileData, "application / " +"."+filedata[1] );
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("BadRequest", "Home");
                    }
                    
                }
               
            }
            else {
                return RedirectToAction("BadRequest", "Home");
            }
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
                    TempData["Attachment"] = httpPostedFileBase;
                    return httpPostedFileBase.FileName;
                }
            }
            else
            {
                return "1";
            }

            return "0";
        }

        public string HostAdd { get; set; }

        public string FromEmailid { get; set; }

        public string Password { get; set; }
    }
}