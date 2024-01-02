using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CheckPrintModule
{
    public class BPS_ClearingPrintTrans : BaseModel
    {
        public string ClearingPrintTransID { get; set; }
        public string TransDate { get; set; }
        public string TransType { get; set; }
        public string BillClearingID { get; set; }
        public string CompanyCode { get; set; }
        public string InvoiceDocNo { get; set; }
        public string ClearingTransStatus { get; set; }
        public string ClearingNote { get; set; }
        public string ReleaseIndicator { get; set; }
        public string DeletionIndecator { get; set; }
        public string CompletionStatus { get; set; }
        public string ProcessState { get; set; }
        public DateTime InitiateDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string StageID { get; set; }
        public string ProcessGroupID { get; set; }
    }
}
