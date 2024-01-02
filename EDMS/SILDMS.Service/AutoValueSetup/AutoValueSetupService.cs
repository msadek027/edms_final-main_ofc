using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.DataAccessInterface.AutoValueSetup;
using SILDMS.DataAccessInterface.OwnerProperIdentity;
using SILDMS.Model.SecurityModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;

namespace SILDMS.Service.AutoValueSetup
{
    public class AutoValueSetupService: IAutoValueSetupService
    {
        private readonly IAutoValueSetupDataService _autoValueSetupDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber = string.Empty;

        public AutoValueSetupService(IAutoValueSetupDataService autoValueSetupDataService,
            ILocalizationService localizationService)
        {
            _autoValueSetupDataService = autoValueSetupDataService;
            _localizationService = localizationService;
        }

        public ValidationResult AddAutoValueSetup(SEC_AutoValueSetup modelAutoValueSetup, string action, out string status)
        {
            _autoValueSetupDataService.AddAutoValueSetup(modelAutoValueSetup, action,
                out status);
            if (status.Length > 0)
            {
                return new ValidationResult(status, _localizationService.GetResource(status));
            }
            return ValidationResult.Success;
        }

        public ValidationResult EditAutoValueSetup(SEC_AutoValueSetup modelAutoValueSetup, string action, out string status)
        {
            _autoValueSetupDataService.AddAutoValueSetup(modelAutoValueSetup, action,
               out status);
            if (status.Length > 0)
            {
                return new ValidationResult(status, _localizationService.GetResource(status));
            }
            return ValidationResult.Success;
        }

        public ValidationResult GetAutoValueSetupData(string _ConfigureID, string _ConfigureColumnID, string _UserID,
            out List<SEC_AutoValueSetup> autoValueSetups)
        {
            autoValueSetups = _autoValueSetupDataService.GetAutoValueSetupData
                (_ConfigureID, _ConfigureColumnID, _UserID, out _errorNumber);

            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult GetConfiguredMenuList(string _UserID,
            out List<SEC_Menu> autoValueSetups)
        {
            autoValueSetups = _autoValueSetupDataService.GetConfiguredMenuList
                (_UserID, out _errorNumber);

            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }

            return ValidationResult.Success;
        }


        public string GetConfigureColumnList(string menuUrl,string ownerID,string catId, string typeID, string property,string propertyIdentity)
        {
            return _autoValueSetupDataService.GetConfigureColumnList(menuUrl, ownerID, catId, typeID, property, propertyIdentity);
        }
    }
}
