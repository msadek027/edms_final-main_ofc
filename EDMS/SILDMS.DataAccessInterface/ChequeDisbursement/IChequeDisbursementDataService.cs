using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.CheckPrintModule;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccessInterface.ChequeDisbursement
{
    public interface IChequeDisbursementDataService
    {
        List<DSM_Owner> GetCompanies(string id, string action, out string errorNumber);

        List<CMSVendor> GetVendors(string ownerId, out string errorNumber);

        List<DSM_DocProperty> GetDocProperty(string ownerId, out string errorNumber);

        List<CPS_PrintAccountDocAction> GetEftCheque(string docType, string ownerId, string vendorId, string chequeNo,
            out string errorNumber);

        string SetPrintAccountDocAction(List<CPS_PrintAccountDocAction> docs, out string errorNumber);

        DisburseModel Diburse(List<CPS_PrintAccountDocAction> docActions, int savedOnce, out string errorNumber);
    }
}
