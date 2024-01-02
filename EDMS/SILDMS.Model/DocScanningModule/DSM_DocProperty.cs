using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.DocScanningModule
{
    public class DSM_DocProperty
    {

        public string DocPropertyID { get; set; }
        [Required]
        public string DocCategoryID { get; set; }
        [Required]
        public string OwnerLevelID { get; set; }
        public int StageMapID { get; set; }
        [Required]
        public string OwnerID { get; set; }
        [Required]
        public string DocTypeID { get; set; }
        public string DocPropertySL { get; set; }
        public string UDDocPropertyCode { get; set; }
        [Required]
        public string DocPropertyName { get; set; }

        public string DocClassification { get; set; }
        public string PreservationPolicy { get; set; }
        public string PhysicalLocation { get; set; }
        public string Remarks { get; set; }
        public int? SerialNo { get; set; }
        public int? InformationValidityPeriod { get; set; }

        public string SetOn { get; set; }
        public string SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        [Required]
        public int? Status { get; set; }
        public string ConfigureColumnIds { get; set; }
        public bool isSelected { get; set; }

        public bool email { get; set; }
        public bool sms { get; set; }
        public bool obsulate { get; set; }
        public string emailSub { get; set; }
        public string emailBody { get; set; }
        public string smsBody { get; set; }
        public string emailId { get; set; }
        public string smsNum { get; set; }
        public string InfoValidOn { get; set; }
    }
}
