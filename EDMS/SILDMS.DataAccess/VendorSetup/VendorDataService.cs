using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.VendorSetup;
using SILDMS.Model.CBPSModule;
using SILDMS.Model.DocScanningModule;

namespace SILDMS.DataAccess.VendorSetup
{
    public class VendorDataService : IVendorDataService
    {

        #region Fields

        private readonly string _spStatusParam;

        #endregion

        #region Constructor
        public VendorDataService()
        {
            _spStatusParam = "@p_Status";
        }
        #endregion

        public List<DSM_Owner> GetOwners(out string errornumber)
        {
            errornumber = string.Empty;
            var owners = new List<DSM_Owner>();

            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetAllOwnerByCompany"))
            {
                // Set parameters 
                db.AddOutParameter(dbCommandWrapper, _spStatusParam, DbType.String, 10);
                // Execute SP.
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                {
                    //Get the error number, if error occurred.
                    errornumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return owners;
                    var dt1 = ds.Tables[0];
                    owners = dt1.AsEnumerable().Select(reader => new DSM_Owner
                    {
                        OwnerID = reader.GetString("OwnerID"),
                        OwnerName = reader.GetString("OwnerName"),
                        UDOwnerCode = reader.GetString("UDOwnerCode"),
                        OwnerLevelID = reader.GetString("OwnerLevelID")
                    }).ToList();
                }
            }
            return owners;
        }

