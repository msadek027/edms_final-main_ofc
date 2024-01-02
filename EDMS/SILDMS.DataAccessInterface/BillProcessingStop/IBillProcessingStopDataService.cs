using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccessInterface.BillProcessingStop
{
    public interface IBillProcessingStopDataService
    {
        List<DSM_Owner> GetCompanies(string id, string action, out string errorNumber);
        List<BPS_BillProcessGroup> GetBillProcessGroups(string id, string action, out string errorNumber);
        List<BPS_BillProcessingStage> GetBillProcessingStages(string stageId, string groupId, string action,
            out string errorNumber);
        List<CMSVendor> GetVendors(string vendorId, string ownerId, string action, out string errorNumber);
        List<BPS_BillProcessingStopVM> GetBillReceives(string ownerId, string processGroupId, string currentStage,
            string vendorCode, string billTrackingNo, out string errorNumber);
        List<BPS_BillProcessingStopVM> GetStopBill(string billTrackingNo, string action, out string errorNumber);
        List<ReturnResult> GetVendorPhoneNoEmail(string vendorCode, string action, out string errorNumber);
        string ManipulateBillProcessStop(BPS_StopBillDtl billDtl, string action,
            out string errorNumber);
    }
}
