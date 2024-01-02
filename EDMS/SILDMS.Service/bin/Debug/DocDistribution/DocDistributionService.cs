using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Utillity.Localization;
using SILDMS.DataAccessInterface.DocDistribution;
using System.Net;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Policy;
using System.Xml.Linq;

namespace SILDMS.Service.DocDistribution
{
    public class DocDistributionService : IDocDistributionService
    {
        private string _errorNumber = "";

        public DocDistributionService(IDocDistributionDataService docDistDataService, ILocalizationService localizationService)
        {
            _docDistDataService = docDistDataService;
            _localizationService = localizationService;
        }


        public IDocDistributionDataService _docDistDataService { get; set; }

        public ILocalizationService _localizationService { get; set; }



        public ValidationResult AddDocumentInfoForVersion(DocumentsInfo modelDocumentsInfo, string selectedPropId, List<DocMetaValue> lstMetaValues, string action, out string outStatus)
        {
            _docDistDataService.AddDocumentInfoForVersion(modelDocumentsInfo, selectedPropId, lstMetaValues, action, out outStatus);
            if (outStatus.Length > 0)
            {
                return new ValidationResult(outStatus, _localizationService.GetResource(outStatus));
            }
            return ValidationResult.Success;
        }

        public ValidationResult AddDocumentInfo(DocumentsInfo modelDocumentsInfo, string selectedPropId, List<DocMetaValue> lstMetaValues, string action, out string outStatus)
        {
            _docDistDataService.AddDocumentInfo(modelDocumentsInfo, selectedPropId, lstMetaValues, action, out outStatus);
            if (outStatus.Length > 0)
            {
                return new ValidationResult(outStatus, _localizationService.GetResource(outStatus));
            }
            return ValidationResult.Success;
        }

        public ValidationResult GetDoc(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string value, string UserID, out List<DocSearch> docList)
        {
            docList = _docDistDataService.GetDoc(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, value, UserID, out _errorNumber);
            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }
            return ValidationResult.Success;
        }

        public async Task ServerFileUpload(DocSearch docSearch, string sourcePath) {
            try {
                WebClient client = new WebClient();
                client.Credentials = new NetworkCredential(docSearch.FtpUserName, docSearch.FtpPassword);
                string filePath = "ftp://" + docSearch.ServerIP + "/" + docSearch.FileServerURL + "/" + docSearch.DocumentID + ".pdf";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filePath);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(docSearch.FtpUserName, docSearch.FtpPassword);
                string srcPath = sourcePath;

                try
                {
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine("File deleted: " + response.StatusDescription);
                        byte[] fileBytes = File.ReadAllBytes(@sourcePath);
                        client.UploadData(filePath, "STOR", fileBytes);
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine("Error deleting file: " + ex.Message);
                }
            }
            catch (Exception ex)
            {

            };

        }

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

        public void ServerFileDelete(string sourcePath)
        { 
            try
            {
                if (File.Exists(sourcePath))
                {
                    File.Delete(sourcePath);
        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting file: " + ex.Message);
            }
        }
    }
    
}
