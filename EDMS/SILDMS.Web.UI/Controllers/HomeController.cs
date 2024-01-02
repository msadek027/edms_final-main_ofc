using SILDMS.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SILDMS.Web.UI.Areas.SecurityModule;
using System.Web.UI;
using System.Net;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Text;
//using leedtools.wia;
using System.Drawing;
using System.Drawing.Imaging;

namespace SILDMS.Web.UI.Controllers
{////testc juthi
    public class HomeController : Controller
    {

        [SILAuthorize]
        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult DocumentScanning()
        {
            string UserID = SILAuthorization.GetUserID();
            TempData["UserID"] = UserID;
            return View();
        }
        //[SILAuthorize]
        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult AccountingDocumentScanning(string BatchNo, string VoucherNo, string AccountCode)
        {
            //string UserID = SILAuthorization.GetUserID();
            //TempData["UserID"] = UserID;
            return View();
        }


        [SILAuthorize]
        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult DocModification()
        {
            return View();
        }

        [SILAuthorize]
        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult DocumentUpdate()
        {
            return View();
        }

        [SILAuthorize]
        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult OriginalDocSearching()
        {
            return View();
        }



        [SILAuthorize]
        //[SILLogAttribute]
        // [OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult OriginalDocSearchingV2()
        {
            return View();
        }

        [SILAuthorize]
        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult WorkflowDocSearching()
        {
            return View();
        }

        [SILAuthorize]
        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult VersioningOriginalDoc()
        {
            return View();
        }
       
        

        [SILAuthorize]
        // [OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult VersionDocSearching()
        {
            return View();
        }

        [SILAuthorize]
        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult VersioningVersionedDoc()
        {
            return View();
        }

        [OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Server)]
        public ActionResult NotFound()
        {
            return View();
        }

        [OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Server)]
        public ActionResult BadRequest()
        {
            return View();
        }
        [SILAuthorize]
        public ActionResult DocumentDistribution()
        {
            return View();
        }

        public ActionResult NewDocMassDistribution()
        {
            return View();
        }

        public ActionResult SameDocumentDistribution()
        {
            return View();
        }

       

        //This is only a test action Method 
        public ActionResult FilePassWord()
        {
            return View();
        }
        public ActionResult OpenFilePassWord()
        {
            return View();
        }


        //New Dev By- Mir Sadequr Rahman


        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult DocumentScan()
        {
            string UserID = SILAuthorization.GetUserID();
            TempData["UserID"] = UserID;
            return View();
        }
        public ActionResult OriginalDocSearch()
        {
            
            return View();
        }
        [SILAuthorize]
        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult VersioningOriginalDocNew()
        {
            return View();
        }


        [SILAuthorize]
        // [OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult VersionDocSearchingNew()
        {
            return View();
        }

        [SILAuthorize]
        public ActionResult DocumentDistributionNew()
        {
            return View();
        }
        [SILAuthorize]
        public ActionResult DocumentDistributionSearch()
        {
            return View();
        }
        [SILAuthorize]     
        public ActionResult DocumentSharing()
        {
            return View();
        }











        [SILAuthorize]
        //[OutputCache(Duration = 1000, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult VersioningVersionedDocNew()
        {
            return View();
        }











        public string DocUpload(string server,string port,string name,string password,string path,string file_name, HttpPostedFileBase file)
        {
            try
            {
                string PureFileName = file.FileName;
                String uploadUrl = String.Format("ftp://{0}/{1}", server + ':' + port, path);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                // This example assumes the FTP site uses anonymous logon.  
                request.Credentials = new NetworkCredential(name, password);
                request.Proxy = null;
                request.KeepAlive = true;
                request.UseBinary = true;
                request.Method = WebRequestMethods.Ftp.UploadFile;
                using (Stream ftpStream = request.GetRequestStream())
                {
                    file.InputStream.CopyTo(ftpStream);
                }

                // Get the FTP server response
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                }

                return "Success";
            }catch(Exception ex)
            {
                return ex.Message;
            }
            

        }

        //public Stream GetScannedImage()
        //{
        //    var deviceManager = new DeviceManager();
        //    DeviceInfo scanner = null;

        //    // Find the scanner device
        //    foreach (DeviceInfo deviceInfo in deviceManager.DeviceInfos)
        //    {
        //        if (deviceInfo.Type == WiaDeviceType.ScannerDeviceType)
        //        {
        //            scanner = deviceInfo;
        //            break;
        //        }
        //    }

        //    if (scanner != null)
        //    {
        //        var device = scanner.Connect();
        //        var item = device.Items[1];

        //        // Configure the scan settings (optional)
        //        // For example, setting the color mode to Color
        //        var properties = item.Properties;
        //        properties["6146"].Value = 2; // 2 for Color, 1 for Grayscale

        //        // Start the scan
        //        var imageFile = (ImageFile)item.Transfer("{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}");

        //        // Save the scanned image to a memory stream
        //        var stream = new MemoryStream();
        //        imageFile.Save(stream);

        //        // Reset the scanner settings (optional)
        //        properties["6146"].Value = 1; // Reset to Grayscale

        //        // Clean up resources
        //        device.Disconnect();
        //        stream.Position = 0; // Reset stream position before returning

        //        return stream;
        //    }
        //    else
        //    {
        //        throw new Exception("Scanner not found.");
        //    }
        //}


        //  var scannedImageStream = GetScannedImage();



    }

    //public void sa()
    //{

    //    // Initialize the TWAIN library
    //    var twainManager = new TwainManager();

    //    // Select and connect to a scanner
    //    var selectedSource = twainManager.SelectSource();
    //    twainManager.OpenSource(selectedSource);

    //    // Configure scanning settings
    //    twainManager.SetResolution(300);
    //    twainManager.SetPixelType(PixelType.RGB);
    //    twainManager.SetPageSize(new PageSize(PageSizeKind.A4));

    //    // Acquire images or documents
    //    var scannedImages = twainManager.AcquireImages();

    //    // Save the scanned files
    //    foreach (var image in scannedImages)
    //    {
    //        image.Save("path/to/save/image.jpg", ImageFormat.Jpeg);
    //    }

    //    // Dispose the TWAIN manager
    //    twainManager.Dispose();
    //}


}