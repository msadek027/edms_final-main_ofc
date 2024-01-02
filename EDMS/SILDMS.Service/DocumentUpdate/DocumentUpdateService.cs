using SILDMS.DataAccessInterface.DocumentUpdate;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Service.DocumentUpdate
{
    public class DocumentUpdateService: IDocumentUpdateService
    {
        private readonly IDocumentUpdateDataService _iDocumentUpdateDataService;
        private readonly ILocalizationService _localizationService;
        private string errorNumber = "";
      
        public ValidationResult GetOriginalDocMeta(string _DocumentId, string _DocDistributionID, out List<OriginalDocMeta> metaList)
        {
            metaList = _iDocumentUpdateDataService.GetOriginalDocMeta(_DocumentId, _DocDistributionID, out errorNumber);

            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;
        }



        public ValidationResult UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo, string UserID, out string outStatus)
        {
            _iDocumentUpdateDataService.UpdateDocMetaInfo(_modelDocumentsInfo, UserID, out outStatus);

            if (outStatus.Length > 0)
            {
                return new ValidationResult(outStatus, _localizationService.GetResource(outStatus));
            }

            return ValidationResult.Success;
        }
       


        public ValidationResult UpdateDocMailNotifyAndExpDate(string UserID, string DocID, string NotifyDate, string ExpDate, out string outStatus)
        {
            _iDocumentUpdateDataService.UpdateDocMailNotifyAndExpDate(UserID, DocID, NotifyDate, ExpDate, out outStatus);

            if (outStatus.Length > 0)
            {
                return new ValidationResult(outStatus, _localizationService.GetResource(outStatus));
            }


            return ValidationResult.Success;
        }
    }
}
