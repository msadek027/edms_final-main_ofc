using System;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Linq;
using System.Collections.Generic;

namespace SILDMS.Utillity
{
    public static class DataHelper
    {
        public static string GenerateQueryForDocumentPropertyAttributeData(string ownerID, string documentCategoryID, string documentTypeID, string documentPropertyID)
        {
            string query = "";
            query += @"SELECT DISTINCT(IdentificationAttribute) FROM DSM_DocPropIdentify WHERE ";

            if (ownerID != "" && documentCategoryID != "" && documentTypeID != "" && documentPropertyID != "")
            {
                query += "OwnerID='" + ownerID + "' AND DocCategoryID='" + documentCategoryID + "' AND DocTypeID='" + documentTypeID + "' AND DocPropertyID='" + documentPropertyID + "'";
            }
            else if (ownerID != "" && documentCategoryID != "" && documentTypeID != "")
            {
                query += "OwnerID='" + ownerID + "' AND DocCategoryID='" + documentCategoryID + "' AND DocTypeID='" + documentTypeID + "'";
            }
            else if (ownerID != "" && documentCategoryID != "")
            {
                query += "OwnerID='" + ownerID + "' AND DocCategoryID='" + documentCategoryID + "'";
            }
            else
            {
                query += "OwnerID='" + ownerID + "'";
            }

            return query;
        }

        public static DataTable GetData(string query)
        {
            DataTable dt = new DataTable();

            if (query != "")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                using (DbCommand dbCommandWrapper = db.GetSqlStringCommand(query))
                {
                    DataSet ds = db.ExecuteDataSet(dbCommandWrapper);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dt = ds.Tables[0];
                    }
                }
            }

            return dt;
        }

        public static int DataCount(DataTable dt)
        {
            return dt.Rows.Count;
        }

    }
}
