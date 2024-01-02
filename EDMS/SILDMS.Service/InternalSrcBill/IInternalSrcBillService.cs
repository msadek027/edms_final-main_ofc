using SILDMS.Model.CBPSModule;
using SILDMS.Utillity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.InternalSrcBill
{
    public interface IInternalSrcBillService
    {
        ValidationResult FindBills(string BillTrackingNoFrom, string BillTrackingNoTo, out List<BPS_BillReceive> bills);
        ValidationResult ChartValue(string BillTrackingNo, out List<DonutChartFor> donutChart);
        ValidationResult GraphForBarChart(string BillTrackingNo, out List<DonutChartFor> donutChart);

        ValidationResult GetVendors(out List<CMSVendor> vendors);

        ValidationResult GetBillByVendors(string vendorCode, out List<BPS_BillReceive> bills);

        ValidationResult GetBillByCompany(string OwnerID, out List<BPS_BillReceive> bills);

        ValidationResult FindBillbyDate(string RecDateOne, string RecDateTwo, out List<BPS_BillReceive> bills);
    }
}
