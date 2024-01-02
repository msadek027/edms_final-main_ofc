using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.CBPSModule;
using SILDMS.Utillity;

namespace SILDMS.Service.ClearingReceiveForPrint
{
    public interface IClearingReceiveForPrintService
    {
        ValidationResult GetClearings(string docNo, string dateFrom, string dateTo,
            out List<CBPSBillClearing> clearings);

        ValidationResult SetClearingReceiveDocs(List<CBPSBillClearing> docs, out string status);
    }
}
