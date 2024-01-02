using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.CheckPrintModule;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccessInterface.CheckPrintDataServiceBadhon
{
    public interface ICheckPrintDataServiceBadhon
    {
        List<DSM_Owner> GetOwnersForAOwnerLevel(string UserID, string _OwnerLevelName, out string errorNumber);

        List<CMSBank> SearchBank(string UserID, string OwnerID, out string errorNumber);

        List<CMSBankBranch> SearchBranch(string UserID, string BankID, out string errorNumber);

        string InsertOrUpdateBank(CMSBank model, out string _errorNumber);

        string InsertOrUpdateAccount(CPSBankBranchAccount model, out string _errorNumber);

        string InsertOrUpdateCheckBook(CPSChequeRegistration model, out string _errorNumber);

        List<CMSBankBranch> InsertOrUpdateBranch(CMSBankBranch model, out string _errorNumber);
        List<CBPS_GetChequePrintInfByCompany> GetChequePrintInfo(string companyCode, string accountID,string user, out string _errorNumber);


        string SetChequePrintInfo(List<CBPS_GetChequePrintInfByCompany> printInfo, string UserID);

        List<CMSBank> SearchBankWithOutOwner(string UserID, out string _errorNumber);
    }


}
