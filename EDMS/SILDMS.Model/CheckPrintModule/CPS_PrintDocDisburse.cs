using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CheckPrintModule
{
    public class CPS_PrintDocDisburse : BaseModel
    {

        public string PrintDocDisburseID { get; set; }
        public string DocumentID { get; set; }
        public string PrintAccDocActionID { get; set; }
        public string PrintAccountDocID { get; set; }
        public string DocPropertyID { get; set; }
        public string FileOriginalName { get; set; }
        public string FileCodeName { get; set; }
        public string FileExtension { get; set; }
        public string FileServerUrl { get; set; }
        public string FileServerIP { get; set; }
        public string FtpPort { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
        public int HasVersion { get; set; }
        public string UploaderIP { get; set; }
        public string DeletionIndecator { get; set; }
        public string CompletionStatus { get; set; }
        public bool IsCheck { get; set; }
    }
}
