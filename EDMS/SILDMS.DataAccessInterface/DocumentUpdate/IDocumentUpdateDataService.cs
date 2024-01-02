using SILDMS.Model.DocScanningModule;
using SILDMS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccessInterface.DocumentUpdate
{ 
    public interface IDocumentUpdateDataService
    {
        List<OriginalDocMeta> GetOriginalDocMeta(string _DocumentId, string _DocDistributionID, out string errorNumber);

        string UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo, string UserID, out string outStatus);

        void UpdateDocMailNotifyAndExpDate(string UserID, string DocID, string NotifyDate, string ExpDate, out string outStatus);
    }
}
