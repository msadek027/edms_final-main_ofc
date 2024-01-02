using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.Service.UserStagePermission
{
    public interface IUserStagePermissionService
    {
        void SetStagesWithPermission(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _UserID, string userID, List<WFM_ProcessStageMap> objs, out string res_code, out string res_message);
        void GetStagesWithPermission( string _DocTypeID, string _UserID, out List<WFM_ProcessStageMap> objs);
    }
}
