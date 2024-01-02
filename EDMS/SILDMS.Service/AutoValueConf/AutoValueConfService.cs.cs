using SILDMS.DataAccessInterface.AutoValueConf;
using SILDMS.Model.SecurityModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.AutoValueConf
{
    public class AutoValueConfService : IAutoValueConfService
    {
         #region Fields

        private readonly IAutoValueConfDataService _autoValueConfDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber = string.Empty;

        #endregion

        #region Constructor

        public AutoValueConfService(IAutoValueConfDataService autoValueConfDataService, ILocalizationService localizationService)
        {
            _autoValueConfDataService = autoValueConfDataService;
            _localizationService = localizationService;
        }

        #endregion

        public ValidationResult GetAutoValue(SEC_ConfigurePage obj, out List<SEC_ConfigureColumn> autoValues)
        {
            autoValues = _autoValueConfDataService.GetAutoValueConfiguration(obj, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult GetDbTables(string getDbTables, out List<SysDbTables> autoList)
        {
            autoList = _autoValueConfDataService.GetDbTables(getDbTables, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult ManipulateAutoValue(SEC_ConfigureColumn autoValue, string action, out string status)
        {
            _autoValueConfDataService.ManipulateAutoValue(autoValue, action, out status);
            return status.Length > 0
                ? new ValidationResult(status, _localizationService.GetResource(status))
                : ValidationResult.Success;
        }
    }
}
