using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccessInterface.VendorSetup
{
    public interface IVendorDataService
    {
        List<DSM_Owner> GetOwners(out string errornumber);
        List<CMSVendor> GetVendors(string vendorId, string ownerId, out string errorNumber);
        List<CMS_VendorAddress> GetVendorAddresses(string vendorId, string addressId, out string errorNumber);
        string ManipulateVendor(CMSVendor vendor, string action, out string errorNumber);
        string ManipulateVendorAddress(CMS_VendorAddress address, string action, out string errorNumber);
    }
}
