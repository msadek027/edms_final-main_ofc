using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.DocumentUpdate
{
    public interface IDocumentUpdateService
    {
        ValidationResult GetOriginalDocMeta(string _DocumentId, string _DocDistributionID, out List<OriginalDocMeta> metaList);

        ValidationResult UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo, string UserID, out string outStatus);

        ValidationResult UpdateDocMailNotifyAndExpDate(string UserID, string DocID, string NotifyDate, string ExpDate, out string outStatus);
    }
}
