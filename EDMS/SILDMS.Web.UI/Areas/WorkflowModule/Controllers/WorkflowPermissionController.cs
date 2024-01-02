using SILDMS.Model.WorkflowModule;
using SILDMS.Service.UserStagePermission;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
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
    public class WorkflowPermissionController : Controller
    {
        // GET: WorkflowModule/WorkflowPermission
        private readonly IUserStagePermissionService _IUserStagePermissionService;
        private readonly ILocalizationService _localizationService;
        private ValidationResult respStatus = new ValidationResult();
        private string outStatus = string.Empty;
        private string res_code = string.Empty;
        private string res_message = string.Empty;
        private readonly string UserID = string.Empty;

        public WorkflowPermissionController(IUserStagePermissionService userStagePermissionService, ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            _IUserStagePermissionService = userStagePermissionService;
            UserID = SILAuthorization.GetUserID();
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<dynamic> GetReportView_StageAssignedRoleUser(string WorkdflowID)
        {
            List<StageAssignedRoleUserModel> result = null;
        
            string Qry = @"Select * from (Select a.StageID,a.StageName,a.WorkdflowID,a.DocTypeID,b.UserStagePermissionID,b.UserID,b.RoleID,b.UserLevelID,
                        m.WorkflowName,m.NumberOfStage,c.EmployeeID,c.UserFullName,c.UserName,d.RoleTitle
                        from WFM_ProcessingStage a Left JOIN WFM_UserStagePermission b
                        ON a.StageID=b.StageID
                        INNER JOIN WFM_Workflow m ON a.WorkdflowID=m.WorkflowID
                        Left JOIN SEC_User c ON b.RoleID=c.RoleID AND b.UserID=c.UserID
                        Left JOIN SEC_Role d ON b.RoleID=d.RoleID
                        ) f Where f.DocTypeID='" + WorkdflowID + "'";
     
            DataTable dt = CommandExecute(Qry);
            result = (from DataRow row in dt.Rows
                            select new StageAssignedRoleUserModel
                            {
                                StageID = row["StageID"].ToString(),
                                StageName = row["StageName"].ToString(),
                                NumberOfStage = row["NumberOfStage"].ToString(),
                                EmployeeID = row["EmployeeID"].ToString(),
                                EmployeeName = row["UserFullName"].ToString(),
                                WorkdflowID = row["WorkdflowID"].ToString(),
                                WorkdflowName = row["WorkflowName"].ToString(),
                                RoleID = row["RoleID"].ToString(),
                                RoleName = row["RoleTitle"].ToString(),
                                UserID = row["UserName"].ToString(),
                                RoleUserBase = (row["UserID"].ToString()=="" || row["UserID"].ToString()==null)? "RoleBase" : "UserBase",
                                UserLevelID = row["UserLevelID"].ToString(),

                            }).ToList();


         
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> GetStagesWithPermission(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _UserID)
        {
            List<WFM_ProcessStageMap> objs = new List<WFM_ProcessStageMap>();
            await Task.Run(() => _IUserStagePermissionService.GetStagesWithPermission( _DocTypeID, _UserID, out objs));

            return Json(objs, JsonRequestBehavior.AllowGet);
        }

        public async Task<dynamic> SetStagesWithPermission(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _UserID, List<WFM_ProcessStageMap> objs,string RoleID,string OwnerLevelID)
        {
           

            await Task.Run(() => _IUserStagePermissionService.SetStagesWithPermission(_OwnerID, _DocCategoryID, _DocTypeID, _UserID, UserID, objs, out res_code, out res_message));
            foreach (WFM_ProcessStageMap obj in objs)
            {
                if (obj.NotifyMk) //"RoleBase"
                {
                    string QryMax = "Select MAX(UserStagePermissionID) from WFM_UserStagePermission Where  StageID=" + obj.StageID + "";
                    DataTable dtMax = CommandExecute(QryMax);
                    string max_UserStagePermissionID = dtMax.Rows[0][0].ToString();
                    string QryDel = "Delete from WFM_UserStagePermission Where  StageID=" + obj.StageID + " AND UserStagePermissionID !=" + max_UserStagePermissionID + "";
                    DataTable dtDel = CommandExecute(QryDel);

                    string Qry = @"Update WFM_UserStagePermission Set UserID ='',RoleID='" + RoleID + "',UserLevelID=" + OwnerLevelID + " Where  StageID=" + obj.StageID + "";
                    DataTable dt = CommandExecute(Qry);
                }
                else
                {
                    string QryDel = "Delete from WFM_UserStagePermission Where StageID=" + obj.StageID + " AND UserID IS NULL OR UserID =''";
                    DataTable dtDel = CommandExecute(QryDel);
                    string QryUpdate = @"Update WFM_UserStagePermission Set RoleID='" + RoleID + "',UserLevelID=" + OwnerLevelID + " Where  StageID=" + obj.StageID + "";
                    DataTable dt = CommandExecute(QryUpdate);
                }
            }
            
            return Json(new { Message = res_message, Code = res_code }, JsonRequestBehavior.AllowGet);
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

    public class StageAssignedRoleUserModel
    {
        public string StageID { get; set; }
        public string StageName { get; set; }
        public string NumberOfStage { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string WorkdflowID { get; set; }
        public string WorkdflowName { get; set; }    

        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string UserLevelID { get; set; }
        public string UserID { get; set; }
        public string RoleUserBase { get; set; }




    }
}