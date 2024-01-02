using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;

namespace SILDMS.Service.FurtherDocumentDemand
{
    public interface IFurtherDocumentDemandService
    {
        ValidationResult GetCompanies(string id, string action, out List<DSM_Owner> companies);
        ValidationResult GetBillProcessGroups(string id, string action, out List<BPS_BillProcessGroup> processGroups);
        ValidationResult GetBillProcessingStages(string stageId, string groupId, string action, out List<BPS_BillProcessingStage> processingStages);
        ValidationResult GetBillReceiveForFDD(string billTrackingNo, out List<BPS_BillReceive> bills);
        ValidationResult GetDocPropertyForFDD(string docCatId, string docPropId, out List<DSM_DocProperty> docProps);
        ValidationResult AddUpdateFdd(BPS_MailSms mailSms, BPS_MailSmsSend mailSmsSend, BPS_FurtherDemand furtherDemand,
            List<BPS_FurtherDemandDoc> furtherDemandDocs,string action, out string status);
    }
}
