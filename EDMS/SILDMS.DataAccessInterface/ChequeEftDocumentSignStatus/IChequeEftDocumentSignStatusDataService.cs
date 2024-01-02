using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.CheckPrintModule;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccessInterface.ChequeEftDocumentSignStatus
{
    public interface IChequeEftDocumentSignStatusDataService
    {
        List<DSM_Owner> GetCompanies(string id, string action, out string errorNumber);
        List<CPS_PrintAccountDocAction> GetChequeEft(string transcType, string ownerId, string docCat,
           out string errorNumber);
        string SetPrintAccountDocAction(List<CPS_PrintAccountDocAction> docs, out string errorNumber);
    }
}
