using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SILDMS.Service.OwnerProperIdentity;
using SILDMS.Service.DocDistribution;
using SILDMS.Service.OriginalDocSearching;
using SILDMS.Service.VersionDocSearching;
using SILDMS.Web.UI.Areas.SecurityModule;
using System.Configuration;
using System.Data.SqlClient;
using SILDMS.Model;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class DocDistributionController : Controller
    {
        private ValidationResult _respStatus = new ValidationResult();
        private readonly Utillity.Localization.ILocalizationService _localizationService;
        private readonly IDocDistributionService _docDistributionService;
        private readonly IOwnerProperIdentityService _docPropertyIdentityService;
        private readonly IVersionDocSearchingService _versionDocScanService;
        private readonly IOriginalDocSearchingService _originalDocSearchingService;
        private readonly string _userId;
        private string outStatus = string.Empty;

        private readonly string ownerLevelId;
        private readonly string ownerId;
        public DocDistributionController(IOriginalDocSearchingService originalDocSearchingService, IVersionDocSearchingService versionDocScanService, IDocDistributionService docDistributionService, IOwnerProperIdentityService docPropertyIdentityService, SILDMS.Utillity.Localization.ILocalizationService localizationService)
        {
            _originalDocSearchingService = originalDocSearchingService;
            _versionDocScanService = versionDocScanService;
            _docDistributionService = docDistributionService;
            _docPropertyIdentityService = docPropertyIdentityService;
            _localizationService = localizationService;
            _userId = SILAuthorization.GetUserID();
            ownerLevelId = SILAuthorization.GetOwnerLevelID();
            ownerId = SILAuthorization.GetOwnerID();
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

        [Authorize]
        [HttpPost]
        public async Task<dynamic> GetVersionDocBySearchParam(string _OwnerID, string _DocCategoryID,string _DocTypeID, string _DocPropertyID, string _SearchBy,int page = 1, int itemsPerPage = 5, string sortBy = null, bool reverse = false, string search = null)
        {
            List<DocSearch> lstDocSearch = null;
            if (!string.IsNullOrEmpty(_OwnerID) && !string.IsNullOrEmpty(_DocCategoryID) && !string.IsNullOrEmpty(_DocTypeID) && !string.IsNullOrEmpty(_DocPropertyID) && !string.IsNullOrEmpty(_SearchBy))
            {
                await Task.Run(() => _versionDocScanService.GetVersionDocBySearchParam(_OwnerID,_DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, _userId, out lstDocSearch));
            }

            var result =     (from r in lstDocSearch
                              group r by new
                              {
                                  r.DocVersionID,
                                  r.DocDistributionID
                              }
                              into g
                              select new
                              {
                                  DocVersionID = g.Key.DocVersionID,
                                  DocumentID = g.Select(o => o.DocumentID).FirstOrDefault(),
                                  MetaValue = String.Join(", ", g.Select(o => o.MetaValue).Distinct()),
                                  DocPropIdentifyID = String.Join(",", g.Select(o => o.DocPropIdentifyID).Distinct()),
                                  DocPropIdentifyName = String.Join(",", g.Select(o => o.DocPropIdentifyName).Distinct()),
                                  OriginalReference = DMSUtility.IdentifyPropertySeparator(String.Join(", ", g.Select(o => o.OriginalReference).Distinct()), g.Select(o => o.DocPropIdentifyID).Distinct().Count()),
                                  FileServerURL = g.Select(o => o.FileServerURL).FirstOrDefault(),
                                  ServerIP = g.Select(o => o.ServerIP).FirstOrDefault(),
                                  ServerID = g.Select(o => o.ServerID).FirstOrDefault(),
                                  ServerPort = g.Select(o => o.ServerPort).FirstOrDefault(),
                                  FtpUserName = g.Select(o => o.FtpUserName).FirstOrDefault(),
                                  FtpPassword = g.Select(o => o.FtpPassword).FirstOrDefault(),
                                  VersionNo = g.Select(o => o.VersionNo).FirstOrDefault(),
                                  DocPropertyID = g.Select(o => o.DocPropertyID).FirstOrDefault(),
                                  DocPropertyName = g.Select(o => o.DocPropertyName).FirstOrDefault()
                              });

            var content=result.Skip((page-1)*itemsPerPage).Take(itemsPerPage);
            var totalPages = result.Count();

            return Json(new { content, totalPages}, JsonRequestBehavior.AllowGet);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<dynamic> GetDocPropIdentityForSelectedDocTypes(string _OwnerID, string _DocCategoryID,string _DocTypeID, string _DocPropertyID, string _SearchBy,int page = 1, int itemsPerPage = 5, string sortBy = null, bool reverse = false, string attribute=null, string search = null)
        {
            List<DocSearch> lstDocSearch = null;
            if (!string.IsNullOrEmpty(_OwnerID) && !string.IsNullOrEmpty(_DocCategoryID) && !string.IsNullOrEmpty(_DocTypeID) && !string.IsNullOrEmpty(_DocPropertyID) && !string.IsNullOrEmpty(_SearchBy))
            {
                await Task.Run(() => _originalDocSearchingService.GetOriginalDocBySearchParam(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, _userId,page,itemsPerPage,sortBy,reverse,null,null,out lstDocSearch));
            }

            return Json(lstDocSearch, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        [SILLogAttribute]
        public dynamic AddDocumentInfo(DocumentsInfo modelDocumentsInfo, string selectedPropId, DocMetaValue docMetaValues)
        {
            List<DocMetaValue> lstMetaValues = new List<DocMetaValue>();
            List<DSM_DocPropIdentify> objDocPropIdentifies = new List<DSM_DocPropIdentify>();
            List<string> existColumnName = new List<string>();
            var action = "";

            if (ModelState.IsValid)
            {
                action = "add";
                modelDocumentsInfo.SetBy = _userId;
                modelDocumentsInfo.ModifiedBy = modelDocumentsInfo.SetBy;

                DataTable dt = (DataTable)TempData["ExcelData"];
                if (dt != null)
                {
                    _docPropertyIdentityService.GetDocPropIdentify(_userId, "", out objDocPropIdentifies);
                    var documentList = objDocPropIdentifies.Where(ob => ob.DocPropertyID == modelDocumentsInfo.DocProperty.DocPropertyID).Select(ob => ob.IdentificationAttribute).ToList();
                    var arrayNames = (from DataColumn x in dt.Columns select x.ColumnName).ToArray();

                    foreach (string item in arrayNames)
                    {
                        if (documentList.Contains(item))
                        {
                            existColumnName.Add(item);
                        }
                    }

                    if (existColumnName.Count > 0)
                    {
                        // Database and Import File match column

                        foreach (DataRow row in dt.Rows)
                        {
                            // List<DocMetaValue> lstMetaValues = new List<DocMetaValue>();

                            foreach (var columnName in existColumnName)
                            {
                                DocMetaValue ob = new DocMetaValue();
                                ob.DocumentID = docMetaValues.DocumentID;

                                ob.MetaValue = row[columnName].ToString();
                                ob.DocPropIdentifyID = (
                                                          from t in objDocPropIdentifies
                                                          where (t.DocCategoryID == modelDocumentsInfo.DocCategory.DocCategoryID) && (t.DocTypeID == modelDocumentsInfo.DocType.DocTypeID) &&
                                                                (t.DocPropertyID == modelDocumentsInfo.DocProperty.DocPropertyID) && (t.IdentificationAttribute == columnName)
                                                          select t.DocPropIdentifyID).FirstOrDefault();// docMetaValues.DocPropIdentifyID;

                                ob.DocPropertyID = selectedPropId;
                                ob.DocMetaID = docMetaValues.DocMetaID;
                                ob.DocumentID = docMetaValues.DocumentID;
                                ob.DocVersionID = docMetaValues.DocVersionID;

                                lstMetaValues.Add(ob);
                            }

                            if (lstMetaValues.Count > 0)
                            {
                                if (modelDocumentsInfo.DidtributionOf.Equals("Original"))
                                {
                                    _respStatus = _docDistributionService.AddDocumentInfo(modelDocumentsInfo, selectedPropId, lstMetaValues, action, out outStatus);
                                }
                                else
                                {
                                    _respStatus = _docDistributionService.AddDocumentInfoForVersion(modelDocumentsInfo, selectedPropId, lstMetaValues, action, out outStatus);
                                }

                                _respStatus = new ValidationResult(outStatus, _localizationService.GetResource(outStatus));
                            }

                            lstMetaValues.Clear();
                        }

                        //if (lstMetaValues.Count > 0)
                        //{
                        //    if (modelDocumentsInfo.DidtributionOf.Equals("Original"))
                        //    {
                        //        _respStatus = _docDistributionService.AddDocumentInfo(modelDocumentsInfo, selectedPropId, lstMetaValues, action, out  outStatus);
                        //    }
                        //    else
                        //    {
                        //        _respStatus = _docDistributionService.AddDocumentInfoForVersion(modelDocumentsInfo, selectedPropId, lstMetaValues, action, out outStatus);
                        //    }

                        //    _respStatus = new ValidationResult(outStatus, _localizationService.GetResource(outStatus));
                        //}
                    }
                    else
                    {
                        _respStatus = new ValidationResult("E411", _localizationService.GetResource("E411"));
                    }
                }
            }
            else
            {
                //var errors = ModelState.Values.SelectMany(v => v.Errors);
                _respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));
                //  return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.LoggID = modelDocumentsInfo.DocumentID;
            ViewBag.LoggResult = "";
            ViewBag.LoggAction = "Insert DocDistribution";
            ViewBag.LookupTable = "DSM_Documents";

            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
        }

        //string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy,
        [HttpPost]
        [Authorize]
        [SILLogAttribute]
        public dynamic AddDocumentInfoWithUser(DocumentsInfo modelDocumentsInfo, string selectedPropId, List<DocMetaValue> docMetaValues,string employeeId, string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID)
        {
            
            //List<DocMetaValue> lstMetaValues = new List<DocMetaValue>();
            //List<DSM_DocPropIdentify> objDocPropIdentifies = new List<DSM_DocPropIdentify>();
            
       
            string Str = employeeId.Replace("[", "").Replace("]", "").Replace("\"", "'");
            string[] empLoop = Str.Replace("'","").Split(',');


            for (int i = 0; i < empLoop.Length; i++)
            {
                string emp = empLoop[i];

                if (ModelState.IsValid)
                {
                    var action = "add";
                    modelDocumentsInfo.SetBy = _userId;
                    modelDocumentsInfo.ModifiedBy = modelDocumentsInfo.SetBy;

                    if (modelDocumentsInfo.DidtributionOf.Equals("Original"))
                    {
                        foreach (var metaValue in docMetaValues)
                        {

                            string QryChk = @"Select * from DSM_Distribution_Documents Where DocumentID='" + metaValue.DocumentID + "' And OwnerID='" + _OwnerID + "' And DocCategoryID='" + _DocCategoryID + "' And DocTypeID='" + _DocTypeID + "' And DocPropertyID='" + _DocPropertyID + "' And DistributionTo='" + emp + "' And SetBy='" + modelDocumentsInfo.SetBy + "' ";
                            DataTable dtChk = CommandExecute(QryChk);
                            if (dtChk.Rows.Count <= 0)
                            {
                                string Qry = @"Insert Into DSM_Distribution_Documents(DocumentID,OwnerID,DocCategoryID,DocTypeID,DocPropertyID,FileCodeName,ServerID,FileServerUrl,UploaderIP,SetBy,DistributionTo,Status) Values" +
                                    " ('" + metaValue.DocumentID + "','" + _OwnerID + "','" + _DocCategoryID + "','" + _DocTypeID + "','" + _DocPropertyID + "','','" + metaValue.ServerID + "','" + metaValue.FileServerURL + "','','" + modelDocumentsInfo.SetBy + "','" + emp + "','1') ";
                                DataTable dt = CommandExecute(Qry);
                                _respStatus.Message = "Saved Successfully.";
                                _respStatus.Status = "1";
                                _respStatus.ErrorCode = "S201";
                            }
                            else
                            {
                                _respStatus.Message = "Saved Successfully.";
                                _respStatus.Status = "1";
                                _respStatus.ErrorCode = "S201";
                            }
                        }
                    }
                    else
                    {
                        foreach (var metaValue in docMetaValues)
                        {
                            string QryChk = @"Select * from DSM_Distribution_DocumentsVersion Where DocVersionID='" + metaValue.DocVersionID + "' And VersionNo='" + metaValue.VersionNo + "' And DocumentID='" + metaValue.DocumentID + "' And OwnerID='" + _OwnerID + "' And DocCategoryID='" + _DocCategoryID + "' And DocTypeID='" + _DocTypeID + "' And DocPropertyID='" + _DocPropertyID + "' And DistributionTo='" + emp + "' And SetBy='" + modelDocumentsInfo.SetBy + "' ";
                            DataTable dtChk = CommandExecute(QryChk);
                            if (dtChk.Rows.Count <= 0)
                            {
                                string Qry = @"Insert Into DSM_Distribution_DocumentsVersion(DocVersionID,DocumentID,VersionNo,OwnerID,DocCategoryID,DocTypeID,DocPropertyID,FileCodeName,ServerID,FileServerUrl,UploaderIP,SetBy,DistributionTo,Status) Values" +
                               " ('" + metaValue.DocVersionID + "','" + metaValue.DocumentID + "'," + metaValue.VersionNo + ",'" + _OwnerID + "','" + _DocCategoryID + "','" + _DocTypeID + "','" + _DocPropertyID + "','','" + metaValue.ServerID + "','" + metaValue.FileServerURL + "','','" + modelDocumentsInfo.SetBy + "','" + emp + "','1') ";
                                DataTable dt = CommandExecute(Qry);

                                _respStatus.Message = "Saved Successfully.";
                                _respStatus.Status = "1";
                                _respStatus.ErrorCode = "S201";
                            }
                            else
                            {
                                _respStatus.Message = "Saved Successfully.";
                                _respStatus.Status = "1";
                                _respStatus.ErrorCode = "S201";
                            }
                        }
                    }
                }
           
            else
            {               
                _respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));
             
            }
            }
            ViewBag.LoggID = modelDocumentsInfo.DocumentID;
            ViewBag.LoggResult = "";
            ViewBag.LoggAction = "Insert DocDistribution";
            ViewBag.LookupTable = "DSM_Documents";

            return Json(new { Message = _respStatus.Message, _respStatus }, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public async Task<dynamic> GetDistributionOriginalDocuments(string FromDate,string ToDate)
        {
            List<DocSearch> lstDocSearch = null;      
          
            //  exec dbo.GetDistributionDocuments @UserID='16060900001' , @FromDate='2023-01-01', @ToDate='2023-10-16'
            string Qry = @" Exec dbo.GetDistributionDocuments @UserID='" + _userId + "', @FromDate='" + FromDate + "', @ToDate='" + ToDate + "'";
            DataTable dt = CommandExecute(Qry);

            lstDocSearch = (from DataRow row in dt.Rows
                            select new DocSearch
                            {
                                DocumentID = row["DocumentID"].ToString(),
                                FileCodeName = row["FileCodeName"].ToString(),
                                DocPropIdentifyID = row["DocPropIdentifyID"].ToString(),
                                DocPropIdentifyName = row["DocPropIdentifyName"].ToString(),
                                MetaValue = row["MetaValue"].ToString(),
                                DocPropertyName = row["DocPropertyName"].ToString(),

                                FileServerURL = row["FileServerURL"].ToString(),
                                ServerIP = row["ServerIP"].ToString(),
                                ServerPort = row["ServerPort"].ToString(),
                                FtpUserName = row["FtpUserName"].ToString(),
                                FtpPassword = row["FtpPassword"].ToString(),

                                VersionNo = row["DocumentID"].ToString(),
                                DocDistributionID = row["DocDistributionID"].ToString(),

                                OwnerID = row["OwnerID"].ToString(),
                                DocCategoryID = row["DocCategoryID"].ToString(),
                                DocTypeID = row["DocTypeID"].ToString(),
                                DocPropertyID = row["DocPropertyID"].ToString(),
                                Status = row["Status"].ToString(),
                                OwnerLevelID = row["OwnerLevelID"].ToString(),
                                DistributionTo = row["Employee"].ToString(),


                            }).ToList();
     

            return Json(new { lstDocSearch }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetDistributionVersionDocuments(string FromDate, string ToDate)
        {

            List<DocSearch> lstDocSearch = null;
            string connString = ConfigurationManager.ConnectionStrings["AuthContext"].ToString();
            //  exec dbo.GetDistributionDocumentsVersion @UserID='16060900001' , @FromDate='2023-01-01', @ToDate='2023-10-16'
            string Qry = @" Exec dbo.GetDistributionDocumentsVersion @UserID='" + _userId + "', @FromDate='"+ FromDate + "', @ToDate='"+ ToDate + "'";
            DataTable dt = CommandExecute(Qry);

            lstDocSearch = (from DataRow row in dt.Rows
                            select new DocSearch
                            {
                                DocumentID = row["DocumentID"].ToString(),
                                DocVersionID = row["DocVersionID"].ToString(),
                                FileCodeName = row["FileCodeName"].ToString(),
                                DocPropIdentifyID = row["DocPropIdentifyID"].ToString(),
                                DocPropIdentifyName = row["DocPropIdentifyName"].ToString(),
                                MetaValue = row["MetaValue"].ToString(),
                                DocPropertyName = row["DocPropertyName"].ToString(),

                                FileServerURL = row["FileServerURL"].ToString(),
                                ServerIP = row["ServerIP"].ToString(),
                                ServerPort = row["ServerPort"].ToString(),
                                FtpUserName = row["FtpUserName"].ToString(),
                                FtpPassword = row["FtpPassword"].ToString(),

                                VersionNo = row["VersionNo"].ToString(),
                                DocDistributionID = row["DocDistributionID"].ToString(),
                                OwnerID = row["OwnerID"].ToString(),
                                DocCategoryID = row["DocCategoryID"].ToString(),
                                DocTypeID = row["DocTypeID"].ToString(),
                                DocPropertyID = row["DocPropertyID"].ToString(),
                                Status = row["Status"].ToString(),
                                OwnerLevelID = row["OwnerLevelID"].ToString(),
                                DistributionTo = row["Employee"].ToString(),

                            }).ToList();


            return Json(new { lstDocSearch }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetShareOriginalDocuments(string FromDate, string ToDate)
        {
            List<DocSearch> lstDocSearch = null;

            //  exec dbo.GetDistributionDocuments @UserID='16060900001' , @FromDate='2023-01-01', @ToDate='2023-10-16'
            string Qry = @" Exec dbo.GetShareDocuments @UserID='" + _userId + "', @FromDate='" + FromDate + "', @ToDate='" + ToDate + "'";
            DataTable dt = CommandExecute(Qry);

            lstDocSearch = (from DataRow row in dt.Rows
                            select new DocSearch
                            {
                                DocumentID = row["DocumentID"].ToString(),
                                FileCodeName = row["FileCodeName"].ToString(),
                                DocPropIdentifyID = row["DocPropIdentifyID"].ToString(),
                                DocPropIdentifyName = row["DocPropIdentifyName"].ToString(),
                                MetaValue = row["MetaValue"].ToString(),
                                DocPropertyName = row["DocPropertyName"].ToString(),

                                FileServerURL = row["FileServerURL"].ToString(),
                                ServerIP = row["ServerIP"].ToString(),
                                ServerPort = row["ServerPort"].ToString(),
                                FtpUserName = row["FtpUserName"].ToString(),
                                FtpPassword = row["FtpPassword"].ToString(),

                                VersionNo = row["DocumentID"].ToString(),
                                DocDistributionID = row["DocDistributionID"].ToString(),

                                OwnerID = row["OwnerID"].ToString(),
                                DocCategoryID = row["DocCategoryID"].ToString(),
                                DocTypeID = row["DocTypeID"].ToString(),
                                DocPropertyID = row["DocPropertyID"].ToString(),
                                Status = row["Status"].ToString(),
                                OwnerLevelID = row["OwnerLevelID"].ToString(),
                                DistributionTo = row["Employee"].ToString(),


                            }).ToList();


            return Json(new { lstDocSearch }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetShareVersionDocuments(string FromDate, string ToDate)
        {
            List<DocSearch> lstDocSearch = null;
            string connString = ConfigurationManager.ConnectionStrings["AuthContext"].ToString();   
            string Qry = @" Exec dbo.GetShareDocumentsVersion @UserID='" + _userId + "', @FromDate='" + FromDate + "', @ToDate='" + ToDate + "'";
            DataTable dt = CommandExecute(Qry);

            lstDocSearch = (from DataRow row in dt.Rows
                            select new DocSearch
                            {
                                DocumentID = row["DocumentID"].ToString(),
                                DocVersionID = row["DocVersionID"].ToString(),
                                FileCodeName = row["FileCodeName"].ToString(),
                                DocPropIdentifyID = row["DocPropIdentifyID"].ToString(),
                                DocPropIdentifyName = row["DocPropIdentifyName"].ToString(),
                                MetaValue = row["MetaValue"].ToString(),
                                DocPropertyName = row["DocPropertyName"].ToString(),

                                FileServerURL = row["FileServerURL"].ToString(),
                                ServerIP = row["ServerIP"].ToString(),
                                ServerPort = row["ServerPort"].ToString(),
                                FtpUserName = row["FtpUserName"].ToString(),
                                FtpPassword = row["FtpPassword"].ToString(),

                                VersionNo = row["VersionNo"].ToString(),
                                DocDistributionID = row["DocDistributionID"].ToString(),
                                OwnerID = row["OwnerID"].ToString(),
                                DocCategoryID = row["DocCategoryID"].ToString(),
                                DocTypeID = row["DocTypeID"].ToString(),
                                DocPropertyID = row["DocPropertyID"].ToString(),
                                Status = row["Status"].ToString(),
                                OwnerLevelID = row["OwnerLevelID"].ToString(),
                                DistributionTo = row["Employee"].ToString(),

                            }).ToList();


            return Json(new { lstDocSearch }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetEmployee()
        {            
            string Qry = @"Select UserID, EmployeeID,UserFullName from SEC_User Where Status='1' AND UserFullName Is Not Null AND OwnerLevelID='"+ownerLevelId+ "' AND UserID NOT IN('"+_userId+"')";
            DataTable dt = CommandExecute(Qry);         
            List<DefaultBEL> data;
            data = (from DataRow row in dt.Rows
                    select new DefaultBEL
                    {
                        EmployeeId = row["UserID"].ToString(),
                        EmployeeName = row["UserFullName"].ToString(),

                    }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
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



    }
}