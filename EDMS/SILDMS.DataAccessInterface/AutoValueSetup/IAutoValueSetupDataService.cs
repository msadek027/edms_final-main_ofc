using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.SecurityModule;

namespace SILDMS.DataAccessInterface.AutoValueSetup
{
    public interface IAutoValueSetupDataService
    {
        string AddAutoValueSetup(SEC_AutoValueSetup moAutoValueSetup, string action, out string errorNumber);

        List<SEC_AutoValueSetup> GetAutoValueSetupData(string _ConfigureID, string _ConfigureColumnID, string _UserID, out string errorNumber);

        List<SEC_Menu> GetConfiguredMenuList(string _UserID, out string errorNumber);

       // string GetConfigureColumnList(string menuUrl);

        string GetConfigureColumnList(string menuUrl, string ownerID, string catId, string typeID, string property, string propertyIdentity);
    }
}
