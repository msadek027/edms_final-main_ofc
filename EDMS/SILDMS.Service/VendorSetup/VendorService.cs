using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.DataAccess.VendorSetup;
using SILDMS.DataAccessInterface.VendorSetup;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;

namespace SILDMS.Service.VendorSetup
{
    public class VendorService : IVendorService
    {
        #region Fields

        private readonly IVendorDataService _vendorDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber;

        #endregion

        #region Constructor

        public VendorService()
        {
            _vendorDataService= new VendorDataService();
            _localizationService = new LocalizationService();
            _errorNumber = string.Empty;
        }

        #endregion

        public ValidationResult GetOwners(out List<DSM_Owner> ownersList)
        {
            ownersList = _vendorDataService.GetOwners(out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult GetVendors(string vendorId, string ownerId, out List<CMSVendor> vendors)
        {
            vendors = _vendorDataService.GetVendors(vendorId, ownerId, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult GetVendorAddresses(string vendorId, string addressId, out List<CMS_VendorAddress> addresses)
        {
            addresses = _vendorDataService.GetVendorAddresses(vendorId, addressId, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : new ValidationResult();
        }

        public ValidationResult ManipulateVendor(CMSVendor vendor, string action, out string status)
        {
            _vendorDataService.ManipulateVendor(vendor, action, out status);
            return status.Length > 0
                ? new ValidationResult(status, _localizationService.GetResource(status))
                : ValidationResult.Success;
        }

        public ValidationResult ManipulateVendorAddress(CMS_VendorAddress address, string action, out string status)
        {
            _vendorDataService.ManipulateVendorAddress(address, action, out status);
            return status.Length > 0
                ? new ValidationResult(status, _localizationService.GetResource(status))
                : ValidationResult.Success;
        }
    }
}
