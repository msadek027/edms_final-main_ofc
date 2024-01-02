using SILDMS.Model.DocScanningModule;
using SILDMS.Service.WorkflowDocSearching;
using SILDMS.Web.UI.Areas.SecurityModule.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Areas.WorkflowModule.Controllers
{
    public class WorkflowDocSearchingController : Controller
    {
        private readonly string UserID = string.Empty;
        private readonly IWorkflowDocSearchingService _workflowDocSearchingService; 

        public WorkflowDocSearchingController(IWorkflowDocSearchingService workflowDocSearchingService)
        {
            _workflowDocSearchingService = workflowDocSearchingService;
            UserID = SILAuthorization.GetUserID();
        }

        [Authorize]
        public async Task<dynamic> GetDocumentsBySearchParam(string _OwnerID, string _DocCategoryID, string _DocTypeID, int page = 1, int itemsPerPage = 5, string sortBy = "[ObjectID]", bool reverse = false, string search = "")
        {
            DataTable dt = null;
            int totalPages = 0;
            await Task.Run(() => _workflowDocSearchingService.GetWorkflowDocument(_OwnerID, _DocCategoryID, _DocTypeID, UserID, page, itemsPerPage, sortBy, reverse, search, out dt));

            string html = "";
            if (dt.Columns.Count > 0)
            {
                html += "<table class=\"table table-condensed table-bordered table-striped table-hover pnlView\">";
                //add header row
                html += "<thead><tr>";

                for (int i = 0; i < dt.Columns.Count - 1; i++)
                    html += "<th>" + dt.Columns[i].ColumnName + "</th>";

                html += "<th>Action</th>";
                html += "</tr>";
                html += "<tr><th colspan=\"" + (dt.Columns.Count - 1) / 2 + "\"><input placeholder = \"Search for...\" ng-keypress=\"search($event)\" ng-model=\"pagingInfo.search\" class=\"form-control\"/></th></tr>";
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

                        html += "<td><button type=\"button\" class=\"btn btn-info btn-xs btn-flat\" ng-click=\"viewDetail('" + dt.Rows[i][0].ToString() + "')\">View</button>&nbsp";
                        html += "<a class=\"btn btn-xs btn-success btn-flat\"  style=\"display: inline-block;\" href=\"/Download/DownloadWorkflowDocument?ObjectID=" + dt.Rows[i][0].ToString() + "\">Download</a>";
                        html += "</td></tr>";
                    }

                    totalPages = Convert.ToInt32(dt.Rows[0]["COUNT"]);
                }

                html += "</tbody></table>";
            }
            else
            {
                html += "<div class=\"text-center\"><img src =\"/Images/no_results.png\" alt =\"Smiley face\" height = \"300\" width = \"500\"></div>";
            }

            //add rows

            return Json(new { html, totalPages }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<dynamic> GetDocumentForSpecificObject(string _ObjectID)
        {
            List<DSM_Documents> documents = null;
            await Task.Run(() => _workflowDocSearchingService.GetDocumentForSpecificObject(_ObjectID, UserID, out documents));

            return Json(documents, JsonRequestBehavior.AllowGet);
        }

    }
}