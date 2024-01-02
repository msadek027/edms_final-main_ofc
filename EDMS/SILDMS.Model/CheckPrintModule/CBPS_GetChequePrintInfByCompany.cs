using SILDMS.Model.DocScanningModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CheckPrintModule
{
    public class CBPS_GetChequePrintInfByCompany
    {
        public string BillClearingID { get; set; }
        public string ClearingNo { get; set; }
        public string Beneficiary { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string ChequeDate { get; set; }
        public string TransType { get; set; }
        public string ChequeNo { get; set; }
        public string VoidChequeNo { get; set; }

        public bool IsChecked { get; set; }

        public DSM_Owner Owner { get; set; }
        public Bank Bank { get; set; }
        public Branch Branch { get; set; }
        public Account Account { get; set; }

        public virtual ICollection<DSM_Owner> lstOwner { get; set; }
        public virtual ICollection<Bank> lstBank { get; set; }
        public virtual ICollection<Account> lstAccount { get; set; }
        public virtual ICollection<Branch> lstBranch { get; set; }

        public string OwnerID { get; set; }
        public string BankID { get; set; }
        public string AccountNo { get; set; }
        public string LeafNo { get; set; }
        public string AmountInWord { get; set; }
        public string PaymentMode { get; set; }
        public string PrintMode { get; set; }
        public string AmtInWord { get; set; }
    }

    public class CPS_PrintAccount
    {
        public string BillClearingID { get; set; }
        public string ClearingNo { get; set; }
        public string Beneficiary { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string ChequeDate { get; set; }
        public string TransType { get; set; }
        public string PaymentPurpose { get; set; }
        public string DocumentNo { get; set; }
        public string Party { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentType { get; set; }
        public string PrintMode { get; set; }
        public string VoidDocumentNo { get; set; }
        public string Remarks { get; set; }
        public string MessageToBenf { get; set; }
    }

    public class Bank
    {
        public string BankID { get; set; }
        public string BankName { get; set; }
    }

   


    public class Branch
    {
        public string BranchID { get; set; }
        public string BranchName { get; set; }
    }

    public class Account
    {
        public string AccountID { get; set; }
        public string AccountName { get; set; }
           public string AccountNo { get; set; }
    }

    public class ChequePrintInfDB
    {
        public string BillClearingID { get; set; }
        public string ClearingNo { get; set; }
        public string Beneficiary { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string ChequeDate { get; set; }
        public string TransType { get; set; }
        public string ChequeNo { get; set; }
        public string VoidChequeNo { get; set; }
        public string OwnerID { get; set; }
        public string BankID { get; set; }
        public string BranchID { get; set; }
        public string AccountID { get; set; }
    }
}
