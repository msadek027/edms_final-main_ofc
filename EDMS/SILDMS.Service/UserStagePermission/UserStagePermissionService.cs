using SILDMS.DataAccessInterface.UserStagePermission;
using SILDMS.Utillity.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.Service.UserStagePermission
{
    public class UserStagePermissionService : IUserStagePermissionService
    {
        private readonly IUserStagePermissionDataService _IUserStagePermissionDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber = string.Empty;
        public UserStagePermissionService(IUserStagePermissionDataService userStagePermissionDataService, ILocalizationService localizationService)
        {
            _IUserStagePermissionDataService = userStagePermissionDataService;
            localizationService = _localizationService;
        }

        public void SetStagesWithPermission(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _UserID, string _SessionUserID, List<WFM_ProcessStageMap> objs, out string res_code, out string res_message)
        {
            _IUserStagePermissionDataService.SetStagesWithPermission(_OwnerID, _DocCategoryID, _DocTypeID, _UserID, _SessionUserID, objs, out res_code, out res_message);
        }

        public void GetStagesWithPermission( string _DocTypeID, string _UserID, out List<WFM_ProcessStageMap> objs)
        {
            objs=_IUserStagePermissionDataService.GetStagesWithPermission(_DocTypeID, _UserID);
        }
    }
}
