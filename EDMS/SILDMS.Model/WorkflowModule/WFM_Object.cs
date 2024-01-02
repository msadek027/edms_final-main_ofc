using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.WorkflowModule
{
    public class WFM_Object
    {
        public string ObjectID { get; set; }
        public string OwnerID { get; set; }
        public string DocCategoryID { get; set; }
        public string DocTypeID { get; set; }
        //public int StageMapID { get; set; }
        //public bool MkStatus { get; set; }
        //public bool CkStatus { get; set; }
        public bool PassedKey { get; set; }
    }
}
