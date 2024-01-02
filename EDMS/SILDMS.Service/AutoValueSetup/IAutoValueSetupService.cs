using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.SecurityModule;
using SILDMS.Utillity;

namespace SILDMS.Service.AutoValueSetup
{
    public interface IAutoValueSetupService
    {
        ValidationResult AddAutoValueSetup(SEC_AutoValueSetup modelAutoValueSetup,
            string action, out string status);

        ValidationResult EditAutoValueSetup(SEC_AutoValueSetup modelAutoValueSetup,
            string action, out string status);

        ValidationResult GetAutoValueSetupData(string _ConfigureID, string _ConfigureColumnID, string _UserID,
            out List<SEC_AutoValueSetup> docPropIdentifyList);

        ValidationResult GetConfiguredMenuList(string _UserID,
            out List<SEC_Menu> secMenus);

        string GetConfigureColumnList(string menuUrl,string ownerId, string catId, string typeID, string property, string propertyIdentity);
    }
}
