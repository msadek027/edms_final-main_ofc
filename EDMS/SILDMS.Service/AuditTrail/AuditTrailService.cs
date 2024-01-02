using SILDMS.DataAccessInterface.AuditTrail;
using SILDMS.Model.SecurityModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.AuditTrail
{
    public class AuditTrailService : IAuditTrailService
    {
        private readonly IAuditTrailDataService _iAuditTrailDataService;
        private readonly ILocalizationService _localizationService;
        private string errorNumber = string.Empty;
         public AuditTrailService(IAuditTrailDataService
            iAuditTrailDataService, ILocalizationService localizationService)
        {
            _iAuditTrailDataService = iAuditTrailDataService;
            _localizationService = localizationService;
        }
        public ValidationResult GetAllAuditTrail(string _SearchBy, string UserID, int page, 
            int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<SEC_UserLog> lstLogSearch)
        {
            lstLogSearch = _iAuditTrailDataService.GetAllAuditTrail(_SearchBy, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out errorNumber);

            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;
        }



        public ValidationResult GetAllAuditTrailForReport(string _SearchBy, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<SEC_UserLog> lstLogSearch)
        {
            lstLogSearch = _iAuditTrailDataService.GetAllAuditTrailForReport(_SearchBy, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out errorNumber);

            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;
        }
    }
}
