using SILDMS.Model.DocScanningModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.WorkflowModule
{
    public class VM_DocumentsProperty
    {
        public ICollection<DSM_DocProperty> Documents { get; set; }
        public ICollection<WFM_DocStageProperty> TypeProperties { get; set; }
        public ICollection<VM_ListTypeProperties> ListTypeProperties { get; set; }
    }
}
