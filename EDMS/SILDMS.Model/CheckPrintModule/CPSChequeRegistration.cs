using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.Model.CheckPrintModule
{
    public class CPSChequeRegistration
    {
        public string RegistrationID { get; set; }
        public string RequisitionID { get; set; }
        public string AccountID { get; set; }
        public string ReqSlipNo { get; set; }
        public string ChequebookNo { get; set; }
        public int? NoOfLeaf { get; set; }
        public string FirstLeafNo { get; set; }
        public string LastLefNo { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string RegistrationNote { get; set; }
        public string RegistrationBy { get; set; }
        public string ChequebookStatus { get; set; }
        public string OwnerID { get; set; }
        public int? UserLevel { get; set; }
        public DateTime? SetOn { get; set; }
        public string SetBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int? Status { get; set; }

        public DSM_Owner Owner { get; set; }
        public CMSBank Bank { get; set; }
        public CMSBankBranch Branch { get; set; }
        public CPSBankBranchAccount Account { get; set; }
    }
}
