using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.DocumentUpdate;
using SILDMS.Model.DocScanningModule;
using SILDMS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccess.DocumentUpdate
{
    public class DocumentUpdateDataService : IDocumentUpdateDataService
    {
        #region Fields
        private readonly string spErrorParam = "@p_Error";
        private readonly string spStatusParam = "@p_Status";
        #endregion
        public List<OriginalDocMeta> GetOriginalDocMeta(string _DocumentId, string _DocDistributionID, out string errorNumber)
        {
            throw new NotImplementedException();
        }


        public string UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo, string UserID, out string errorNumber)
        {
            errorNumber = String.Empty;
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                DataTable docMetaDataTable = new DataTable();
                //docMetaDataTable.Columns.Add("ID");
                docMetaDataTable.Columns.Add("DocMetaID");
                docMetaDataTable.Columns.Add("MetaValue");

                foreach (var item in _modelDocumentsInfo.DocMetaValues)
                {
                    DataRow objDataRow = docMetaDataTable.NewRow();
                    //objDataRow[0] = 0;
                    objDataRow[0] = item.DocMetaID;
                    objDataRow[1] = item.MetaValue;

                    docMetaDataTable.Rows.Add(objDataRow);
                }

                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("UpdateDocMetaInfo"))
                {
                    db.AddInParameter(dbCommandWrapper, "@Doc_Meta", SqlDbType.Structured, docMetaDataTable);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, UserID);

                    //db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.NVarChar, _modelDocumentsInfo.Status);
                    //db.AddInParameter(dbCommandWrapper, "@DocumentID", SqlDbType.NVarChar, _modelDocumentsInfo.DocumentID);

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


        public void UpdateDocMailNotifyAndExpDate(string UserID, string DocID, string NotifyDate, string ExpDate, out string outStatus)
        {
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;
                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("UpdateNotifyDateandExpDateForDocument"))
                {
                    db.AddInParameter(dbCommandWrapper, "@DocID", SqlDbType.NVarChar, DocID);
                    db.AddInParameter(dbCommandWrapper, "@NotifyDate", SqlDbType.NVarChar, NotifyDate);
                    db.AddInParameter(dbCommandWrapper, "@ExpDate", SqlDbType.NVarChar, ExpDate);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.NVarChar, UserID);

                    db.ExecuteNonQuery(dbCommandWrapper);
                    outStatus = "success";


                }
            }
            catch (Exception ex)
            {
                outStatus = "Failed";
            }
        }
    }
}
