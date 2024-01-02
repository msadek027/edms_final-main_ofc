using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.SecurityModule;

namespace SILDMS.DataAccessInterface.DefaultValueSetup
{
    public interface IDefaultValueSetupDataService
    {
        List<SEC_DefaultValue> GetDefaultValues(SEC_DefaultValue obj, out string errorNumber);
        object GetDefaultPrimary(string id, out string errornumber);
        string ManipulateDefaultValues(SEC_DefaultValue obj, string action, out string errorNumber);
        List<SEC_Menu> GetMenusForDefaultValue();
        List<SEC_ConfigureColumn> GetColumnForDefaultValue(SEC_ConfigurePage obj, out string errorNumber);
    }
}
