using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.SecurityModule;
using SILDMS.Utillity;

namespace SILDMS.Service.DefaultValueSetup
{
    public interface IDefalutValueSetupService
    {
        ValidationResult GetDefaultPrimary(string id, out object result);
        ValidationResult GetDefaultValueSetup(SEC_DefaultValue obj, out List<SEC_DefaultValue> result);
        ValidationResult ManipulateDefaultValues(SEC_DefaultValue docType, string action, out string status);
        ValidationResult GetMenusForDefaultValue(out List<SEC_Menu> result);
        ValidationResult GetColumnForDefaultValue(SEC_ConfigurePage obj, out List<SEC_ConfigureColumn> result);
    }
}
