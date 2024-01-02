using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.DataAccessInterface.DefaultValueSetup;
using SILDMS.Model.SecurityModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;

namespace SILDMS.Service.DefaultValueSetup
{
    public class DefaultValueSetupService : IDefalutValueSetupService
    {
        #region Fields

        private readonly IDefaultValueSetupDataService _defaultValueSetupDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber = string.Empty;

        #endregion

        #region Constructor

        public DefaultValueSetupService(IDefaultValueSetupDataService defaultValueSetupDataService, ILocalizationService localizationService)
        {
            _defaultValueSetupDataService = defaultValueSetupDataService;
            _localizationService = localizationService;
        }

        #endregion

        public ValidationResult GetDefaultPrimary(string id, out object result)
        {
            result = _defaultValueSetupDataService.GetDefaultPrimary(id, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult GetDefaultValueSetup(SEC_DefaultValue obj, out List<SEC_DefaultValue> result)
        {
            result = _defaultValueSetupDataService.GetDefaultValues(obj, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult ManipulateDefaultValues(SEC_DefaultValue obj, string action, out string status)
        {
            _defaultValueSetupDataService.ManipulateDefaultValues(obj, action, out status);
            return status.Length > 0
                ? new ValidationResult(status, _localizationService.GetResource(status))
                : ValidationResult.Success;
        }

        public ValidationResult GetMenusForDefaultValue(out List<SEC_Menu> result)
        {
            result = _defaultValueSetupDataService.GetMenusForDefaultValue();
            return ValidationResult.Success;
        }

        public ValidationResult GetColumnForDefaultValue(SEC_ConfigurePage obj, out List<SEC_ConfigureColumn> result)
        {
            result = _defaultValueSetupDataService.GetColumnForDefaultValue(obj, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }
    }
}
