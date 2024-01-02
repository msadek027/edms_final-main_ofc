using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Service.InternalSrcBill;
using SILDMS.Model.CBPSModule;
using SILDMS.Utillity;
using SILDMS.DataAccessInterface.InternalSrcBill;
using SILDMS.Utillity.Localization;

namespace SILDMS.Service.InternalSrcBill
{
    public class InternalSrcBillService:IInternalSrcBillService
    {
        #region Fields

        private readonly IInternalSrcBillDataService _internalSrcBillDataService;
        private readonly ILocalizationService _localizationService;
        private string _errorNumber = string.Empty;
       

        #endregion

        #region Constructor

        public InternalSrcBillService(IInternalSrcBillDataService internalSrcBillDataService, ILocalizationService localizationService)
        {
            _internalSrcBillDataService = internalSrcBillDataService;
            _localizationService = localizationService;
        }

        #endregion

        public ValidationResult FindBills(string BillTrackingNoFrom, string BillTrackingNoTo, out List<BPS_BillReceive> bills)
        {
            bills = _internalSrcBillDataService.FindBills(BillTrackingNoFrom, BillTrackingNoTo, out _errorNumber);
            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }
            return ValidationResult.Success;
        }


        public ValidationResult ChartValue(string BillTrackingNo, out List<DonutChartFor> donutChart)
        {
            donutChart = _internalSrcBillDataService.ChartValue(BillTrackingNo, out _errorNumber);
            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }
            return ValidationResult.Success;
        }


        public ValidationResult GraphForBarChart(string BillTrackingNo, out List<DonutChartFor> donutChart)
        {
            donutChart = _internalSrcBillDataService.GraphForBarChart(BillTrackingNo, out _errorNumber);
            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }
            return ValidationResult.Success;
        }

        public ValidationResult GetVendors(out List<CMSVendor> vendors)
        {
            vendors = _internalSrcBillDataService.GetVendors(out _errorNumber);
            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }
            return ValidationResult.Success;
        }


        public ValidationResult GetBillByVendors(string vendorCode, out List<BPS_BillReceive> bills)
        {
            bills = _internalSrcBillDataService.GetBillByVendors(vendorCode, out _errorNumber);
            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }
            return ValidationResult.Success;
        }


        public ValidationResult GetBillByCompany(string OwnerID, out List<BPS_BillReceive> bills)
        {
            bills = _internalSrcBillDataService.GetBillByCompany(OwnerID, out _errorNumber);
            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }
            return ValidationResult.Success;
        }


        public ValidationResult FindBillbyDate(string RecDateOne, string RecDateTwo, out List<BPS_BillReceive> bills)
        {
            bills = _internalSrcBillDataService.FindBillbyDate(RecDateOne, RecDateTwo, out _errorNumber);
            if (_errorNumber.Length > 0)
            {
                return new ValidationResult(_errorNumber, _localizationService.GetResource(_errorNumber));
            }
            return ValidationResult.Success;
        }
    }
}
