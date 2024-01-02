using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class BPS_RestrictedCompany : BaseModel
    {
        public string RestrictedCompanyID { get; set; }
        public string StageID { get; set; }
        public string CompanyCode { get; set; } 

        #region ForView
        public string OwnerName { get; set; }

        #endregion
    }
}
