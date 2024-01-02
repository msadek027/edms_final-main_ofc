using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.AutoValueSetup;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.SecurityModule;

namespace SILDMS.DataAccess.AutoValueSetup
{
    public class AutoValueSetupDataService: IAutoValueSetupDataService
    {
        private readonly string spStatusParam = "@p_Status";
        public string AddAutoValueSetup(SEC_AutoValueSetup modelAutoValueSetup, string action, out string errorNumber)
        {
            errorNumber = String.Empty;
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;
                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("SetAutoValueSetup"))
                {
                    db.AddInParameter(dbCommandWrapper, "@AutoValueSetupID", SqlDbType.NVarChar, modelAutoValueSetup.AutoValueSetupID);
                    db.AddInParameter(dbCommandWrapper, "@ConfigureID", SqlDbType.NVarChar, modelAutoValueSetup.ConfigureID);
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, modelAutoValueSetup.Owner.OwnerID);
                    db.AddInParameter(dbCommandWrapper, "@DocCategoryID ", SqlDbType.NVarChar, modelAutoValueSetup.DocCategory.DocCategoryID);
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, modelAutoValueSetup.DocType.DocTypeID);
                    db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, modelAutoValueSetup.DocProperty.DocPropertyID);
                    db.AddInParameter(dbCommandWrapper, "@DocPropIdentifyID", SqlDbType.NVarChar, modelAutoValueSetup.DocPropIdentify.DocPropIdentifyID);

                    db.AddInParameter(dbCommandWrapper, "@AutoValueID", SqlDbType.NVarChar, modelAutoValueSetup.AutoValueID);
                    db.AddInParameter(dbCommandWrapper, "@ConfigureColumnID", SqlDbType.NVarChar, modelAutoValueSetup.ConfigureColumnID);
                    db.AddInParameter(dbCommandWrapper, "@AutoValueFormat", SqlDbType.NVarChar, modelAutoValueSetup.AutoValueFormat);
                    db.AddInParameter(dbCommandWrapper, "@IncrementValueLength", SqlDbType.NVarChar, modelAutoValueSetup.IncrementValueLength);

                    db.AddInParameter(dbCommandWrapper, "@IncrementReplaceBy", SqlDbType.NVarChar, modelAutoValueSetup.IncrementReplaceBy);
                    db.AddInParameter(dbCommandWrapper, "@IncrementRestartBy", SqlDbType.NVarChar, modelAutoValueSetup.IncrementRestartBy);
                    db.AddInParameter(dbCommandWrapper, "@MaxValue", SqlDbType.NVarChar, modelAutoValueSetup.MaxValue);
                    db.AddInParameter(dbCommandWrapper, "@UserLevel", SqlDbType.Int, modelAutoValueSetup.UserLevel);
                    db.AddInParameter(dbCommandWrapper, "@Remarks", SqlDbType.NVarChar, modelAutoValueSetup.Remarks);

                    db.AddInParameter(dbCommandWrapper, "@SetBy ", SqlDbType.NVarChar, modelAutoValueSetup.SetBy);
                    db.AddInParameter(dbCommandWrapper, "@ModifiedBy", SqlDbType.NVarChar, modelAutoValueSetup.ModifiedBy);
                    db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.Int, modelAutoValueSetup.Status);
                    db.AddInParameter(dbCommandWrapper, "@Action", SqlDbType.VarChar, action);
                    db.AddOutParameter(dbCommandWrapper, spStatusParam, SqlDbType.VarChar, 10);

                    db.ExecuteNonQuery(dbCommandWrapper);

                    if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                    {
                        errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                    }
                }
            }
            catch (Exception ex)
            {
                errorNumber = "E404";
            }
            return errorNumber;
        }

        public List<SEC_AutoValueSetup> GetAutoValueSetupData(string _ConfigureID,
            string _ConfigureColumnID, string _UserID, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<SEC_AutoValueSetup> autoValueSetups = new List<SEC_AutoValueSetup>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetAutoValueSetupData"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, _UserID);
                db.AddInParameter(dbCommandWrapper, "@ConfigureID", SqlDbType.VarChar, _ConfigureID);
                db.AddInParameter(dbCommandWrapper, "@ConfigureColumnID", SqlDbType.VarChar, _ConfigureColumnID);

                

                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                // Execute SP.
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        autoValueSetups = dt1.AsEnumerable().Select(reader => new SEC_AutoValueSetup
                        {
                            AutoValueSetupID = reader.GetString("AutoValueSetupID"),
                            ConfigureID = reader.GetString("ConfigureID"),
                            AutoValueID = reader.GetString("AutoValueID"),
                            ConfigureColumnID = reader.GetString("ConfigureColumnID"),
                            AutoColumnTitle = reader.GetString("AutoColumnTitle"),
                            AutoValueFormat = reader.GetString("AutoValueFormat"),
                            IncrementValueLength = reader.GetString("IncrementValueLength"),
                            IncrementReplaceBy = reader.GetString("IncrementReplaceBy"),
                            IncrementRestartBy = reader.GetString("IncrementRestartBy"),
                            MaxValue = reader.GetString("MaxValue"),
                            SetOn = reader.GetString("SetOn"),
                            SetBy = reader.GetString("SetBy"),
                            ModifiedOn = reader.GetString("ModifiedOn"),
                            ModifiedBy = reader.GetString("ModifiedBy"),
                            Status = reader.GetInt32("Status"),
                            Remarks = reader.GetString("Remarks")
                        }).ToList();
                    }
                }
            }
            return autoValueSetups;
        }


        public List<SEC_Menu> GetConfiguredMenuList(string _UserID, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<SEC_Menu> secMenus = new List<SEC_Menu>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetConfiguredMenuList"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, _UserID);
                
                db.AddOutParameter(dbCommandWrapper, spStatusParam, DbType.String, 10);
                // Execute SP.
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, spStatusParam).IsNullOrZero())
                {
                    // Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        secMenus = dt1.AsEnumerable().Select(reader => new SEC_Menu
                        {
                            
                            MenuID = reader.GetString("MenuID"),
                            MenuTitle = reader.GetString("MenuTitle")
                            
                        }).ToList();
                    }
                }
            }
            return secMenus;
        }


        public string GetConfigureColumnList(string menuUrl, string ownerID,string catId, string typeID, string property, string propertyIdentity)
        {
       
            string id="";

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetConfigColumnIDs"))
            {
                db.AddInParameter(dbCommandWrapper, "@MenuUrl", SqlDbType.VarChar, menuUrl);     
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.VarChar, ownerID);
                db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.VarChar, catId);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.VarChar, typeID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.VarChar, property);
                db.AddInParameter(dbCommandWrapper, "@DocPropIdentifyID", SqlDbType.VarChar, propertyIdentity);
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    id= dt1.Rows[0].ItemArray[0].ToString();
                }              
            }
            return id;
        }
    }
}
