using SILDMS.Model.DocScanningModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.WorkflowModule
{
    public class VM_DocumentsPropertyValues
    {
        public List<ObjectProperties> ObjectProperties { get; set; }
        public List<ObjectDocuments> ObjectDocuments { get; set; }
        public List<DataTable> ListProperties { get; set; }
        public List<PermittedDocument> PermittedDocuments { get; set; }
        public List<ParentStage> ParentStages { get; set; }
        public bool IsBacked { get; set; }
        public string BackReason { get; set; }
    }
    public class VM_DocumentsPropertyValuesAll : VM_DocumentsPropertyValues
    {
        public string LevelName { get; set; }
        public string OwnerName { get; set; }
        public string DocCategoryName { get; set; }
        public string DocTypeName { get; set; }
        public List<StageChangeHistory> StageChangeHistory { get; set; }
    }

    public class ObjectProperties
    {
        public string ObjectPropertyID { get; set; }
        public string DocTypePropertyID { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public int StageMapID { get; set; }
        public bool IsRequired { get; set; }
        public string PropertyType { get; set; }
        public string TableRefID { get; set; }
    }

    public class ObjectDocuments : DSM_Documents
    {
        public int StageMapID { get; set; }
        public string DocClassification { get; set; }
        public bool IsPermitted { get; set; }
    }

    public class StageChangeHistory
    {
        public string FromStage { get; set; }
        public string ToStage { get; set; }
        public string FromStageName { get; set; }
        public string ToStageName { get; set; }
        public bool IsMakeOrCheck { get; set; }
        public int TypeOfChange { get; set; }
        public string Reason { get; set; }
        public string SetOn { get; set; }
        public string SetBy { get; set; }
    }
    public class PermittedDocument
    {
        public string DocPropertyID { get; set; }
     
    }

    public class ParentStage
    {
        public int StageMapID { get; set; }
        public string StageName { get; set; }
        public int StageID { get; set; }
    }
}
