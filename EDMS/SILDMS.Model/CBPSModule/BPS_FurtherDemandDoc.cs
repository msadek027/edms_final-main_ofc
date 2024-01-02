using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_FurtherDemandDoc : BaseModel
    {
        public string DemandDocID { get; set; }
        public string DemandID { get; set; }
        public string DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentNature { get; set; }
        public int? NoOfCopy { get; set; }
        public string NecessityType { get; set; }
        public string RequireBy { get; set; }
        public string DocNote { get; set; }
        public string DeletionIndicator { get; set; }
        public string CompletionStatus { get; set; }

    }
}
