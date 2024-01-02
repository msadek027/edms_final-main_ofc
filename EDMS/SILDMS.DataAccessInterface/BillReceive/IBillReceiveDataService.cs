using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccessInterface.BillReceive
{
    public interface IBillReceiveDataService
    {

        BPS_POHeader GetPOInfo(string poNumber, string userID, out string errorNumber);

        BPS_POHeader FindBillReceiveForParking(string UserID, string BillTrackingNo, out string errorNumber);

        BPS_POHeader FindBillReceiveForPosting(string UserID, string BillTrackingNo, out string errorNumber);

        BPS_POHeader FindBillReceiveForClearing(string UserID, string BillTrackingNo, out string errorNumber);

        List<BPS_POHeader> FindBillReceiveForIssueToPayment(string UserID, string ClearingDateFrom,
            string ClearingDateTo, out string errorNumber);



        List<DSM_DocProperty> FindScannedDocumentInfoForBill(string UserID, string billReceiveID, out string errorNumber);

        DocumentsInfo FindReceivedBill(string UserID, string BillTrackingNo, out string errorNumber);

        List<DocSearch> GetIdentificationAttribute(string UserID, string BillTrackingNo, out string errorNumber);

        List<BPS_POHeader> GetVendorBillingInfo(string UserID, string BillTrackingNo, out string errorNumber);


        List<DSM_DocPropIdentify> AddDocumentInfo(DocumentsInfo _modelDocumentsInfo,
            string _selectedPropID, List<DocMetaValue> _docMetaValues,
            string _action, out string _errorNumber);


        BPS_POHeader GenerateBillTrackingNo(DocumentsInfo _modelDocumentsInfo,
            out string _errorNumber);

        BPS_POHeader GetLastBillInfo(string BillTrackNo,out string _errorNumber);


        void UpdateReceivedBillInfo(DocumentsInfo _modelDocumentsInfo,
            out string _errorNumber);

        string UpdateParkingBillStatus(string UserID, string billStatus, string BillTrackingNo, out string errorNumber);

        string ParkingBackToPreviousStage(string UserID, string billStatus, string BillTrackingNo, out string errorNumber);

        string PostingBackToPreviousStage(string UserID, string billStatus, string BillTrackingNo, out string errorNumber);
        string ClearingBackToPreviousStage(string UserID, string billStatus, string BillTrackingNo, out string errorNumber);

        string UpdatePostingBillStatus(string UserID, string billStatus, string BillTrackingNo, out string errorNumber);

        string UpdateClearingBillStatus(string UserID, string billStatus, string BillTrackingNo, out string errorNumber);
    }
}
