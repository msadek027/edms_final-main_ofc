using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.DocScanningModule
{
    public class DSM_DocPropIdentify
    {
        public string BillTrackingNo { get; set; }
        public string BillReceiveID { get; set; }
        public string DocPropIdentifyID { get; set; }
        public string BoothName { get; set; }
        public string ReceivedBy { get; set; }
        public int NumberOfMissingDocuments { get; set; }

        public string OwnerID { get; set; }

        public string DocCategoryID { get; set; }

        public string DocTypeID { get; set; }

        public string DocPropertyID { get; set; }
        public string DocPropertyName { get; set; }
       
        public string IdentificationCode { get; set; }
        public string IdentificationSL { get; set; }
        public string AttributeGroup { get; set; }
        public string FileCodeName { get; set; }

        [Required]
        public string IdentificationAttribute { get; set; }

        [Required]
        public int? IsRequired { get; set; }
        public int? IsAuto { get; set; }
        public int? IsRestricted { get; set; }
        public string Remarks { get; set; }
        public string SetOn { get; set; }
        public string SetBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public bool IsObsoleteIdentity { get; set; }







        [Required]
        public int Status { get; set; }
        public string MetaValue { get; set; }
        public string FileServerUrl { get; set; }
        public string DocumentID { get; set; }
        public string DocMetaID { get; set; }

        public string ServerIP { get; set; }
        public string ServerPort { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
        public string DocVersionID { get; set; }
        public string VersionNo { get; set; }

        public string DocMetaIDVersion { get; set; }
        public ddlDSMOwnerLevel OwnerLevel { get; set; }
        public ddlDSMOwner Owner { get; set; }
        public ddlDSMDocCategory DocCategory { get; set; }
        public ddlDSMDocType DocType { get; set; }
        public ddlDSMDocProperty DocProperty { get; set; }

        public string ConfigureColumnIds { get; set; }
        public string ObjectID { get; set; }
    }

    public class ddlDSMOwnerLevel
    {
        
        public string OwnerLevelID { get; set; }
        public string LevelName { get; set; }
    }

    public class ddlDSMOwner
    {
        
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
    }

    public class ddlDSMDocCategory
    {
        
        public string DocCategoryID { get; set; }
        public string DocCategoryName { get; set; }
    }

    public class ddlDSMDocType
    {
        
        public string DocTypeID { get; set; }
        public string DocTypeName { get; set; }
    }

    public class ddlDSMDocProperty
    {
        
        public string DocPropertyID { get; set; }
        public string DocPropertyName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsObsolete { get; set; }
    }

    public class ddlDSMDocPropIdentify
    {
        
        public string DocPropIdentifyID { get; set; }
        public string IdentificationAttribute { get; set; }
    }

    public class ddlRole
    {
        public string RoleID { get; set; }
        public string RoleTitle { get; set; }
    }

    public class DocMetaValue
    {
        public string DocPropertyID { get; set; }
        public string MetaValue { get; set; }
        public string VersionMetaValue { get; set; }
        public string Remarks { get; set; }
        public string DocPropIdentifyID { get; set; }
        public string DocumentID { get; set; }
        public string DocVersionID { get; set; }
        public string DocMetaID { get; set; }
        public string IdentificationAttribute { get; set; }

        public string FileServerURL { get; set; }
        public string ServerIP { get; set; }
        public string ServerID { get; set; }
        public string VersionNo { get; set; }

        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }

    }

    public class DocumentsInfo
    {
        public ddlDSMOwnerLevel OwnerLevel { get; set; }
        public ddlDSMOwner Owner { get; set; }
        public ddlDSMDocCategory DocCategory { get; set; }
        public ddlDSMDocType DocType { get; set; }
        public ddlDSMDocProperty DocProperty { get; set; }
        public string DidtributionOf { get; set; }
        public string Remarks { get; set; }
        public string DocVersionID { get; set; }
        public string SetBy { get; set; }
        public string ModifiedBy { get; set; }
        public string PageName { get; set; }

        public string OwnerLevelID { get; set; }
        public string OwnerID { get; set; }
        public string DocPropertyID { get; set; }
        public string DocTypeID { get; set; }
        public string DocCategoryID { get; set; }
        public string DocumentID { get; set; }
        public string DocCategoryName { get; set; }

        public string UploaderIP { get; set; }

        public string ConfigureColumnIds { get; set; }
        public string Status { get; set; }




        public string BillReceiveID { get; set; }
        public string BoothID { get; set; }
        public string ProcessGroupID { get; set; }

        public string PONo { get; set; }
        public string BillTrackingNo { get; set; }
        public string InvoicingParty { get; set; }
        public string InvoicingPartyCode { get; set; }
        public string InvoicingPartyName { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        
        public string InvoiceCurrency { get; set; }
        public string PreferedPayMode { get; set; }
        public string BillSubmittedBy { get; set; }
        public string BillSubmitDate { get; set; }
        public string BearerContactNo { get; set; }


        public string InvoiceAmt { get; set; }
        public string POType { get; set; }
        public string PODate { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public IList<DocMetaValue> DocMetaValues { get; set; }
        public IList<DocSearch> docList { get; set; }
    }


  }
