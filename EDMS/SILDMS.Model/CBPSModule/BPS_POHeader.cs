using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_POHeader
    {
        public string PONo { get; set; }
        public string POType { get; set; }
        public string PODate { get; set; }

        public string VendorID { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string HardCopyMove { get; set; }

        public string OwnerLevelID { get; set; }
        public string LevelName { get; set; }

        public string OwnerID { get; set; }
        public string OwnerName { get; set; }

        public string DocCategoryCode { get; set; }
        public string DocCategoryID { get; set; }
        public string DocCategoryName { get; set; }

        public string DocTypeID { get; set; }
        public string DocTypeName { get; set; }

        public string IsReceivedForClearing { get; set; }


        public string BillParkingID { get; set; }
        public string BillTrackingNo { get; set; }
        public string BillReceiveID { get; set; }

        public string InvoicingParty { get; set; }
        public string InvoiceAmt { get; set; }

        public string InvoiceCurrency { get; set; }
        public string PreferedPayMode { get; set; }
        public string ProcessGroupID { get; set; }

        public string BillStatus { get; set; }
        public string BillSubmitDate { get; set; }
        public string InvoiceNo { get; set; }
        public string CurrentStage { get; set; }


        public string InvoiceDate { get; set; }
        public string BillReceivedBy { get; set; }
        public string BillReceivedAt { get; set; }
        public string PurchaseGroup { get; set; }




        public string InvoiceDocNo { get; set; }
        public string FiscalYear { get; set; }
        //public string CurrentStage { get; set; }


        public bool isSelected { get; set; }



        public string BillClearingID { get; set; }

        public string BillPostingID { get; set; }
        

        public IList<DSM_DocProperty> DocProperties { get; set; }
        public IList<DSM_DocPropIdentify> DocPropIdentifies { get; set; }
        public IList<BPS_POHeader> PoHeaders { get; set; }
    }
}
