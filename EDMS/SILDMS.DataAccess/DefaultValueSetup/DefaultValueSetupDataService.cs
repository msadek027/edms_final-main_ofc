using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.DefaultValueSetup;
using SILDMS.Model.SecurityModule;

namespace SILDMS.DataAccess.DefaultValueSetup
{
    public class DefaultValueSetupDataService : IDefaultValueSetupDataService
    {
        #region Fields
        private readonly string spErrorParam = "@p_Error";
        private readonly string spStatusParam = "@p_Status";
        #endregion
        public List<SEC_DefaultValue> GetDefaultValues(SEC_DefaultValue obj, out string errorNumber)
        {
            var result = new List<SEC_DefaultValue>();
            errorNumber = string.Empty;
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("GetDefaultValueSetup"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.VarChar, obj.OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.VarChar, obj.DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.VarChar, obj.DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.VarChar, obj.DocPropertyID);
                //db.AddInParameter(dbCommandWrapper, "@DocPropIdentifyID", SqlDbType.VarChar, obj.DocPropIdentifyID);
                //db.AddInParameter(dbCommandWrapper, "@ConfigureColumnID", SqlDbType.VarChar, obj.ConfigureColumnID);
                db.AddInParameter(dbCommandWrapper, "@AutoValueGroupID", SqlDbType.VarChar, obj.AutoValueGroupID);
                db.AddInParameter(dbCommandWrapper, "@ValueDrivenType", SqlDbType.VarChar, obj.ValueDrivenType);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                var ds = db.ExecuteDataSet(dbCommandWrapper);
                if (!db.GetParameterValue(dbCommandWrapper, "@p_Status").IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, "@p_Status").PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return result;
                    var dt1 = ds.Tables[0];
                    result = dt1.AsEnumerable().Select(x => new SEC_DefaultValue
                    {
                        DefaultValueID = x.GetString("DefaultValueID"),
                        DefaultValueGroupID = x.GetString("DefaultValueGroupID"),
                        DefaultValueSetupID = x.GetString("DefaultValueSetupID"),
                        ConfigureColumnID = x.GetString("ConfigureColumnID"),
                        AutoColumnTitle = x.GetString("AutoColumnTitle"),
                        DefaultValue = x.GetString("DefaultValue"),
                        UpdateAllowed = x.GetString("UpdateAllowed"),
                        //VirtualColumn = x.GetString("VirtualColumn"),
                        Remarks = x.GetString("Remarks"),
                        Status = x.GetInt16("Status")
                    }).ToList();
                }
            }
            return result;
        }

        public object GetDefaultPrimary(string id, out string errorNumber)
        {
            var result = new List<SEC_ConfigureColumn>();
            errorNumber = string.Empty;
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("GetConfigureColumnByMenu"))
            {
                db.AddInParameter(dbCommandWrapper, "@MenuID", SqlDbType.VarChar, id);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                var ds = db.ExecuteDataSet(dbCommandWrapper);
                if (!db.GetParameterValue(dbCommandWrapper, "@p_Status").IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, "@p_Status").PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return new {};
                    var dt1 = ds.Tables[0];
                    result = dt1.AsEnumerable().Select(x => new SEC_ConfigureColumn
                    {
                        ConfigureID = x.GetString("ConfigureID"),
                        MenuID = x.GetString("MenuID"),
                        MenuUrl = x.GetString("MenuUrl"),
                        MenuTitle = x.GetString("MenuTitle"),
                        OwnerID = x.GetString("OwnerID"),
                        OwnerName = x.GetString("OwnerName"),
                        DocCategoryID = x.GetString("DocCategoryID"),
                        DocCategoryName = x.GetString("DocCategoryName"),
                        DocTypeID = x.GetString("DocTypeID"),
                        DocTypeName = x.GetString("DocTypeName"),
                        DocPropertyID = x.GetString("DocPropertyID"),
                        DocPropertyName = x.GetString("DocPropertyName"),
                        DocPropIdentifyID = x.GetString("DocPropIdentifyID"),
                        ConfigureColumnID = x.GetString("ConfigureColumnID"),
                        ConfigureCategory = x.GetString("ConfigureCategory"),
                        ConfigurationFor = x.GetString("ConfigurationFor"),
                        AutoValueGroupID = x.GetString("AutoValueGroupID"),
                        AutoValueGroup = x.GetString("AutoValueGroupID"),
                        AutoColumnTitle = x.GetString("AutoColumnTitle"),
                        ConfigureToTable = x.GetString("ConfigureToTable"),
                        ConfigureToColumn = x.GetString("ConfigureToColumn"),
                        LevelName = x.GetString("LevelName")
                    }).ToList();
                }
            }
            return result.FirstOrDefault();
        }

        public string ManipulateDefaultValues(SEC_DefaultValue obj, string action, out string errorNumber)
        {
            errorNumber = string.Empty;
            try
            {
                var factory = new DatabaseProviderFactory();
                var db = factory.CreateDefault() as SqlDatabase;
                using (var dbCommandWrapper = db.GetStoredProcCommand("SetDefaultSetup"))
                {
                    db.AddInParameter(dbCommandWrapper, "@DefaultValueSetupID", SqlDbType.NVarChar, obj.DefaultValueSetupID);
                    db.AddInParameter(dbCommandWrapper, "@DefaultValueGroupID", SqlDbType.NVarChar, obj.DefaultValueGroupID);
                    db.AddInParameter(dbCommandWrapper, "@DefaultValueID", SqlDbType.NVarChar, obj.DefaultValueID);
                    db.AddInParameter(dbCommandWrapper, "@ConfigureID", SqlDbType.NVarChar, obj.ConfigureID);
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, obj.OwnerID);
                    db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, obj.DocCategoryID);
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, obj.DocTypeID);
                    db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, obj.DocPropertyID);
                    db.AddInParameter(dbCommandWrapper, "@DocPropIdentifyID", SqlDbType.NVarChar, obj.DocPropIdentifyID);
                    db.AddInParameter(dbCommandWrapper, "@ConfigureColumnID", SqlDbType.NVarChar, obj.ConfigureColumnID);
                    db.AddInParameter(dbCommandWrapper, "@AutoValueGroupID", SqlDbType.NVarChar, obj.AutoValueGroupID);
                    db.AddInParameter(dbCommandWrapper, "@ValueDrivenType", SqlDbType.NVarChar, obj.ValueDrivenType);
                    db.AddInParameter(dbCommandWrapper, "@DefaultValue", SqlDbType.NVarChar, obj.DefaultValue);
                    db.AddInParameter(dbCommandWrapper, "@UpdateAllowed", SqlDbType.NVarChar, obj.UpdateAllowed);
                    //db.AddInParameter(dbCommandWrapper, "@VirtualColumn", SqlDbType.NVarChar, obj.VirtualColumn);
                    db.AddInParameter(dbCommandWrapper, "@Remarks", SqlDbType.NVarChar, obj.Remarks);
                    db.AddInParameter(dbCommandWrapper, "@SetBy", SqlDbType.NVarChar, obj.SetBy);
                    db.AddInParameter(dbCommandWrapper, "@ModifiedBy", SqlDbType.NVarChar, obj.ModifiedBy);
                    db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.NVarChar, obj.Status);
                    db.AddInParameter(dbCommandWrapper, "@Action", SqlDbType.NVarChar, action);
                    db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);
                    // Execute SP.
                    db.ExecuteNonQuery(dbCommandWrapper);
                    // Getting output parameters and setting response details.
                    if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                    {
                        // Get the error number, if error occurred.
                        errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                    }
                }
            }
            catch (Exception ex)
            {
                errorNumber = "E404"; // Log ex.Message  Insert Log Table 
            }
            return errorNumber;
        }

        public List<SEC_Menu> GetMenusForDefaultValue()
        {
            var result = new List<SEC_Menu>();
            //errorNumber = string.Empty;
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("GetAllMenuForDefaultValue"))
            {
                var ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds.Tables[0].Rows.Count <= 0) return result;
                var dt1 = ds.Tables[0];
                result = dt1.AsEnumerable().Select(x => new SEC_Menu
                {
                    MenuID = x.GetString("MenuID"),
                    MenuTitle = x.GetString("MenuTitle"),
                    MenuUrl = x.GetString("MenuUrl"),
                    MenuOrder = x.GetInt16("MenuOrder")
                }).ToList();
            }
            return result;
        }

        public List<SEC_ConfigureColumn> GetColumnForDefaultValue(SEC_ConfigurePage obj, out string errorNumber)
        {
            var result = new List<SEC_ConfigureColumn>();
            errorNumber = string.Empty;
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("GetColumnsForDefaultValue"))
            {
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.VarChar, obj.OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.VarChar, obj.DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.VarChar, obj.DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.VarChar, obj.DocPropertyID);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                var ds = db.ExecuteDataSet(dbCommandWrapper);
                if (!db.GetParameterValue(dbCommandWrapper, "@p_Status").IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, "@p_Status").PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return result;
                    var dt1 = ds.Tables[0];
                    result = dt1.AsEnumerable().Select(x => new SEC_ConfigureColumn
                    {  
                        ConfigureColumnID = x.GetString("ConfigureColumnID"),
                        AutoColumnTitle = x.GetString("AutoColumnTitle"),
                        ConfigureToTable = x.GetString("ConfigureToTable"),
                        ConfigureToColumn = x.GetString("ConfigureToColumn")
                    }).ToList();
                }
            }
            return result;
        }
    }
}
