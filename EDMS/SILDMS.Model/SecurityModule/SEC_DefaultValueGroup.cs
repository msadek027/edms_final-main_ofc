namespace SILDMS.Model.SecurityModule
{
    public class SEC_DefaultValueGroup : SEC_DefaultValueSetup
    {
        public string DefaultValueGroupID { get; set; }
        //public string DefaultValueSetupID { get; set; }
        //public string OwnerID { get; set; }
        //public string DocCategoryID { get; set; }
        //public string DocTypeID { get; set; }
        //public string DocPropertyID { get; set; }
        //public string DocPropIdentifyID { get; set; }
        public string ConfigureColumnID { get; set; }
        public string AutoValueGroupID { get; set; }
        public string ValueDrivenType { get; set; }
        //public int? UserLevel { get; set; }
        //public string Remarks { get; set; }
        //public string SetOn { get; set; }
        //public string SetBy { get; set; }
        //public string ModifiedOn { get; set; }
        //public string ModifiedBy { get; set; }
        //public int Status { get; set; }

        #region View Properties
        //public string OwnerName { get; set; }
        //public string DocCategoryName { get; set; }
        //public string DocTypeName { get; set; }
        //public string DocPropertyName { get; set; }
        public string AutoColumnTitle { get; set; }
        public string AutoValueGroup { get; set; }

        #endregion
    }
}
