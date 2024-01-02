using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.Model.CheckPrintModule
{
    public class CPSBankBranchAccount
    {
        public string AccountID { get; set; }
        public string BranchID { get; set; }
        public string BankID { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string TransAmt { get; set; }
        public string TransCurrencyType { get; set; }
        public string DefaultCurrency { get; set; }

        public string ConcernGL { get; set; }
        public string LastCreditAmt { get; set; }
        public string LastDebitAmt { get; set; }
        public string ClosingAmt { get; set; }
        public string AccountOpenDate { get; set; }
        public string AccountCloseDate { get; set; }
        public string SignatoryGroup { get; set; }
        public string UserLevel { get; set; }




        public string SetOn { get; set; }
        public string SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }

        public DSM_Owner Owner { get; set; }
        public CMSBank Bank { get; set; }
        public CMSBankBranch Branch { get; set; }
    }
}
