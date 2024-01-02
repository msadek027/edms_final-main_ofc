using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.DataAccess.ClearingReceiveForPrint;
using SILDMS.DataAccessInterface.ClearingReceiveForPrint;
using SILDMS.Model.CBPSModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;

namespace SILDMS.Service.ClearingReceiveForPrint
{
    public class ClearingReceiveForPrintService : IClearingReceiveForPrintService
    {
        #region Fields/Variables

        private readonly IClearingReceiveForPrintDataService _clearingReceiveForPrintDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber;

        #endregion

        #region Constructor

        public ClearingReceiveForPrintService()
        {
            _clearingReceiveForPrintDataService = new ClearingReceiveForPrintDataService();
            _localizationService = new LocalizationService();
            _errorNumber = string.Empty;
        }

        #endregion

        public ValidationResult GetClearings(string docNo,string dateFrom, string dateTo,
            out List<CBPSBillClearing> clearings)
        {
            clearings = _clearingReceiveForPrintDataService.GetClearings(docNo ,dateFrom, dateTo, out _errorNumber);
            return _errorNumber.Length > 0
                ? new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber))
                : ValidationResult.Success;
        }

        public ValidationResult SetClearingReceiveDocs(List<CBPSBillClearing> docs, out string status)
        {
            _clearingReceiveForPrintDataService.SetClearingReceiveDocs(docs, out status);
            return status.Length > 0
                ? new ValidationResult(status, _localizationService.GetResource(status))
                : new ValidationResult();
        }

   
    }
}
