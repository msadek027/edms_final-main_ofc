using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.SecurityModule
{
    public class SEC_ConfigurePage
    {
        public string ConfigureID { get; set; }
        public string MenuID { get; set; }
        public string MenuUrl { get; set; }
        public string MenuTitle { get; set; }
        public string OwnerID { get; set; }
        public string DocCategoryID { get; set; }
        public string DocTypeID { get; set; }
        public string DocPropertyID { get; set; }
        public string DocPropIdentifyID { get; set; }
        public int? UserLevel { get; set; }
        public string Remarks { get; set; }
        public string SetOn { get; set; }
        public string SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int Status { get; set; }

        #region View Fields
        public string OwnerName { get; set; }
        public string DocCategoryName { get; set; }
        public string DocTypeName { get; set; }
        public string DocPropertyName { get; set; }
        public string LevelName { get; set; }

        #endregion
    }
}
