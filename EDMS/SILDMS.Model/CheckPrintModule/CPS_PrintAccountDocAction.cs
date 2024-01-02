using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CheckPrintModule
{
    public class CPS_PrintAccountDocAction : BaseModel
    {
        public string PrintAccDocActionID { get; set; }
        public string PrintAccountDocID { get; set; }
        public string StageID { get; set; }
        public string StageAction { get; set; }
        public string LeafNo { get; set; }
        public string PrintDocumentNo { get; set; }
        public string ActionWith { get; set; }
        public string ActionCause { get; set; }
        public string RecommendBy { get; set; }
        public string ActionNote { get; set; }
        public string SpecialInstruction { get; set; }
        public string ActionDoneBy { get; set; }
        public string InitiateDate { get; set; }
        public string ActionDate { get; set; }
        public string ReleaseDate { get; set; }
        public string BoothID { get; set; }
        public string ReferenceDocNo { get; set; }
        public string ReferenceDocDate { get; set; }
        public string ReceivedBy { get; set; }
        public string ContactNo { get; set; }
        public string DisburseState { get; set; }
        public string ProcessState { get; set; }

        #region

        public string Beneficiary { get; set; }
        public string BeneficiaryName { get; set; }
        public string DebitDate { get; set; }
        public string PayCurrency { get; set; }
        public decimal PayAmount { get; set; }
        public string DisplayInitiateDate { get; set; }
        public string DisplayReleaseDate { get; set; }
        public string DisplayReferenceDocDate { get; set; }
        public string VendorName { get; set; }
        public string InvoicingPartyName { get; set; }
        public string PaymentPurpose { get; set; }
        public string ProcessGroupName { get; set; }
        public bool IsCheck { get; set; }
        
        #endregion

    }
}
