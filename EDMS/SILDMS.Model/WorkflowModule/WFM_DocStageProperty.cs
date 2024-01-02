using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.WorkflowModule
{
    public class WFM_DocStageProperty
    {
        public string DocTypePropertyID { get; set; }
        public string OwnerID { get; set; }
        public string DocCategoryID { get; set; }
        public string DocTypeID { get; set; }
        public int StageMapID { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string PropertyValue { get; set; }
        public int PropertySL { get; set; }
        public bool IsRequired { get; set; }
        public string SetBy { get; set; }
        public List<WFM_TableProperty> TableProperties { get; set; }
    }
    public class WFM_TableProperty
    {
        public string FieldTitle { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
        public string Master { get; set; }
        public int MaxSize { get; set; }
        public int RelationID { get; set; }
    }
}
