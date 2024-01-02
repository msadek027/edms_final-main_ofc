using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.WorkflowModule
{
    public class VM_ListTypeProperties
    {
        public List<WFM_TableProperty> ColumnList { get; set; }
        public string TableRefID { get; set; }
        public string PropertyName { get; set; }
    }
}
