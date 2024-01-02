using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.CBPSModule;

namespace SILDMS.DataAccessInterface.ClearingReceiveForPrint
{
    public interface IClearingReceiveForPrintDataService
    {
        List<CBPSBillClearing> GetClearings(string docNo, string dateFrom, string dateTo,
            out string errorNumber);

        string SetClearingReceiveDocs(List<CBPSBillClearing> docs, out string errorNumber);
    }
}