        public List<CMSVendor> GetVendors(string vendorId, string ownerId, out string errorNumber)
        {
            errorNumber = string.Empty;
            var vendors = new List<CMSVendor>();

            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetVendorsWithAddress"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@VendorID", SqlDbType.NVarChar, vendorId);
                db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, ownerId);
                db.AddOutParameter(dbCommandWrapper, _spStatusParam, DbType.String, 10);
                // Execute SP.
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                {
                    //Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return vendors;
                    var dt1 = ds.Tables[0];
                    vendors = dt1.AsEnumerable().Select(reader => new CMSVendor
                    {
                        VendorID = reader.GetString("VendorID"),
                        VendorCode = reader.GetString("VendorCode"),
                        VendorName = reader.GetString("VendorName"),
                        VendorNameDisplay = reader.GetString("VendorNameDisplay"),
                        VendorAddressCode = reader.GetString("VendorAddressCode"),
                        VendorGroup = reader.GetString("VendorGroup"),
                        VendorCategory = reader.GetString("VendorCategory"),
                        VendorType = reader.GetString("VendorType"),
                        IsUser = reader.GetInt16("IsUser"),
                        ConcernVendor = reader.GetString("ConcernVendor"),
                        TransBank = reader.GetString("TransBank"),
                        TransAccountName = reader.GetString("TransAccountName"),
                        TransAccountNo = reader.GetString("TransAccountNo"),
                        DataSource = reader.GetString("DataSource"),
                        OwnerID = reader.GetString("OwnerID"),
                        VendorAddress1 = reader.GetString("VendorAddress1"),
                        VendorEmail = reader.GetString("VendorEmail"),
                        ContactPerson = reader.GetString("ContactPerson"),
                        ContactCellNo = reader.GetString("ContactCellNo"),
                        VendorMobileNo = reader.GetString("VendorMobileNo")
                    }).ToList();
                }
            }
            return vendors;
        }

        public List<CMS_VendorAddress> GetVendorAddresses(string vendorId, string addressId, out string errorNumber)
        {
            errorNumber = string.Empty;
            var addresses = new List<CMS_VendorAddress>();

            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault() as SqlDatabase;
            using (var dbCommandWrapper = db.GetStoredProcCommand("CBPS_GetVendorsAddress"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@VendorID", SqlDbType.NVarChar, vendorId);
                db.AddInParameter(dbCommandWrapper, "@VendorAddressCode", SqlDbType.NVarChar, addressId);
                db.AddOutParameter(dbCommandWrapper, _spStatusParam, DbType.String, 10);
                // Execute SP.
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                {
                    //Get the error number, if error occurred.
                    errorNumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count <= 0) return addresses;
                    var dt1 = ds.Tables[0];
                    addresses = dt1.AsEnumerable().Select(reader => new CMS_VendorAddress
                    {
                        VendorAddressCode = reader.GetString("VendorAddressCode"),
                        VendorID = reader.GetString("VendorID"),
                        VendorAddress1 = reader.GetString("VendorAddress1"),
                        VendorAddress2 = reader.GetString("VendorAddress2"),
                        VendorCountry = reader.GetString("VendorCountry"),
                        VendorPhoneNo = reader.GetString("VendorPhoneNo"),
                        VendorMobileNo = reader.GetString("VendorMobileNo"),
                        VendorEmail = reader.GetString("VendorEmail"),
                        SkypeID = reader.GetString("SkypeID"),
                        ContactPerson = reader.GetString("ContactPerson"),
                        ContactCellNo = reader.GetString("ContactCellNo"),
                        DataSource = reader.GetString("DataSource"),
                        OwnerID = reader.GetString("OwnerID"),
                        Status = reader.GetInt16("Status")
                    }).ToList();
                }
            }
            return addresses;
        }

        public string ManipulateVendor(CMSVendor vendor, string action, out string errorNumber)
        {
            errorNumber = string.Empty;
            try
            {
                var factory = new DatabaseProviderFactory();
                var db = factory.CreateDefault() as SqlDatabase;
                using (var dbCommandWrapper = db.GetStoredProcCommand("CMS_SetVendor"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@VendorID", SqlDbType.NVarChar, vendor.VendorID);
                    db.AddInParameter(dbCommandWrapper, "@VendorCode", SqlDbType.NVarChar, vendor.VendorCode);
                    db.AddInParameter(dbCommandWrapper, "@VendorName", SqlDbType.NVarChar, vendor.VendorName);
                    db.AddInParameter(dbCommandWrapper, "@VendorAddressCode", SqlDbType.NVarChar, vendor.VendorAddressCode);
                    db.AddInParameter(dbCommandWrapper, "@VendorGroup", SqlDbType.NVarChar, vendor.VendorGroup);
                    db.AddInParameter(dbCommandWrapper, "@VendorCategory", SqlDbType.NVarChar, vendor.VendorCategory);
                    db.AddInParameter(dbCommandWrapper, "@VendorType", SqlDbType.NVarChar, vendor.VendorType);
                    db.AddInParameter(dbCommandWrapper, "@IsUser", SqlDbType.Bit, vendor.IsUser);
                    db.AddInParameter(dbCommandWrapper, "@ConcernVendor", SqlDbType.NVarChar, vendor.ConcernVendor);
                    db.AddInParameter(dbCommandWrapper, "@TransBank", SqlDbType.NVarChar, vendor.TransBank);
                    db.AddInParameter(dbCommandWrapper, "@TransAccountName", SqlDbType.NVarChar, vendor.TransAccountName);
                    db.AddInParameter(dbCommandWrapper, "@TransAccountNo", SqlDbType.NVarChar, vendor.TransAccountNo);
                    db.AddInParameter(dbCommandWrapper, "@DataSource", SqlDbType.NVarChar, vendor.DataSource);
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, vendor.OwnerID);
                    db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, vendor.DocCategoryID);
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, vendor.DocTypeID);
                    db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, vendor.DocPropertyID);
                    db.AddInParameter(dbCommandWrapper, "@UserLevel", SqlDbType.Int, vendor.UserLevel);
                    db.AddInParameter(dbCommandWrapper, "@SetBy ", SqlDbType.NVarChar, vendor.SetBy);
                    db.AddInParameter(dbCommandWrapper, "@ModifiedBy", SqlDbType.NVarChar, vendor.ModifiedBy);
                    db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.Int, vendor.Status=1);
                    db.AddInParameter(dbCommandWrapper, "@Action", SqlDbType.VarChar, action);
                    db.AddOutParameter(dbCommandWrapper, _spStatusParam, SqlDbType.VarChar, 10);
                    //db.AddInParameter(dbCommandWrapper, "@ConfColumnIds", SqlDbType.VarChar, vendor.ConfigureColumnIds);

                    // Execute SP.
                    db.ExecuteNonQuery(dbCommandWrapper);
                    // Getting output parameters and setting response details.
                    if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                    {
                        // Get the error number, if error occurred.
                        errorNumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                    }
                }
            }
            catch (Exception ex)
            {
                errorNumber = "E404"; // Log ex.Message  Insert Log Table               
            }
            return errorNumber;
        }

        public string ManipulateVendorAddress(CMS_VendorAddress address, string action, out string errorNumber)
        {
            errorNumber = string.Empty;
            try
            {
                var factory = new DatabaseProviderFactory();
                var db = factory.CreateDefault() as SqlDatabase;
                using (var dbCommandWrapper = db.GetStoredProcCommand("CMS_SetVendorAddress"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@VendorAddressCode", SqlDbType.NVarChar, address.VendorAddressCode);
                    db.AddInParameter(dbCommandWrapper, "@VendorID", SqlDbType.NVarChar, address.VendorID);
                    db.AddInParameter(dbCommandWrapper, "@VendorAddress1", SqlDbType.NVarChar, address.VendorAddress1);
                    db.AddInParameter(dbCommandWrapper, "@VendorAddress2", SqlDbType.NVarChar, address.VendorAddress2);
                    db.AddInParameter(dbCommandWrapper, "@VendorCountry", SqlDbType.NVarChar, address.VendorCountry);
                    db.AddInParameter(dbCommandWrapper, "@VendorPhoneNo", SqlDbType.NVarChar, address.VendorPhoneNo);
                    db.AddInParameter(dbCommandWrapper, "@VendorMobileNo", SqlDbType.NVarChar, address.VendorMobileNo);
                    db.AddInParameter(dbCommandWrapper, "@VendorFax", SqlDbType.NVarChar, address.VendorFax);
                    db.AddInParameter(dbCommandWrapper, "@VendorEmail", SqlDbType.NVarChar, address.VendorEmail);
                    db.AddInParameter(dbCommandWrapper, "@SkypeID", SqlDbType.NVarChar, address.SkypeID);
                    db.AddInParameter(dbCommandWrapper, "@ContactPerson", SqlDbType.NVarChar, address.ContactPerson);
                    db.AddInParameter(dbCommandWrapper, "@ContactCellNo", SqlDbType.NVarChar, address.ContactCellNo);
                    db.AddInParameter(dbCommandWrapper, "@DataSource", SqlDbType.NVarChar, address.DataSource);
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, address.OwnerID);
                    db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, address.DocCategoryID);
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.NVarChar, address.DocTypeID);
                    db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, address.DocPropertyID);
                    db.AddInParameter(dbCommandWrapper, "@UserLevel", SqlDbType.Int, address.UserLevel);
                    db.AddInParameter(dbCommandWrapper, "@SetBy ", SqlDbType.NVarChar, address.SetBy);
                    db.AddInParameter(dbCommandWrapper, "@ModifiedBy", SqlDbType.NVarChar, address.ModifiedBy);
                    db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.Int, address.Status);
                    db.AddInParameter(dbCommandWrapper, "@Action", SqlDbType.VarChar, action);
                    db.AddOutParameter(dbCommandWrapper, _spStatusParam, SqlDbType.VarChar, 10);
                    //db.AddInParameter(dbCommandWrapper, "@ConfColumnIds", SqlDbType.VarChar, vendor.ConfigureColumnIds);

                    // Execute SP.
                    db.ExecuteNonQuery(dbCommandWrapper);
                    // Getting output parameters and setting response details.
                    if (!db.GetParameterValue(dbCommandWrapper, _spStatusParam).IsNullOrZero())
                    {
                        // Get the error number, if error occurred.
                        errorNumber = db.GetParameterValue(dbCommandWrapper, _spStatusParam).PrefixErrorCode();
                    }
                }
            }
            catch (Exception ex)
            {
                errorNumber = "E404"; // Log ex.Message  Insert Log Table               
            }
            return errorNumber;
        }
    }
}
