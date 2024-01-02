using iTextSharp.text;
using iTextSharp.text.pdf;
using SILDMS.Model.WorkflowModule;
using SILDMS.Service.WorkflowDocSearching;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IWorkflowDocSearchingService _workflowDocSearchingService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult respStatus = new ValidationResult();
        private readonly string UserID = string.Empty;
        private string res_code = string.Empty;
        private string res_message = string.Empty;

        public DownloadController(IWorkflowDocSearchingService workflowDocSearchingService, ILocalizationService localizationService)
        {
            UserID = SILAuthorization.GetUserID();
            _workflowDocSearchingService = workflowDocSearchingService;
        }

        public FileResult DownloadDocument(string serverIP, string ftpPort, string ftpUserName, string ftpPassword, string serverURL, string documentID, string Ext)
        {
            using (WebClient request = new WebClient())
            {
                if (Ext == "")
                {
                    Ext = "pdf";
                }

                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

                string fullUrl = "ftp://" + serverIP + "/" + serverURL + "/" + documentID + "." + Ext;
                byte[] fileData = request.DownloadData(fullUrl);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = documentID+"."+Ext,
                    Inline = false,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(fileData, "application / "+Ext);
            }
        }

        public FileResult DownloadVersionDocument(string serverIP, string ftpPort, string ftpUserName, string ftpPassword, string serverURL, string documentID, string versionNO, string Ext)
        {
            using (WebClient request = new WebClient())
            {
                if (Ext == "")
                {
                    Ext = "pdf";
                }                
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                string fullUrl = "ftp://" + serverIP + "/" + serverURL + "/" + documentID + "_v_" + versionNO + "." + Ext;
                byte[] fileData = request.DownloadData(fullUrl);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = documentID + "." + Ext,
                    Inline = false,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(fileData, "application / " + Ext);
            }
        }

        //[HttpPost]
        //[SILLogAttribute]
        //public string GetDocument(string serverIP, string serverPort, string serverURL, string ftpUserName, string ftpUserPassword, string documentID, string documentExtension)
        //{
        //    using (WebClient request = new WebClient())
        //    {
        //        if (documentExtension == "")
        //        {
        //            documentExtension = "pdf";
        //        }
        //        request.Credentials = new NetworkCredential(ftpUserName, ftpUserPassword);
        //        byte[] fileData = request.DownloadData("ftp://" + serverIP + "/" + serverURL + "/" + documentID + "." + documentExtension);
        //        ViewBag.LoggID = documentID;
        //        ViewBag.LoggResult = "";
        //        ViewBag.LoggAction = "Print Document";
        //        ViewBag.LookupTable = "DSM_Documents";

        //        return Convert.ToBase64String(fileData);
        //    }
        //}

        public FileResult DownloadWorkflowDocument(string ObjectID)
        {
            VM_DocumentsPropertyValuesAll obj = new VM_DocumentsPropertyValuesAll();

            _workflowDocSearchingService.GetDocumentPropertyValues(UserID, ObjectID, out obj);

            MemoryStream subStream = new MemoryStream();
            Document attrDoc = new Document(PageSize.A4, 40, 40, 40, 10);
            PdfWriter.GetInstance(attrDoc, subStream).CloseStream = false;
            attrDoc.Open();

            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false);
            Font F_Header = new Font(bfTimes, 14, Font.BOLD, BaseColor.BLACK);

            Font F_SubHeader = new Font(bfTimes, 12, Font.BOLD, BaseColor.BLUE);

            Font F_Body_Bold = new Font(bfTimes, 12, Font.BOLD, BaseColor.BLACK);

            Font F_Body_Normal = new Font(bfTimes, 12, Font.NORMAL, BaseColor.BLACK);

            Image jpg = Image.GetInstance(Request.Url.GetLeftPart(UriPartial.Authority) + "/Images/one_logo_png.png");
            jpg.ScaleToFit(127f, 40f);
            //jpg.SpacingBefore = 1f;
            //jpg.SpacingAfter = 1f;
            jpg.Alignment = Element.ALIGN_CENTER;
            attrDoc.Add(jpg);

            Paragraph Header = new Paragraph("Document Details of " + obj.LevelName + "/" + obj.OwnerName + "/" + obj.DocCategoryName + "/" + obj.DocTypeName, F_Header);
            Header.Alignment = Element.ALIGN_CENTER;
            Header.SpacingAfter = 5;
            attrDoc.Add(Header);

            Paragraph AttrHeader = new Paragraph("Identification Attribute List", F_SubHeader);
            AttrHeader.Alignment = Element.ALIGN_LEFT;
            attrDoc.Add(AttrHeader);

            //RomanList listAttr = new RomanList(true, 20);
            //listAttr.IndentationLeft = 35f;

            string propValue = "";
            Paragraph p = new Paragraph();
            Phrase phrase;

            for (int i = 0; i < obj.ObjectProperties.Count; i++)
            {
                phrase = new Phrase();
                propValue = (obj.ObjectProperties[i].PropertyValue == null || obj.ObjectProperties[i].PropertyValue == "") ? "N/A" : obj.ObjectProperties[i].PropertyValue;

                phrase.Add(new Chunk((i + 1).ToString() + ". ", F_Body_Normal));
                phrase.Add(new Chunk(obj.ObjectProperties[i].PropertyName + " : ", F_Body_Bold));
                phrase.Add(new Chunk(propValue, F_Body_Normal));
                phrase.Add(new Chunk("\n"));

                p.Add(phrase);
            }

            p.IndentationLeft = 35f;
            attrDoc.Add(p);

            Paragraph DocListHead = new Paragraph("Documents Provided", F_SubHeader);
            DocListHead.Alignment = Element.ALIGN_LEFT;
            DocListHead.Font = FontFactory.GetFont(FontFactory.HELVETICA, 20f, BaseColor.BLUE);
            attrDoc.Add(DocListHead);

            int x = 0;
            Paragraph pd = new Paragraph();

            foreach (var item in obj.ObjectDocuments)
            {
                phrase = new Phrase();
                phrase.Add(new Chunk((x + 1).ToString() + ". ", F_Body_Normal));
                phrase.Add(new Chunk(item.DocPropertyName, F_Body_Normal));
                phrase.Add(new Chunk("\n"));
                pd.Add(phrase);
                x++;
            }

            pd.IndentationLeft = 35f;

            attrDoc.Add(pd);

            Paragraph HistoryHead = new Paragraph("History", F_SubHeader);
            HistoryHead.Alignment = Element.ALIGN_LEFT;
            HistoryHead.Font = FontFactory.GetFont(FontFactory.HELVETICA, 20f, BaseColor.BLUE);
            attrDoc.Add(HistoryHead);

            int t = 0;
            Paragraph ht = new Paragraph();
            string hisDetail = "";

            foreach (var item in obj.StageChangeHistory)
            {
                phrase = new Phrase();
                phrase.Add(new Chunk((t + 1).ToString() + ". ", F_Body_Normal));

                if (item.TypeOfChange == 2)
                {
                    hisDetail = "Generated by " + item.SetBy + " on " + item.SetOn + " at stage " + item.FromStageName;
                }
                else if (item.TypeOfChange == 3)
                {
                    hisDetail = "Completed by " + item.SetBy + " on " + item.SetOn + " at stage " + item.FromStageName;
                }
                else if (item.TypeOfChange == 1)
                {
                    if (!item.IsMakeOrCheck)
                    {
                        if (item.FromStage == item.ToStage)
                        {
                            hisDetail = "Upgrated by " + item.SetBy + " on " + item.SetOn + " at stage " + item.FromStageName + ".";
                        }
                        else
                        {
                            hisDetail = "Upgrated by " + item.SetBy + " on " + item.SetOn + " from stage " + item.FromStageName + " to " + item.ToStageName + ".";
                        }
                    }
                    else
                    {
                        if (item.FromStage == item.ToStage)
                        {
                            hisDetail = "Approved by " + item.SetBy + " on " + item.SetOn + " at stage " + item.FromStageName + ".";
                        }
                        else
                        {
                            hisDetail = "Approved by " + item.SetBy + " on " + item.SetOn + " from stage " + item.FromStageName + " to " + item.ToStageName + ".";
                        }
                    }
                }
                else
                {
                    if (!item.IsMakeOrCheck)
                    {
                        if (item.FromStage == item.ToStage)
                        {
                            hisDetail = "Reverted by " + item.SetBy + " on " + item.SetOn + " at stage " + item.FromStageName + " (Make). Reason - " + item.Reason;
                        }
                        else
                        {
                            hisDetail = "Reverted by " + item.SetBy + " on " + item.SetOn + " from stage " + item.FromStageName + " (Make) to " + item.ToStageName + ". Reason - " + item.Reason;
                        }
                    }
                    else
                    {
                        if (item.FromStage == item.ToStage)
                        {
                            hisDetail = "Reverted by " + item.SetBy + " on " + item.SetOn + " at stage " + item.FromStageName + " (Check). Reason - " + item.Reason;
                        }
                        else
                        {
                            hisDetail = "Reverted by " + item.SetBy + " on " + item.SetOn + " from  stage " + item.FromStageName + " (Check) to " + item.ToStageName + ". Reason - " + item.Reason;
                        }
                    }
                }

                phrase.Add(new Chunk(hisDetail, F_Body_Normal));
                phrase.Add(new Chunk("\n"));
                ht.Add(phrase);
                hisDetail = "";
                t++;
            }

            ht.IndentationLeft = 35f;
            attrDoc.Add(ht);
            attrDoc.Close();

            MemoryStream mainStream = new MemoryStream();
            Document doc = new Document();

            var copy = new PdfSmartCopy(doc, mainStream);
            doc.Open();
            copy.AddDocument(new PdfReader(subStream.ToArray()));
            subStream.Close();

            if (subStream != null)
            {
                subStream = null;
            }

            WebClient request = new WebClient();

            foreach (var item in obj.ObjectDocuments)
            {
                request.Credentials = new NetworkCredential(item.FtpUserName, item.FtpPassword);

                try
                {
                    byte[] fileData = request.DownloadData("ftp://" + item.ServerIP + "/" + item.FileServerURL + "/" + item.DocumentID + ".pdf");
                    using (var reader = new PdfReader(fileData))
                    {
                        copy.AddDocument(reader);
                    }
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            request.Dispose();
            doc.Close();

            byte[] byteInfo = mainStream.ToArray();
            mainStream.Close();

            if (mainStream != null)
            {
                mainStream = null;
            }

            return File(byteInfo, "application/pdf", "file.pdf");
        }

    }
}