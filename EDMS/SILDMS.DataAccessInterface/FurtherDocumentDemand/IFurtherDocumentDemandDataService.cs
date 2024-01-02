using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccessInterface.FurtherDocumentDemand
{
    public interface IFurtherDocumentDemandDataService
    {
        List<DSM_Owner> GetCompanies(string id, string action, out string errorNumber);
        List<BPS_BillProcessGroup> GetBillProcessGroups(string id, string action, out string errorNumber);
        List<BPS_BillProcessingStage> GetBillProcessingStages(string stageId, string groupId, string action,
            out string errorNumber);
        List<BPS_BillReceive> GetBillReceiveForFDD(string billTrackingNo, out string errorNumber);
        List<DSM_DocProperty> GetDocPropertyForFDD(string docCatId, string docPropId, out string errorNumber);
        string SetMailSmsAndFurtherDoc(BPS_MailSms mailSms, BPS_MailSmsSend mailSmsSend, BPS_FurtherDemand furtherDemand, List<BPS_FurtherDemandDoc> futherDemandDoc, string action, out string errorNumber);
    }
}
