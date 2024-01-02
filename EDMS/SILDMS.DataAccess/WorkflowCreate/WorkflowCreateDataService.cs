using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SILDMS.DataAccessInterface.WorkflowCreate;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.DataAccess.WorkflowCreate
{
    public class WorkflowCreateDataService : IWorkflowCreateDataService
    {
        private readonly string spStatusParam = "@p_Status";
        private readonly string res_code = "@res_code";
        private readonly string res_message = "@res_message";

        public void AddWorkflow(WFM_Workflow workflow, string UserID,out string _outStatus)
        {
           
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_AddWorkflow"))
            {
                db.AddInParameter(dbCommandWrapper, "@WorkflowName", SqlDbType.NVarChar, workflow.WorkflowName);
                db.AddInParameter(dbCommandWrapper, "@WorkflowDescription", SqlDbType.NVarChar, workflow.WorkflowDescription);
                db.AddInParameter(dbCommandWrapper, "@NumberOfStage", SqlDbType.Int, workflow.NumberOfStage);
                db.AddInParameter(dbCommandWrapper, "@docTypeID", SqlDbType.NVarChar, workflow.DocTypeID);
                db.AddInParameter(dbCommandWrapper, "@SetBy", SqlDbType.NVarChar, UserID);
                try
                {
                    var ds = db.ExecuteDataSet(dbCommandWrapper);
                    _outStatus = "S201";

                }
                catch (Exception e)
                {

                    _outStatus = "E404";
                }

            }

           
        }
        public void EditWorkflow(WFM_Workflow workflow, out string _outStatus)
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_EditWorkflow"))
            {
                db.AddInParameter(dbCommandWrapper, "@WorkflowName", SqlDbType.NVarChar, workflow.WorkflowName);
                db.AddInParameter(dbCommandWrapper, "@WorkflowDescription", SqlDbType.NVarChar, workflow.WorkflowDescription);
                db.AddInParameter(dbCommandWrapper, "@NumberOfStage", SqlDbType.Int, workflow.NumberOfStage);
                db.AddInParameter(dbCommandWrapper, "@WorkflowID", SqlDbType.Int, workflow.WorkflowID);
                db.AddInParameter(dbCommandWrapper, "@ModifiedBy", SqlDbType.NVarChar, workflow.ModifiedBy);
                try
                {
                    var ds = db.ExecuteDataSet(dbCommandWrapper);
                    _outStatus = "S201";

                }
                catch (Exception e)
                {

                    _outStatus = "E404";
                }

            }


        }

        public void WorkflowStatusChange(int WorkflowID, int Status, string UserID, out string _outStatus)
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_WorkflowStatusChange"))
            {
          
                db.AddInParameter(dbCommandWrapper, "@WorkflowID", SqlDbType.Int, WorkflowID);
                db.AddInParameter(dbCommandWrapper, "@Status", SqlDbType.Int, Status);
                db.AddInParameter(dbCommandWrapper, "@ModifiedBy", SqlDbType.NVarChar, UserID);
                try
                {
                    var ds = db.ExecuteDataSet(dbCommandWrapper);
                    _outStatus = "S201";

                }
                catch (Exception e)
                {

                    _outStatus = "E404";
                }

            }


        }
        public List<WFM_Workflow> getListofWorkflow()
        {

            List<WFM_Workflow> workflows = new List<WFM_Workflow>();
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;

                var query = new StringBuilder();

                query.Append("select * from WFM_Workflow");

                if (db != null)
                {
                    var ds = db.ExecuteDataSet(CommandType.Text, query.ToString());

                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt1 = ds.Tables[0];

                            workflows = dt1.AsEnumerable().Select(reader => new WFM_Workflow
                            {
                                WorkflowID= reader.GetInt32("WorkflowID"),
                                WorkflowName = reader.GetString("WorkflowName"),
                                WorkflowDescription = reader.GetString("WorkflowDescription"),
                                NumberOfStage = reader.GetInt32("NumberOfStage"),
                                DocTypeID = reader.GetString("DocTypeID"),
                                
                                Status = reader.GetInt32("Status")


                            }).ToList();
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                
            }


            return workflows;

        }
    }
}
