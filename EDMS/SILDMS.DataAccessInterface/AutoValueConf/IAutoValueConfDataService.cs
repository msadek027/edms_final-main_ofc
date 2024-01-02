using SILDMS.Model.SecurityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccessInterface.AutoValueConf
{
    public interface IAutoValueConfDataService
    {
        List<SysDbTables> GetDbTables(string tbl, out string errorNumber);
        List<SEC_ConfigureColumn> GetAutoValueConfiguration(SEC_ConfigurePage ojb, out string errorNumber);
        string ManipulateAutoValue(SEC_ConfigureColumn obj, string action, out string errorNumber);
    }
}
