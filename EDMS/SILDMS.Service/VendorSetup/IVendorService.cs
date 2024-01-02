using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;

namespace SILDMS.Service.VendorSetup
{
    public interface IVendorService
    {
        ValidationResult GetOwners(out List<DSM_Owner> ownersList);
        ValidationResult GetVendors(string vendorId, string ownerId, out List<CMSVendor> vendors);
        ValidationResult GetVendorAddresses(string vendorId, string addressId, out List<CMS_VendorAddress> addresses);
        ValidationResult ManipulateVendor(CMSVendor vendor, string action, out string status);
        ValidationResult ManipulateVendorAddress(CMS_VendorAddress address, string action, out string status);
    }
}
