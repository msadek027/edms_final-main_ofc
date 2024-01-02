using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using SILDMS.Service.DocMkCkStage;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Areas.SecurityModule;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Areas.WorkflowModule.Controllers
{
    public class DocMkCkStageController : Controller
    {
        private readonly IDocMkCkStageService _docDocMkCkStageService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult respStatus = new ValidationResult();
        private readonly string UserID = string.Empty;
        private string res_code = string.Empty;
        private string res_message = string.Empty;
        private string action = "";

        public DocMkCkStageController(IDocMkCkStageService docDocMkCkStageService, ILocalizationService localizationService)
        {
            _docDocMkCkStageService = docDocMkCkStageService;
            _localizationService = localizationService;
            UserID = SILAuthorization.GetUserID();
        }
        // GET: DocScanningModule/DocMkCkStage
        public ActionResult Index()
        {
            return View();
        }

        public async Task<dynamic> GetStageAndUserPermission(int stageMapID)
        {
            var obj = new WFM_ProcessStageMap();
            await Task.Run(() => _docDocMkCkStageService.GetStageAndUserPermission(stageMapID, UserID, out obj));

            return Json(new { obj, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetDocumentProperty(string _DocCategoryID, string _OwnerID, string _DocTypeID, int _StageMapID)
        {
            VM_DocumentsProperty obj = new VM_DocumentsProperty();
            await Task.Run(() => _docDocMkCkStageService.GetALLDocsProp(_DocCategoryID, _OwnerID, _DocTypeID, _StageMapID, out obj));

            return Json(new { Msg = "", obj }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetMkCkDocuments(string _OwnerID, string _DocCategoryID,string _DocTypeID, int _StageMapID, bool isUserMaker, bool isUserChecker, int page = 1, int itemsPerPage = 5, string sortBy = "[ObjectID]", bool reverse = false, string search = "")
        {
            DataTable dt = null;
            int totalPages = 0;

            await Task.Run(() => _docDocMkCkStageService.GetMkCkDocuments(_OwnerID, _DocCategoryID, _DocTypeID, _StageMapID, UserID, page, itemsPerPage, sortBy, reverse, search, out dt));

            string html = "";

            if (dt.Columns.Count > 0)
            {
                html += "<table class=\"table table-condensed table-bordered table-striped table-hover pnlView\">";
                html += "<thead><tr>";

                for (int i = 0; i <= dt.Columns.Count - 4; i++)
                    html += "<th>" + dt.Columns[i].ColumnName + "</th>";

                html += "<th>Action</th>";
                html += "</tr>";
                //html += "<tr><th colspan=\"" + (dt.Columns.Count - 3) / 2 + "\"><input placeholder = \"Search for...\" ng-keypress=\"search($event)\" ng-model=\"pagingInfo.search\" class=\"form-control\"/></th></tr>";
                html += "</thead><tbody>";

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (isUserMaker && !Convert.ToBoolean(dt.Rows[i]["mkstatus"]) && Convert.ToBoolean(dt.Rows[i]["ckstatus"]))
                        {
                            html += "<tr>";
                            for (int j = 0; j <= dt.Columns.Count - 4; j++)
                            {
                                html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                            }

                            html += "<td>";
                            html += "<button type=\"button\" class=\"btn btn-info btn-sm\" ng-click=\"docMakeCheckVM.ShowMakeModal('" + dt.Rows[i][0].ToString() + "')\">Make</button>";
                            html += "</td>";
                            html += "</tr>";
                        }
                      else if (isUserMaker && !Convert.ToBoolean(dt.Rows[i]["mkstatus"]) && !Convert.ToBoolean(dt.Rows[i]["ckstatus"]))
                        {
                            html += "<tr>";
                            for (int j = 0; j <= dt.Columns.Count - 4; j++)
                            {
                                html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                            }

                            html += "<td>";
                            html += "<button type=\"button\" class=\"btn btn-info btn-sm\" ng-click=\"docMakeCheckVM.ShowMakeModal('" + dt.Rows[i][0].ToString() + "')\">Make</button>";
                            html += "</td>";
                            html += "</tr>";
                        }
                        else if (isUserChecker && Convert.ToBoolean(dt.Rows[i]["mkstatus"]) && !Convert.ToBoolean(dt.Rows[i]["ckstatus"]))
                        {
                            html += "<tr>";
                            for (int j = 0; j <= dt.Columns.Count - 4; j++)
                            {
                                html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                            }

                            html += "<td>";
                            html += "<button type=\"button\" class=\"btn btn-info btn-sm\" ng-click=\"docMakeCheckVM.ShowCheckModal('" + dt.Rows[i][0].ToString() + "')\">Check</button>";
                            html += "</td>";
                            html += "</tr>";
                        }
                    }

                    totalPages = Convert.ToInt32(dt.Rows[0]["COUNT"]);
                }

                html += "</tbody></table>";
            }
            else
            {
                html += "<div class=\"text-center\"><img src =\"/Images/no_results.png\" alt =\"Smiley face\" height = \"300\" width = \"500\"></div>";
            }

            return Json(new { html, totalPages }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetDocumentPropertyValuesMake(string _ObjectID ,string _StageMapID)
        {
            VM_DocumentsPropertyValues obj = new VM_DocumentsPropertyValues();
            await Task.Run(() => _docDocMkCkStageService.GetDocumentPropertyValues(UserID, _ObjectID, _StageMapID,out obj));

            var updatePropertyCollection = obj.ObjectProperties;
            var ParentStages = obj.ParentStages;
            var updateDocumentCollection = obj.ObjectDocuments.Where(p => p.DocumentID != null && p.DocumentID != "");
            var newDocumentCollection = obj.ObjectDocuments.Where(p => p.DocumentID == null || p.DocumentID == "");

            DataTable dt = null;
            string html = "";
            List<string> listPropHtml = new List<string>();

            for (int k = 0; k < obj.ListProperties.Count; k++)
            {
                html = "";
                dt = obj.ListProperties[k];

                if (dt.Columns.Count > 0)
                {
                    html += "<table class=\"table table-condensed table-bordered table-striped table-hover pnlView\">";
                    html += "<thead><tr>";

                    for (int i = 0; i < dt.Columns.Count - 1; i++)
                        html += "<th>" + dt.Columns[i].ColumnName + "</th>";

                    //html += "<th>Action</th>";
                    html += "</tr>";
                    html += "</thead><tbody>";

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            html += "<tr>";
                            for (int j = 0; j < dt.Columns.Count - 1; j++)
                            {
                                html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                            }

                            //html += "<td>";
                            //html += "<button type=\"button\" class=\"btn btn-info btn-sm\" ng-click=\"docMakeCheckVM.DeleteListItem('" + dt.Rows[i][dt.Columns.Count - 1].ToString() + "','" + dt.Rows[i][0].ToString() + "')\">Delete</button>";
                            //html += "</td>";
                            html += "</tr>";
                        }
                    }

                    html += "</tbody></table>";
                }
                else
                {
                    html += "<div class=\"text-center\"><img src =\"/Images/no_results.png\" alt =\"Smiley face\" height = \"300\" width = \"500\"></div>";
                }

                //html += "<div><button type=\"button\" class=\"btn btn-info btn-sm\" ng-click=\"docMakeCheckVM.toggleAddNew('" + dt.Columns[dt.Columns.Count - 1].ColumnName.ToString() + "')\">Add New Item</button></div>";
                listPropHtml.Add(html);
            }

            var IsBacked = obj.IsBacked;
            var BackReason = obj.BackReason;

            return Json(new { Msg = "", updatePropertyCollection, updateDocumentCollection, newDocumentCollection, listPropHtml, IsBacked, BackReason, ParentStages }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetDocumentPropertyValuesCheck(string _ObjectID,string _StageMapID)
        {
            VM_DocumentsPropertyValues obj = new VM_DocumentsPropertyValues();
            await Task.Run(() => _docDocMkCkStageService.GetDocumentPropertyValues(UserID, _ObjectID, _StageMapID, out obj));

            var updatePropertyCollection = obj.ObjectProperties;
            var ParentStages = obj.ParentStages;
            foreach (var Document in obj.ObjectDocuments) {

                foreach (var pd in obj.PermittedDocuments) {

                    if (pd.DocPropertyID == Document.DocPropertyID)
                    {

                        Document.IsPermitted = true;
                        break;
                    }
                    else
                    {
                        Document.IsPermitted = false;
                    }
                }
            }
            var allDocumentCollection = obj.ObjectDocuments;

            DataTable dt = null;
            string html = "";
            List<string> listPropHtml = new List<string>();

            for (int k = 0; k < obj.ListProperties.Count; k++)
            {
                html = "";
                dt = obj.ListProperties[k];

                if (dt.Columns.Count > 0)
                {
                    html += "<table class=\"table table-condensed table-bordered table-striped table-hover pnlView\">";
                    html += "<thead><tr>";

                    for (int i = 0; i < dt.Columns.Count - 1; i++)
                        html += "<th>" + dt.Columns[i].ColumnName + "</th>";

                    html += "</tr>";
                    html += "</thead><tbody>";

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            html += "<tr>";
                            for (int j = 0; j < dt.Columns.Count - 1; j++)
                            {
                                html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                            }

                            html += "</tr>";
                        }
                    }

                    html += "</tbody></table>";
                }
                else
                {
                    html += "<div class=\"text-center\"><img src =\"/Images/no_results.png\" alt =\"Smiley face\" height = \"300\" width = \"500\"></div>";
                }

                listPropHtml.Add(html);
            }

            var IsBacked = obj.IsBacked;
            var BackReason = obj.BackReason;

            return Json(new { Msg = "", updatePropertyCollection, allDocumentCollection, listPropHtml, IsBacked, BackReason, ParentStages }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetDocumentListPropertyValuesCheck(string _ObjectID, string _TableRefID)
        {
            VM_ListPropertyCheck obj = new VM_ListPropertyCheck();
            await Task.Run(() => _docDocMkCkStageService.GetDocumentListPropertyValuesCheck(_ObjectID, _TableRefID, out obj));

            string html = "";

            if (obj.ListPropertyBody.Columns.Count > 0)
            {
                html += "<table class=\"table table-condensed table-bordered table-striped table-hover pnlView\">";
                html += "<thead><tr>";

                for (int i = 0; i < obj.ListPropertyBody.Columns.Count; i++)
                    html += "<th>" + obj.ListPropertyBody.Columns[i].ColumnName + "</th>";

                html += "</tr>";
                html += "</thead><tbody>";

                if (obj.ListPropertyBody.Rows.Count > 0)
                {
                    for (int i = 0; i < obj.ListPropertyBody.Rows.Count; i++)
                    {
                        html += "<tr>";
                        for (int j = 0; j < obj.ListPropertyBody.Columns.Count; j++)
                        {
                            html += "<td>" + obj.ListPropertyBody.Rows[i][j].ToString() + "</td>";
                        }

                        html += "</tr>";
                    }
                }

                html += "</tbody></table>";
            }
            else
            {
                html += "<div class=\"text-center\"><img src =\"/Images/no_results.png\" alt =\"Smiley face\" height = \"300\" width = \"500\"></div>";
            }

            return Json(html, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public async Task<dynamic> UpdateDocumentInfo(string objectID, string docs, List<ObjectProperties> props)
        {
            List<DSM_Documents> objs = null;

            if (ModelState.IsValid)
            {
                respStatus.Message = "Success";
                respStatus = await Task.Run(() => _docDocMkCkStageService.UpdateDocumentInfo(objectID, docs, props, UserID, GetIPAddress.LocalIPAddress(), out objs));


                foreach (var item in objs)
                {
                    try
                    {
                        FolderGenerator.MakeFTPDir(item.ServerIP, item.ServerPort, item.FileServerURL, item.FtpUserName, item.FtpPassword);
                    }
                    catch (Exception e)
                    {

                    }
                }

                return Json(new { Message = "Success", result = objs, distinctIDs = objs.Select(x => x.DocumentID) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                respStatus = new ValidationResult("E404", _localizationService.GetResource("E404"));
                return Json(new { Message = respStatus.Message, respStatus }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<dynamic> SetCheckDone(string objectID, int stageMapID)
        {
          

            if (ModelState.IsValid)
            {
                bool IsClearForMaking = false;
                await Task.Run(() => _docDocMkCkStageService.IsClearForMaking(objectID, stageMapID, out IsClearForMaking));
                if (IsClearForMaking)
                {
                    await Task.Run(() => _docDocMkCkStageService.SetCheckDone(objectID, stageMapID, UserID, out res_code, out res_message));
                    return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Previous Stages aren't Verified Yet", Code = "0" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public async Task<dynamic> SetMakeDone(string objectID, int stageMapID)
        {
            if (ModelState.IsValid)
            {
                bool IsClearForMaking = false;
                await Task.Run(() => _docDocMkCkStageService.IsClearForMaking(objectID, stageMapID, out IsClearForMaking));
                if (IsClearForMaking)
                {
                    await Task.Run(() => _docDocMkCkStageService.SetMakeDone(objectID, stageMapID, UserID, out res_code, out res_message));
                    return Json(new { Message = "Successfull", Code = "1" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Previous Stages aren't Verified  Yet", Code = "0" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public async Task<dynamic> DeleteDocumentInfo(string objectID, string documentIDs, string action)
        {
            respStatus = await Task.Run(() => _docDocMkCkStageService.DeleteDocumentInfo(objectID, documentIDs, action, out res_code, out res_message));
            return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public async Task<dynamic> RevertFromMake(string objectID, List<ParentStage> stages, string revertReason)
        {
            if (ModelState.IsValid)
            {
                foreach (ParentStage stage in stages) {

                    await Task.Run(() => _docDocMkCkStageService.RevertFromMake(objectID, stage.StageMapID, revertReason, UserID, out res_code, out res_message));
                }
                
                return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public async Task<dynamic> RevertFromCheck(string objectID, List<ParentStage> stages, string revertReason)
        {
            if (ModelState.IsValid)
            {
                foreach (ParentStage stage in stages)
                {
                    await Task.Run(() => _docDocMkCkStageService.RevertFromCheck(objectID, stage.StageMapID, revertReason, UserID, out res_code, out res_message));
                    
                }

                return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public async Task<dynamic> RevertFromMakeUpdate(string objectID, List<ParentStage> stages, string revertReason)
        {
            if (ModelState.IsValid)
            {
                foreach (ParentStage stage in stages)
                {//1 for flase and 0 for true 
                   // await Task.Run(() => _docDocMkCkStageService.RevertFromMake(objectID, stage.StageMapID, revertReason, UserID, out res_code, out res_message));

                    string Qry1 = @"Update WFM_ObjectNextStageMap set IsMake='1',IsCheck='1',Isback='0' where ObjectID="+ objectID + " and StageMapID="+ stage.StageMapID + " And BackReason='"+ revertReason + "' ";

                    DataTable dt1 = CommandExecute(Qry1);
                    string Qry2 = @"UPDATE WFM_StageChangeHistory set Status = '1', ReadyForNextStage='1', IsMakeOrCheck= '0' WHERE ObjectID=" + objectID + " And Reason='" + revertReason + "' ";
                    DataTable dt2 = CommandExecute(Qry2);



                }

                return Json(new { Message = "Upload Successful", Code = "1" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
        }
        public async Task<dynamic> GetMasterDataBySearch(string masterID, string searchKey)
        {
            List<MasterData> objs = new List<MasterData>();
            await Task.Run(() => _docDocMkCkStageService.GetMasterDataBySearch(masterID, searchKey, out objs));

            return Json(objs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public async Task<dynamic> DeleteListItem(string tableRefID, int id)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(() => _docDocMkCkStageService.DeleteListItem(tableRefID, id, UserID, out res_code, out res_message));
                return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> ToggleAddNewListItem(string tableRefID)
        {
            VM_ListTypeProperties obj = new VM_ListTypeProperties();
            await Task.Run(() => _docDocMkCkStageService.ToggleAddNewListItem(tableRefID, out obj));

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public async Task<dynamic> AddSingleListItem(List<WFM_TableProperty> listItemColumn, string tableRefID, string objectID)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(() => _docDocMkCkStageService.AddSingleListItem(listItemColumn, tableRefID, objectID, out res_code, out res_message));
                return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = "Please fill up all required data", Code = "0" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<dynamic> AddDocumentInfo(DocumentsInfo _modelDocumentsInfo, string _selectedPropID, List<WFM_DocStageProperty> _docMetaValues, List<VM_ListTypeProperties> _listProperty,  string _extentions, bool _otherUpload = false)
        {
            List<DSM_DocPropIdentify> objDocPropIdentifies = null;

            if (ModelState.IsValid)
            {
                action = "add";
                _modelDocumentsInfo.SetBy = UserID;
                _modelDocumentsInfo.ModifiedBy = _modelDocumentsInfo.SetBy;
                _modelDocumentsInfo.UploaderIP = GetIPAddress.LocalIPAddress();

                string sqlTxt = "";
                string colTxt;
                string valTxt;

                foreach (var item in _listProperty)
                {

                    colTxt = "ObjectID";
                    valTxt = "@ObjectID";

                    foreach (var c in item.ColumnList)
                    {
                        if (c.FieldTitle != "ObjectID" && c.FieldTitle != "ID")
                        {
                            colTxt += "," + c.FieldTitle;
                            valTxt += ",'" + c.Value + "'";
                        }
                    }

                    sqlTxt += "INSERT INTO " + item.TableRefID + " (" + colTxt + ") VALUES(" + valTxt + "); ";
                }

                //var _docPropIdentifyIDs = string.Join(",", _docMetaValues.Select(x => x.DocPropIdentifyID));

                //_modelDocumentsInfo.ConfigureColumnIds = _autoValueSetupService.GetConfigureColumnList
                //    ("/Home/DocumentScanning", _modelDocumentsInfo.Owner.OwnerID,
                //        _modelDocumentsInfo.DocCategory.DocCategoryID,
                //        _modelDocumentsInfo.DocType.DocTypeID,
                //        _selectedPropID, _docPropIdentifyIDs);

                respStatus.Message = "Success";
                respStatus = await Task.Run(() => _docDocMkCkStageService.AddDocumentInfo(_modelDocumentsInfo, _selectedPropID, _docMetaValues, sqlTxt, _otherUpload, _extentions, action, out objDocPropIdentifies));

                var DistinctDocIDs1 = (from s in objDocPropIdentifies
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

                List<DSM_DocProperty> proplList = new List<DSM_DocProperty>();

                string[] docPropIDs = _selectedPropID.Split(',');

                foreach (var item in docPropIDs)
                {
                    DSM_DocProperty objDocProperty = new DSM_DocProperty();
                    objDocProperty.DocPropertyID = item;

                    proplList.Add(objDocProperty);
                }

                var DistinctDocIDs = (from p in proplList
                                      join d in DistinctDocIDs1 on p.DocPropertyID equals d.DocPropID
                                      select new
                                      {
                                          DocPropID = d.DocPropID,
                                          DocumentID = d.DocumentID,
                                          FileCodeName = d.FileCodeName,
                                          FileServerUrl = d.FileServerUrl

                                      }).ToList();

                foreach (var item in objDocPropIdentifies)
                {
                    try
                    {
                        FolderGenerator.MakeFTPDir(objDocPropIdentifies.FirstOrDefault().ServerIP, objDocPropIdentifies.FirstOrDefault().ServerPort, item.FileServerUrl, objDocPropIdentifies.FirstOrDefault().FtpUserName, objDocPropIdentifies.FirstOrDefault().FtpPassword);
                    }
                    catch (Exception e)
                    {
                        
                    }
                }

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
            respStatus = await Task.Run(() => _docDocMkCkStageService.DeleteDocumentInfo(_DocumentIDs));
            return Json(null, JsonRequestBehavior.AllowGet);
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