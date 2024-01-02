using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_BillProcessGroup : BaseModel
    {
        public string ProcessGroupID { get; set; }
        public int ProcessGroupNo { get; set; }
        public string DataSource { get; set; }
        public string ProcessGroupName { get; set; }
        public string ShortName { get; set; }
        public string IdentyFlag { get; set; }
    }
}
