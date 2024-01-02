using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.Model.CheckPrintModule
{
    public class CMSBank
    {
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string CountryofOrigin { get; set; }
        public string BankCategory { get; set; }
        public string BankType { get; set; }
        public string BIN { get; set; }
        public string OwnerID { get; set; }
        public string UserLevel { get; set; }

        public string SetOn { get; set; }
        public string SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }

        public DSM_Owner Owner { get; set; }

        public IList<CMSBankBranch> Branches { get; set; }

    }

    public class CMSBankBranch
    {
        public string BankID { get; set; }
        public string BranchID { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string BranchPhone { get; set; }
        public string BranchFax { get; set; }
        public string BranchEmail { get; set; }
        public string BranchPerson { get; set; }
        public string PersonDesignation { get; set; }
        public string PersonDepartment { get; set; }
        public string PersonCell { get; set; }
        public string BranchCode { get; set; }
        public string BranchBftn { get; set; }
        public string BranchRemarks { get; set; }
        public string OwnerID { get; set; }
        public string UserLevel { get; set; }
        public string SetOn { get; set; }
        public string SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Status { get; set; }

    }
}
