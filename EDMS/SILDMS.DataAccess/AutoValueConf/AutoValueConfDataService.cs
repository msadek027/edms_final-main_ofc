using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.AutoValueConf;
using SILDMS.Model.SecurityModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccess.AutoValueConf
{
    public class AutoValueConfDataService : IAutoValueConfDataService
    {
        
        #region Fields
        private readonly string spErrorParam = "@p_Error";
        private readonly string spStatusParam = "@p_Status";
        #endregion

        public List<SysDbTables> GetDbTables(string tbl, out string errorNumber)
        {
            errorNumber = string.Empty;
            var tblInfoList = new List<SysDbTables>();
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("GetDbTableInfo"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@TableName", SqlDbType.VarChar, tbl);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                // Execute SP.
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        var dt1 = ds.Tables[0];
                        tblInfoList = dt1.AsEnumerable().Select(reader => new SysDbTables
                        {
                            TableName = reader.GetString("TableName"),
                            ColumnName = reader.GetString("ColumnName"),
                            Position = reader.GetString("Position"),
                            IsNull = reader.GetString("IsNull")
                        }).ToList();
                    }
                }
            }
            return tblInfoList;
        }

        public List<SEC_ConfigureColumn> GetAutoValueConfiguration(SEC_ConfigurePage obj, out string errorNumber)
        {
            errorNumber = string.Empty;
            var autoValues = new List<SEC_ConfigureColumn>();
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("GetAutoValue"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@MenuID", SqlDbType.VarChar, obj.MenuID);
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.VarChar, obj.OwnerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.VarChar, obj.DocCategoryID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.VarChar, obj.DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.VarChar, obj.DocPropertyID);
                db.AddInParameter(dbCommandWrapper, "@DocPropIdentifyID", SqlDbType.VarChar, obj.DocPropIdentifyID);
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                 // Execute SP.
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        var dt1 = ds.Tables[0];
                        autoValues = dt1.AsEnumerable().Select(x => new SEC_ConfigureColumn
                        {
                            ConfigureColumnID = x.GetString("ConfigureColumnID"),
                            ConfigureID = x.GetString("ConfigureID"),
                            ConfigureCategory = x.GetString("ConfigureCategory"),
                            ConfigurationFor = x.GetString("ConfigurationFor"),
                            ConfigureToTable = x.GetString("ConfigureToTable"),
                            ConfigureToColumn = x.GetString("ConfigureToColumn"),
                            AutoColumnTitle = x.GetString("AutoColumnTitle"),
                            AutoValueGroupID = x.GetString("AutoValueGroupID"),
                            AutoValueGroup = x.GetString("AutoValueGroup"),
                            CssClass = x.GetString("CssClass"),
                            PrimaryKeyColumn = x.GetString("PrimaryKeyColumn"),
                            Remarks = x.GetString("Remarks"),
                            Status = x.GetInt16("Status")
                        }).ToList();
                    }
                }
            }
            return autoValues;
        }

        public string ManipulateAutoValue(SEC_ConfigureColumn obj, string action, out string errorNumber)
        {
            errorNumber = string.Empty;
            try
            {
                var factory = new DatabaseProviderFactory();
                var db = factory.CreateDefault() as SqlDatabase;
                using (var dbCommandWrapper = db.GetStoredProcCommand("SetAutoValue"))
                {
                    db.AddInParameter(dbCommandWrapper, "@ConfigureID", SqlDbType.NVarChar, obj.ConfigureID);
                    db.AddInParameter(dbCommandWrapper, "@ConfigureColumnID", SqlDbType.NVarChar, obj.ConfigureColumnID);
                    db.AddInParameter(dbCommandWrapper, "@MenuID", SqlDbType.NVarChar, obj.MenuID);
                    db.AddInParameter(dbCommandWrapper, "@MenuTitle", SqlDbType.NVarChar, obj.MenuTitle);
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, obj.OwnerID);
                    db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, obj.DocCategoryID);
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, obj.DocTypeID);
                    db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, obj.DocPropertyID);
                    db.AddInParameter(dbCommandWrapper, "@DocPropIdentifyID", SqlDbType.NVarChar, obj.DocPropIdentifyID);
                    db.AddInParameter(dbCommandWrapper, "@ConfigureCategory", SqlDbType.NVarChar, obj.ConfigureCategory);
                    db.AddInParameter(dbCommandWrapper, "@ConfigurationFor", SqlDbType.NVarChar, obj.ConfigurationFor);
                    db.AddInParameter(dbCommandWrapper, "@ConfigureToTable", SqlDbType.NVarChar, obj.ConfigureToTable);
                    db.AddInParameter(dbCommandWrapper, "@ConfigureToColumn", SqlDbType.NVarChar, obj.ConfigureToColumn);
                    db.AddInParameter(dbCommandWrapper, "@AutoColumnTitle", SqlDbType.NVarChar, obj.AutoColumnTitle);
                    db.AddInParameter(dbCommandWrapper, "@AutoValueGroupID", SqlDbType.NVarChar, obj.AutoValueGroupID);
                    db.AddInParameter(dbCommandWrapper, "@AutoValueGroup", SqlDbType.NVarChar, obj.AutoValueGroup);
                    db.AddInParameter(dbCommandWrapper, "@CssClass", SqlDbType.NVarChar, obj.CssClass);
                    db.AddInParameter(dbCommandWrapper, "@PrimaryKeyColumn", SqlDbType.NVarChar, obj.PrimaryKeyColumn);
                    db.AddInParameter(dbCommandWrapper, "@Remarks", SqlDbType.NVarChar, obj.Remarks);
                    db.AddInParameter(dbCommandWrapper, "@SetBy", SqlDbType.NVarChar, obj.SetBy);
                    db.AddInParameter(dbCommandWrapper, "@ModifiedBy", SqlDbType.NVarChar, obj.ModifiedBy);
                    db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.Int, obj.Status);
                    db.AddInParameter(dbCommandWrapper, "@Action", SqlDbType.NVarChar, action);
                    db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);
                    db.ExecuteNonQuery(dbCommandWrapper);
                    // Getting output parameters and setting response details.
                    if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                    {
                        // Get the error number, if error occurred.
                        errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                    }
                }
            }
            catch(Exception ex)
            {

                errorNumber = "E404"; // Log ex.Message  Insert Log Table 
            }
            return errorNumber;
        }
    }
}
