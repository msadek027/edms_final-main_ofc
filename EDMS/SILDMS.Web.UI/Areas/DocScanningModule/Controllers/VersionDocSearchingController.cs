using System;
using SILDMS.Model.DocScanningModule;
using SILDMS.Service.VersionDocSearching;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using Microsoft.Ajax.Utilities;
using SILDMS.Service.OriginalDocSearching;
using SILDMS.Utillity;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using Newtonsoft.Json;

namespace SILDMS.Web.UI.Areas.DocScanningModule.Controllers
{
    public class VersionDocSearchingController : Controller
    {
        //test
        private ValidationResult respStatus = new ValidationResult();
        private readonly IVersionDocSearchingService _versionDocSearchingService;
        private readonly IOriginalDocSearchingService _originalDocSearchingService;
        private readonly string UserID = string.Empty;
        private string outStatus = string.Empty;

        public VersionDocSearchingController(IOriginalDocSearchingService originalDocSearchingService, IVersionDocSearchingService versionDocSearchingService)
        {
            _originalDocSearchingService = originalDocSearchingService;
            _versionDocSearchingService = versionDocSearchingService;
            UserID = SILAuthorization.GetUserID();
        }

        [SILAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<dynamic> GetVersionDocBySearchParam(string _OwnerID, string _DocCategoryID, 
            string _DocTypeID, string _DocPropertyID, string _SearchBy,int page = 1, int itemsPerPage = 5, string sortBy = "", bool reverse = false, string search = null)
        {
            List<DocSearch> VersionMeta = null;
            if (!string.IsNullOrEmpty(_OwnerID) && !string.IsNullOrEmpty(_DocCategoryID) && !string.IsNullOrEmpty(_DocTypeID) && !string.IsNullOrEmpty(_DocPropertyID) && !string.IsNullOrEmpty(_SearchBy))
            {
                await Task.Run(() => _versionDocSearchingService.GetVersionDocBySearchParam(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, UserID, out VersionMeta));
            }

            var result =   (from r in VersionMeta
                            group r by new
                            {
                                r.DocVersionID,
                                r.DocDistributionID
                            }
                            into g
                            select new
                            {
                                DocVersionID = g.Key.DocVersionID,
                                DocDistributionID=g.Key.DocDistributionID,
                                DocumentID = g.Select(o => o.DocumentID).FirstOrDefault(),
                                MetaValue = String.Join(", ", g.Select(o => o.MetaValue).Distinct()),     
                                DocPropIdentifyID = String.Join(",", g.Select(o => o.DocPropIdentifyID).Distinct()),
                                DocPropIdentifyName = String.Join(",", g.Select(o => o.DocPropIdentifyName).Distinct()),
                                OriginalReference = SILDMS.Utillity.DMSUtility.IdentifyPropertySeparator(String.Join(", ", g.Select(o => o.OriginalReference).Distinct()), g.Select(o => o.DocPropIdentifyID).Distinct().Count()),
                                FileServerURL = g.Select(o => o.FileServerURL).FirstOrDefault(),
                                ServerIP = g.Select(o => o.ServerIP).FirstOrDefault(),
                                ServerID = g.Select(o => o.ServerID).FirstOrDefault(),
                                ServerPort = g.Select(o => o.ServerPort).FirstOrDefault(),
                                FtpUserName = g.Select(o => o.FtpUserName).FirstOrDefault(),
                                FtpPassword = g.Select(o => o.FtpPassword).FirstOrDefault(),
                                VersionNo = g.Select(o => o.VersionNo).FirstOrDefault(),
                                DocPropertyID = g.Select(o => o.DocPropertyID).FirstOrDefault(),
                                DocPropertyName = g.Select(o => o.DocPropertyName).FirstOrDefault(),
                                OwnerID = g.Select(o => o.OwnerID).FirstOrDefault(),
                                DocCategoryID = g.Select(o => o.DocCategoryID).FirstOrDefault(),
                                DocTypeID = g.Select(o => o.DocTypeID).FirstOrDefault()
                            });

            //var content = result.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);
            var totalPages = result.Count();

            return Json(new { Message = "", result, totalPages }, JsonRequestBehavior.AllowGet);           
        }

        [Authorize]
        [HttpPost]
        //[SILLogAttribute]
        public async Task<dynamic> UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo)
        {
            //Session["UserID"].ToString();
            respStatus = await Task.Run(() => _versionDocSearchingService.UpdateDocMetaInfo(_modelDocumentsInfo, UserID, out outStatus));
            ViewBag.LoggID = _modelDocumentsInfo.DocumentID;
            ViewBag.LoggResult = JsonConvert.SerializeObject(_modelDocumentsInfo).ToString();
            ViewBag.LoggAction = "Edit Documents";
            ViewBag.LookupTable = "DSM_Documents";
            return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetDocPropIdentityForSpecificDocType(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _DocVersionID,string _DocDistributionID, string _SearchBy)
        {
            List<DocSearch> lstDocSearch = null;
            if (!string.IsNullOrEmpty(_OwnerID) && !string.IsNullOrEmpty(_DocCategoryID) && !string.IsNullOrEmpty(_DocTypeID) && !string.IsNullOrEmpty(_DocPropertyID) && !string.IsNullOrEmpty(_SearchBy))
            {
                await Task.Run(() => _versionDocSearchingService.GetVersionDocBySearchParam(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, UserID, out lstDocSearch));
            }

            var result = (from r in lstDocSearch
                          where r.DocVersionID == _DocVersionID && ((_DocDistributionID == null) || (r.DocDistributionID == _DocDistributionID))
                          select new
                          {
                              DocMetaID = r.DocMetaIDVersion,
                              DocPropIdentifyID = r.DocPropIdentifyID,
                              DocPropIdentifyName = r.DocPropIdentifyName,
                              MetaValue = r.MetaValue
                          }).ToList();

            return Json(sort(result, result.Count(), result.Select(o => o.DocPropIdentifyID).Distinct().Count()), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<dynamic> GetDocumentsByWildSearch(string _SearchFor,int page = 1, int itemsPerPage = 5, string sortBy = "", bool reverse = false)
        {
            List<DocSearch> VersionMeta = null;

            if (!string.IsNullOrEmpty(_SearchFor))
            {
                await Task.Run(() => _versionDocSearchingService.GetDocumentsByWildSearch(_SearchFor, UserID, out VersionMeta));
            }

            var result =   (from r in VersionMeta
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
                                OriginalReference = String.Join(", ", g.Select(o => o.OriginalReference).Distinct()),
                                DocPropIdentifyID = String.Join(",", g.Select(o => o.DocPropIdentifyID).Distinct()),
                                DocPropIdentifyName = String.Join(",", g.Select(o => o.DocPropIdentifyName).Distinct()),
                                FileServerURL = g.Select(o => o.FileServerURL).FirstOrDefault(),
                                ServerIP = g.Select(o => o.ServerIP).FirstOrDefault(),
                                ServerPort = g.Select(o => o.ServerPort).FirstOrDefault(),
                                FtpUserName = g.Select(o => o.FtpUserName).FirstOrDefault(),
                                FtpPassword = g.Select(o => o.FtpPassword).FirstOrDefault(),
                                VersionNo = g.Select(o => o.VersionNo).FirstOrDefault(),
                                OwnerID = g.Select(o => o.OwnerID).FirstOrDefault(),
                                DocCategoryID = g.Select(o => o.DocCategoryID).FirstOrDefault(),
                                DocTypeID = g.Select(o => o.DocTypeID).FirstOrDefault(),
                                DocPropertyID = g.Select(o => o.DocPropertyID).FirstOrDefault(),
                                DocPropertyName = g.Select(o => o.DocPropertyName).FirstOrDefault()
                            });

            var content =result.Skip((page-1)*itemsPerPage).Take(itemsPerPage);
            var totalPages = result.Count();

            return Json(new { content, totalPages}, JsonRequestBehavior.AllowGet);  
        }

        private ArrayList sort(dynamic list, int count, int difference)
        {
            ArrayList temp = new ArrayList();
            for (int j = 1; j <= difference; j++)
            {
                int i = j;
                while (i <= count)
                {
                    temp.Add(list[i-1]);
                    i = i + difference;
                }
            }

            return temp;          
        }

	}
}