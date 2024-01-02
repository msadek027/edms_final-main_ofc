using SILDMS.Model.CBPSModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccessInterface.InternalSrcBill
{
    public interface IInternalSrcBillDataService
    {
        List<BPS_BillReceive> FindBills(string BillTrackingNoFrom, string BillTrackingNoTo, out string errorNumber);

        List<DonutChartFor> ChartValue(string BillTrackingNo, out string _errorNumber);

        List<DonutChartFor> GraphForBarChart(string BillTrackingNo, out string _errorNumber);

        List<CMSVendor> GetVendors(out string _errorNumber);

        List<BPS_BillReceive> GetBillByVendors(string vendorCode, out string _errorNumber);

        List<BPS_BillReceive> GetBillByCompany(string OwnerID, out string _errorNumber);

        List<BPS_BillReceive> FindBillbyDate(string RecDateOne, string RecDateTwo, out string _errorNumber);
    }
}
