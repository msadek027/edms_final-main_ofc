using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.DataAccess.FurtherDocumentDemand;
using SILDMS.DataAccessInterface.FurtherDocumentDemand;
using SILDMS.Model;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;

namespace SILDMS.Service.FurtherDocumentDemand
{
    public class FurtherDocumentDemandService : IFurtherDocumentDemandService
    {
        #region Fields

        private readonly IFurtherDocumentDemandDataService _furtherDocumentDemandDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber;

        #endregion

        #region Constructor

        public FurtherDocumentDemandService()
        {
            _furtherDocumentDemandDataService = new FurtherDocumentDemandDataService();
            _localizationService = new LocalizationService();
        }

        #endregion

        public ValidationResult GetCompanies(string id, string action, out List<DSM_Owner> companies)
        {
            companies = _furtherDocumentDemandDataService.GetCompanies(id, action, out _errorNumber);
            return _errorNumber.Length > 0 ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber)) :
                ValidationResult.Success;
        }

        public ValidationResult GetBillProcessGroups(string id, string action, out List<BPS_BillProcessGroup> processGroups)
        {
            processGroups = _furtherDocumentDemandDataService.GetBillProcessGroups(id, action, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult GetBillProcessingStages(string stageId, string groupId, string action, out List<BPS_BillProcessingStage> processingStages)
        {
            processingStages = _furtherDocumentDemandDataService.GetBillProcessingStages(stageId, groupId, action,
                out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult GetBillReceiveForFDD(string billTrackingNo, out List<BPS_BillReceive> bills)
        {
            bills = _furtherDocumentDemandDataService.GetBillReceiveForFDD(billTrackingNo, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult GetDocPropertyForFDD(string docCatId, string docPropId, out List<DSM_DocProperty> docProps)
        {
            docProps = _furtherDocumentDemandDataService.GetDocPropertyForFDD(docCatId, docPropId, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult AddUpdateFdd(BPS_MailSms mailSms, BPS_MailSmsSend mailSmsSend, BPS_FurtherDemand furtherDemand,
            List<BPS_FurtherDemandDoc> furtherDemandDocs, string action, out string status)
        {
            _furtherDocumentDemandDataService.SetMailSmsAndFurtherDoc(mailSms, mailSmsSend, furtherDemand,
                furtherDemandDocs, action, out status);
            return status.Length > 0
                ? new ValidationResult(status, _localizationService.GetResource(status))
                : ValidationResult.Success;
        }
    }
}
