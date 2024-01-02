using System.Collections.Generic;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.SecurityModule;
using SILDMS.Utillity;

namespace SILDMS.Service.DocSharing
{
    public interface IDocSharingService
    {
        ValidationResult GetDocumentByDocumentId(string DocumentID, out DSM_Documents document);
    }
}
