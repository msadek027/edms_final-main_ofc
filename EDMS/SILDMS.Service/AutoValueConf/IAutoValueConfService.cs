using SILDMS.Model.SecurityModule;
using SILDMS.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.AutoValueConf
{
    public interface IAutoValueConfService
    {
        ValidationResult GetAutoValue(SEC_ConfigurePage obj, out List<SEC_ConfigureColumn> autoValues);
        ValidationResult GetDbTables(string getDbTables, out List<SysDbTables> ownerLevelList);
        ValidationResult ManipulateAutoValue(SEC_ConfigureColumn autoValue, string action, out string status);

    }
}
