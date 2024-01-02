using SILDMS.DataAccessInterface.ProcessStage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.DataAccess.ProcessStage
{
    public class ProcessStageDataService : IProcessStageDataService
    {
        private readonly string res_code = "@res_code";
        private readonly string res_message = "@res_message";

        public void UpdateProcessStage(WFM_ProcessStage stage, out string _res_code, out string _res_message)
        {
            try
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory();
                SqlDatabase db = factory.CreateDefault() as SqlDatabase;
                using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_UpdateProcessingStage"))
                {
                    // Set parameters 
                    db.AddInParameter(dbCommandWrapper, "@StageID", SqlDbType.Int, stage.StageID);
                    db.AddInParameter(dbCommandWrapper, "@StageName", SqlDbType.NVarChar, stage.StageName);
                    db.AddInParameter(dbCommandWrapper, "@StageSuspendKey", SqlDbType.NVarChar, stage.StageSuspendKey);
                    db.AddInParameter(dbCommandWrapper, "@DefaultStageSL", SqlDbType.NVarChar, stage.StageSL);
        
                   
                    db.AddInParameter(dbCommandWrapper, "@DefaultHaveMk", SqlDbType.NVarChar, stage.HaveMk);
                    db.AddInParameter(dbCommandWrapper, "@DefaultHaveCk", SqlDbType.Int, stage.HaveCk);
                    db.AddInParameter(dbCommandWrapper, "@DefaultNotifyMk", SqlDbType.VarChar, stage.NotifyMk);
                    db.AddInParameter(dbCommandWrapper, "@DefaultNotifyCk", SqlDbType.Int, stage.NotifyCk);
                    db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, stage.SetBy);
                   // db.AddInParameter(dbCommandWrapper, "@action", SqlDbType.NVarChar, action);                   
                    db.AddOutParameter(dbCommandWrapper, res_code, SqlDbType.VarChar, 10);
                    db.AddOutParameter(dbCommandWrapper, res_message, SqlDbType.VarChar, 100);
                    db.ExecuteNonQuery(dbCommandWrapper);         
                    _res_code = db.GetParameterValue(dbCommandWrapper, res_code).ToString();
                    _res_message = db.GetParameterValue(dbCommandWrapper, res_message).ToString();
                }
            }
            catch (Exception ex)
            {
                _res_code = "0";
                _res_message = "Procedure Exeution Error";
            }
        }
        //public void AddProcessStage(WFM_ProcessStage stage,  out string _res_code, out string _res_message)
        //{
        //    try
        //    {
        //        DatabaseProviderFactory factory = new DatabaseProviderFactory();
        //        SqlDatabase db = factory.CreateDefault() as SqlDatabase;
        //        using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_AddProcessingStage"))
        //        {
        //            // Set parameters 
        //            db.AddInParameter(dbCommandWrapper, "@StageID", SqlDbType.Int, stage.StageID);
        //            db.AddInParameter(dbCommandWrapper, "@StageName", SqlDbType.NVarChar, stage.StageName);
        //            db.AddInParameter(dbCommandWrapper, "@ProcessingGroupID", SqlDbType.NVarChar, stage.ProcessingGroupID);
        //            db.AddInParameter(dbCommandWrapper, "@StageSuspendKey", SqlDbType.NVarChar, stage.StageSuspendKey);
        //            db.AddInParameter(dbCommandWrapper, "@DefaultStageSL", SqlDbType.NVarChar, stage.StageSL);

        //            db.AddInParameter(dbCommandWrapper, "@DefaultAutoIssue", SqlDbType.Int, stage.AutoIssue);
        //            db.AddInParameter(dbCommandWrapper, "@NumberOfCheck", SqlDbType.Int, stage.NumberOfCheck);
        //            db.AddInParameter(dbCommandWrapper, "@DefaultAutoReceive", SqlDbType.NVarChar, stage.AutoReceive);
        //            db.AddInParameter(dbCommandWrapper, "@DefaultHaveMk", SqlDbType.NVarChar, stage.HaveMk);
        //            db.AddInParameter(dbCommandWrapper, "@DefaultHaveCk", SqlDbType.Int, stage.HaveCk);
        //            db.AddInParameter(dbCommandWrapper, "@DefaultNotifyMk", SqlDbType.VarChar, stage.NotifyMk);
        //            db.AddInParameter(dbCommandWrapper, "@DefaultNotifyCk", SqlDbType.Int, stage.NotifyCk);
        //            db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, stage.SetBy);
        //            // db.AddInParameter(dbCommandWrapper, "@action", SqlDbType.NVarChar, action);                   
        //            db.AddOutParameter(dbCommandWrapper, res_code, SqlDbType.VarChar, 10);
        //            db.AddOutParameter(dbCommandWrapper, res_message, SqlDbType.VarChar, 100);
        //            db.ExecuteNonQuery(dbCommandWrapper);
        //            _res_code = db.GetParameterValue(dbCommandWrapper, res_code).ToString();
        //            _res_message = db.GetParameterValue(dbCommandWrapper, res_message).ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _res_code = "0";
        //        _res_message = "Procedure Exeution Error";
        //    }
        //}
        public void AddProcessStages(List<WFM_ProcessStage> stages, string userID, out string res_message)
        {
           
            DataTable stageTable = new DataTable();
            stageTable.Columns.Add("StageName");
            stageTable.Columns.Add("WorkdflowID");
            stageTable.Columns.Add("DocTypeID");
            stageTable.Columns.Add("StageSerial");
            stageTable.Columns.Add("NumberOfCheck");
            stageTable.Columns.Add("DefaultAutoIssue");
            stageTable.Columns.Add("DefaultAutoReceive");
            stageTable.Columns.Add("DefaultHaveMk");
            stageTable.Columns.Add("DefaultHaveCk");
            stageTable.Columns.Add("DefaultifyMk");
            stageTable.Columns.Add("DefaultifyCk");
            stageTable.Columns.Add("SetBy");
            if (stages != null)
            {

                foreach (var item in stages)
                {
                    DataRow objDataRow = stageTable.NewRow();



                    objDataRow[0] = item.StageName;
                    objDataRow[1] = item.WorkflowID;
                    objDataRow[2] = item.DocTypeID;
                    objDataRow[3] = item.StageSL;
                    objDataRow[4] = 0;
                    objDataRow[5] = 0;
                    objDataRow[6] = 0;
                    objDataRow[7] = item.HaveMk;
                    objDataRow[8] = item.HaveCk;
                    objDataRow[9] = item.NotifyMk;
                    objDataRow[10] = item.NotifyCk;
                    objDataRow[11] = userID;
                    stageTable.Rows.Add(objDataRow);
                }

            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;

            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_AddProcessStages"))
            {
                db.AddInParameter(dbCommandWrapper, "@stageTable", SqlDbType.Structured, stageTable);
                db.AddInParameter(dbCommandWrapper, "@DoctypeId", SqlDbType.NVarChar, stages[0].DocTypeID);
                //db.AddInParameter(dbCommandWrapper, "@StageName", SqlDbType.NVarChar, stage.StageName);
                //db.AddInParameter(dbCommandWrapper, "@ProcessingGroupID", SqlDbType.NVarChar, stage.ProcessingGroupID);
                //db.AddInParameter(dbCommandWrapper, "@StageSuspendKey", SqlDbType.NVarChar, stage.StageSuspendKey);
                //db.AddInParameter(dbCommandWrapper, "@DefaultStageSL", SqlDbType.NVarChar, stage.StageSL);
                //db.AddInParameter(dbCommandWrapper, "@DefaultPosition", SqlDbType.NVarChar, stage.StagePosition);
                //db.AddInParameter(dbCommandWrapper, "@DefaultNextStage", SqlDbType.NVarChar, stage.NextStege);
                //db.AddInParameter(dbCommandWrapper, "@DefaultAutoIssue", SqlDbType.Int, stage.AutoIssue);
                //db.AddInParameter(dbCommandWrapper, "@DefaultAutoReceive", SqlDbType.NVarChar, stage.AutoReceive);
                //db.AddInParameter(dbCommandWrapper, "@DefaultHaveMk", SqlDbType.NVarChar, stage.HaveMk);
                //db.AddInParameter(dbCommandWrapper, "@DefaultHaveCk", SqlDbType.Int, stage.HaveCk);
                //db.AddInParameter(dbCommandWrapper, "@DefaultNotifyMk", SqlDbType.VarChar, stage.NotifyMk);
                //db.AddInParameter(dbCommandWrapper, "@DefaultNotifyCk", SqlDbType.Int, stage.NotifyCk);
                //db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, stage.SetBy);
                try
                {
                    var ds = db.ExecuteDataSet(dbCommandWrapper);
                    res_message = "S201";

                }
                catch (Exception e)
                {

                    res_message = "E404";
                }

            }
        }

        public List<WFM_ProcessStage> GetALLProcessStage(string userID)
        {
 
            List<WFM_ProcessStage> stageList = new List<WFM_ProcessStage>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetAllStage"))
            {
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, userID);
                var ds = db.ExecuteDataSet(dbCommandWrapper);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    stageList = dt1.AsEnumerable().Select(reader => new WFM_ProcessStage
                    {
                        StageID=reader.GetInt32("StageID"),
                        StageSL=reader.GetInt32("StageSerial"),
                        StageName =reader.GetString("StageName"),
                        StagePosition=reader.GetString("DefaultPosition")
                    }).ToList();
                }
               
            }
            return stageList;
        }
        public List<WFM_ProcessStage> GetProcessStagesByWorkflow(string DocTypeID, string userID, out string _outStatus)
        {

            List<WFM_ProcessStage> stageList = new List<WFM_ProcessStage>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetProcessStagesByWorkflow"))
            {
                db.AddInParameter(dbCommandWrapper, "@UserID", SqlDbType.VarChar, userID);
                db.AddInParameter(dbCommandWrapper, "@DocTypeID", SqlDbType.VarChar, DocTypeID);

                try
                {

                    var ds = db.ExecuteDataSet(dbCommandWrapper);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        stageList = dt1.AsEnumerable().Select(reader => new WFM_ProcessStage
                        {
                            StageID = reader.GetInt32("StageID"),
                            StageSL = reader.GetInt32("StageSerial"),
                            PositionX = reader.GetInt32("PositionX"),
                            PositionY = reader.GetInt32("PositionY"),
                            DocTypeID = reader.GetString("DocTypeID"),
                            WorkflowID = reader.GetInt32("WorkdflowID"),
                            NumberOFStage = reader.GetInt32("NumberOFStage"),
                            
                            StageName = reader.GetString("StageName"),
                            

                            Status = reader.GetInt32("Status"),
                        }).ToList();

                        _outStatus = "S201";
                    }
                    else {
                        
                        _outStatus = "S200";
                    }
                   
                }

                catch(Exception e) {

                    _outStatus = "E404";
                }
            }
            return stageList;
        }

        public WFM_Workflow GetWorkflow(string DocTypeID, out string _outStatus)
        {
            WFM_Workflow workflow = new WFM_Workflow();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            var query = new StringBuilder();
            query.Append("select * from WFM_Workflow where DocTypeID='" + DocTypeID + "'");
            _outStatus = "E404";
            if (db != null)
            {
                try
                {
                    var ds = db.ExecuteDataSet(CommandType.Text, query.ToString());

                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt1 = ds.Tables[0];

                            var dr = dt1.Rows[0];


                            workflow = new WFM_Workflow()
                            {

                                NumberOfStage = dr.GetInt32("NumberOfStage"),
                                WorkflowID = dr.GetInt32("WorkflowID"),
                                WorkflowName = dr.GetString("WorkflowName"),
                                DocTypeID = dr.GetString("DocTypeID"),
                                Status = dr.GetInt32("Status")


                            };

                        }
                        _outStatus = "S201";
                    }
                    else
                    {
                        _outStatus = "S200";
                    }
                }
                catch (Exception e)
                {

                    _outStatus = "E404";
                }
             
            }

            return workflow;
        }

        public WFM_ProcessStage GetProcessStagesByStageID(int StageID, out string res_message)
        {
            WFM_ProcessStage stage = new WFM_ProcessStage();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            var query = new StringBuilder();
            query.Append("select * from WFM_ProcessingStage where StageID='" + StageID + "'");
            res_message = "E404";
            if (db != null)
            {
                try
                {
                    var ds = db.ExecuteDataSet(CommandType.Text, query.ToString());

                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt1 = ds.Tables[0];

                            var dr = dt1.Rows[0];


                            stage = new WFM_ProcessStage()
                            {

                               
                                WorkflowID = dr.GetInt32("WorkdflowID"),
                                StageName = dr.GetString("StageName"),
                                DocTypeID = dr.GetString("DocTypeID"),
                           
                                HaveMk = dr.GetBoolean("DefaultHaveMk"),
                                HaveCk = dr.GetBoolean("DefaultHaveCk"),
                                NotifyMk = dr.GetBoolean("DefaultNotifyMk"),
                                NotifyCk = dr.GetBoolean("DefaultNotifyCk"),
                              
                                Status = dr.GetInt32("Status")


                            };

                        }
                        res_message = "S201";
                    }
                    else
                    {
                        res_message = "S200";
                    }
                }
                catch (Exception e)
                {

                    res_message = "E404";
                }

            }
            return stage;



        }


        public List<WFM_ProcessStage> GetChildProcessStagesByStageID(int StageID, out string res_message)
        {
            List<WFM_ProcessStage> ChildStages = new List<WFM_ProcessStage>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.CreateDefault() as SqlDatabase;
            using (DbCommand dbCommandWrapper = db.GetStoredProcCommand("WFM_GetProcessStagesByWorkflow"))
            {
                db.AddInParameter(dbCommandWrapper, "@StageID", SqlDbType.VarChar, StageID);
               

                try
                {

                    var ds = db.ExecuteDataSet(dbCommandWrapper);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        ChildStages = dt1.AsEnumerable().Select(reader => new WFM_ProcessStage
                        {
                            StageID = reader.GetInt32("StageID"),
                            StageSL = reader.GetInt32("StageSerial"),
                            PositionX = reader.GetInt32("PositionX"),
                            PositionY = reader.GetInt32("PositionY"),
                            DocTypeID = reader.GetString("DocTypeID"),
                            WorkflowID = reader.GetInt32("WorkdflowID"),
                            NumberOFStage = reader.GetInt32("NumberOFStage"),

                            StageName = reader.GetString("StageName"),


                            Status = reader.GetInt32("Status"),
                        }).ToList();

                        res_message = "S201";
                    }
                    else
                    {

                        res_message = "S200";
                    }

                }

                catch (Exception e)
                {

                    res_message = "E404";
                }
            }
            return ChildStages;



        }
    }
 }
