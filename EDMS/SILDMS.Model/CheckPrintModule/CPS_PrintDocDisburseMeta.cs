using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CheckPrintModule
{
    public class CPS_PrintDocDisburseMeta : BaseModel
    {
        public string PrintDocDisburseMetaID { get; set; }
        public string PrintDocDisburseID { get; set; }
        public string DocMetaID { get; set; }
        public string DocumentID { get; set; }
        public string FileType { get; set; }
        public string DocPropIdentifyID { get; set; }
        public string MetaValue { get; set; }
        public int IsAuto { get; set; }
        public string DeletionIndecator { get; set; }
        public string CompletionStatus { get; set; }
    }
}
