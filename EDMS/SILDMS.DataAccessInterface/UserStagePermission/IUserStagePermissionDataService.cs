using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.DataAccessInterface.UserStagePermission
{
    public interface IUserStagePermissionDataService
    {
        void SetStagesWithPermission(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _UserID, string _SessionUserID, List<WFM_ProcessStageMap> objs, out string res_code, out string res_message);
        List<WFM_ProcessStageMap> GetStagesWithPermission(string _DocTypeID, string _UserID);
    }
}
