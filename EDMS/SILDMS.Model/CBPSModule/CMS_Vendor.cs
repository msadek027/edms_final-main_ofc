using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class CMSVendor : BaseModel
    {
        public string VendorID { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorAddressCode { get; set; }
        public string VendorGroup { get; set; }
        public string VendorCategory { get; set; }
        public string VendorType { get; set; }
        public int IsUser { get; set; }
        public string ConcernVendor { get; set; }
        public string TransBank { get; set; }
        public string TransAccountName { get; set; }
        public string TransAccountNo { get; set; }
        public string DataSource { get; set; }

        #region Properties for view

        public string VendorAddress1 { get; set; }
        public string VendorEmail { get; set; }
        public string ContactPerson { get; set; }
        public string ContactCellNo { get; set; }
        public string VendorMobileNo { get; set; }
        public string VendorNameDisplay { get; set; }


        #endregion
    }
}
