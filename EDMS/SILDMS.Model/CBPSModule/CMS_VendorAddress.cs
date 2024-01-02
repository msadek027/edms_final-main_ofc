using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class CMS_VendorAddress : BaseModel
    {
        public string VendorAddressCode { get; set; }
        public string VendorID { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }
        public string VendorCountry { get; set; }
        public string VendorPhoneNo { get; set; }
        public string VendorMobileNo { get; set; }
        public string VendorFax { get; set; }
        public string VendorEmail { get; set; }
        public string SkypeID { get; set; }
        public string ContactPerson { get; set; }
        public string ContactCellNo { get; set; }
        public string DataSource { get; set; }
        
    }
}
