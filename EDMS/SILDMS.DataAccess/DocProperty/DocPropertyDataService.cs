using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.OwnerProperty;
using SILDMS.Model.DocScanningModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccess.OwnerProperty
{
    public class DocPropertyDataService : IDocPropertyDataService
    {
        private readonly string spStatusParam = "@p_Status";
        public List<DSM_DocProperty> GetDocProperty(string DocPropertyId, string userID, out string errorNumber)
        {
            errorNumber = string.Empty;
            List<DSM_DocProperty> docPropertyList = new List<DSM_DocProperty>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetDocProperty"))
            {
                // Set parameters 
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, userID);
                db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.VarChar, DocPropertyId);
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
                        docPropertyList = dt1.AsEnumerable().Select(reader => new DSM_DocProperty
                        {
                            DocPropertyID = reader.GetString("DocPropertyID"),
                            DocCategoryID = reader.GetString("DocCategoryID"),
                            OwnerLevelID = reader.GetString("OwnerLevelID"),
                            OwnerID = reader.GetString("OwnerID"),
                            DocTypeID = reader.GetString("DocTypeID"),
                            DocPropertySL = reader.GetString("DocPropertySL"),
                            UDDocPropertyCode = reader.GetString("UDDocPropertyCode"),
                            DocPropertyName = reader.GetString("DocPropertyName"),
                            DocClassification = reader.GetString("DocClassification"),
                            PreservationPolicy = reader.GetString("PreservationPolicy"),
                            PhysicalLocation = reader.GetString("PhysicalLocation"),
                            SerialNo = reader.GetInt32("SerialNo"),
                            InformationValidityPeriod = reader["InformationValidityPeriod"] == System.DBNull.Value ? default(int) : reader.GetInt32("InformationValidityPeriod"),
                            Remarks = reader.GetString("Remarks"),
                            SetOn = reader.GetString("SetOn"),
                            SetBy = reader.GetString("SetBy"),
                            ModifiedOn = reader.GetString("ModifiedOn"),
                            ModifiedBy = reader.GetString("ModifiedBy"),
                            email = reader.GetBoolean("email"),
                            emailSub = reader.GetString("EmailSubject"),
                            emailBody = reader.GetString("EmailBody"),
                            sms = reader.GetBoolean("sms"),
                            smsBody = reader.GetString("SMSText"),
                            obsulate = reader.GetBoolean("obsulate"),
                            emailId = reader.GetString("Emails"),
                            smsNum = reader.GetString("Phones"),
                            Status = reader.GetInt32("Status")
                        }).ToList();
                    }
                }
            }

            return docPropertyList;
        }

        public string AddDocProperty( DSM_DocProperty docProperty, string action, out string errorNumber)
        {
            errorNumber = String.Empty;
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("SetDocProperty"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, docProperty.DocPropertyID);
                    db.AddInParameter(dbCommandWrapper, "@DocCategoryID", SqlDbType.NVarChar, docProperty.DocCategoryID.Trim());
                    db.AddInParameter(dbCommandWrapper, "@OwnerLevelID", SqlDbType.NVarChar, docProperty.OwnerLevelID.Trim());
                    db.AddInParameter(dbCommandWrapper, "@OwnerID", SqlDbType.NVarChar, docProperty.OwnerID.Trim());
                    db.AddInParameter(dbCommandWrapper, "@DocTypeID ", SqlDbType.NVarChar, docProperty.DocTypeID.Trim());
                    db.AddInParameter(dbCommandWrapper, "@DocPropertySL", SqlDbType.NVarChar, docProperty.DocPropertySL != null ? docProperty.DocPropertySL.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@UDDocPropertyCode", SqlDbType.NVarChar, docProperty.UDDocPropertyCode != null ? docProperty.UDDocPropertyCode.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@DocPropertyName", SqlDbType.NVarChar, docProperty.DocPropertyName != null ? docProperty.DocPropertyName.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@DocClassification", SqlDbType.NVarChar, docProperty.DocClassification != null ? docProperty.DocClassification.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@PreservationPolicy ", SqlDbType.NVarChar, docProperty.PreservationPolicy);
                    db.AddInParameter(dbCommandWrapper, "@PhysicalLocation", SqlDbType.NVarChar, docProperty.PhysicalLocation != null ? docProperty.PhysicalLocation.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@Remarks", SqlDbType.NVarChar, docProperty.Remarks != null ? docProperty.Remarks.Trim() : "");
                    db.AddInParameter(dbCommandWrapper, "@SetBy ", SqlDbType.NVarChar, docProperty.SetBy.Trim());
                    db.AddInParameter(dbCommandWrapper, "@ModifiedBy", SqlDbType.NVarChar, docProperty.ModifiedBy.Trim());
                    db.AddInParameter(dbCommandWrapper, "@SerialNo", SqlDbType.Int, docProperty.SerialNo);
                    db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.Int, docProperty.Status);
                    db.AddInParameter(dbCommandWrapper, "@InformationValidityPeriod", SqlDbType.Int, docProperty.InformationValidityPeriod != null ? docProperty.InformationValidityPeriod.Value : (int?) null);
                    db.AddInParameter(dbCommandWrapper, "@email", SqlDbType.Int, docProperty.email ? 1 : 0);
                    db.AddInParameter(dbCommandWrapper, "@sms", SqlDbType.Int, docProperty.sms ? 1 : 0);
                    db.AddInParameter(dbCommandWrapper, "@obsulate", SqlDbType.Int, docProperty.obsulate ? 1 : 0);

                    db.AddInParameter(dbCommandWrapper, "@emailId", SqlDbType.NVarChar, docProperty.emailId);
                    db.AddInParameter(dbCommandWrapper, "@emailSub", SqlDbType.NVarChar, docProperty.emailSub);
                    db.AddInParameter(dbCommandWrapper, "@emailBody", SqlDbType.NVarChar, docProperty.emailBody);
                    db.AddInParameter(dbCommandWrapper, "@smsNum", SqlDbType.NVarChar, docProperty.smsNum);
                    db.AddInParameter(dbCommandWrapper, "@smsBody", SqlDbType.NVarChar, docProperty.smsBody);
                    db.AddInParameter(dbCommandWrapper, "@Action", SqlDbType.VarChar, action);
                    db.AddInParameter(dbCommandWrapper, "@ConfColumnIds", SqlDbType.VarChar, docProperty.ConfigureColumnIds);
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


        public DSM_DocProperty GetInfoValidPeriod(string UserID, string documentID, out string errorNumber)
        {
            errorNumber = string.Empty;
            DSM_DocProperty docProperty = new DSM_DocProperty();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            string query = "";
            query = "select dateadd(HOUR,(select InformationValidityPeriod from DSM_DocProperty where DocPropertyID in (select DocPropertyID from DSM_Documents where DocumentID = '" + documentID + "')), getdate()) as RequiredDate";
            using (DbCommand dbCommandWrapper = db.GetSqlStringCommand(query))
            {
                
                DataSet ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];

                        docProperty = dt1.AsEnumerable().Select(reader => new DSM_DocProperty
                        {
                            InfoValidOn = reader.GetString("RequiredDate"),
                        }).FirstOrDefault();
                    }
                }
            }

            return docProperty;
        }
        public bool GetMailNotificationStatusForindividualDocProperty(string DocPropertyId)
        {
            bool status = false;
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;
                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("GetMailStatusForDocProperty"))
                {
                    db.AddInParameter(dbCommandWrapper, "@DocPropertyID", SqlDbType.NVarChar, DocPropertyId);


                    IDataReader dr = db.ExecuteReader(dbCommandWrapper);
                    while (dr.Read())
                    {
                        status = dr.GetBoolean("Email");
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return status;
        }
    }
}
