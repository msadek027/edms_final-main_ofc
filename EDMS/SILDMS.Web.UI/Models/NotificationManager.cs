using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TableDependency;
using TableDependency.Enums;
using TableDependency.EventArgs;
using TableDependency.SqlClient;

namespace SILDMS.Web.UI.Models
{
    public class NotificationManager
    {
        private readonly static Lazy<NotificationManager> _instance = new Lazy<NotificationManager>(
            () => new NotificationManager(GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients));

        private static SqlTableDependency<WFM_Object> _tableDependency;

        private Dictionary<string, string> connections = new Dictionary<string, string>();

        private NotificationManager(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            var mapper = new ModelToTableMapper<WFM_Object>();
            mapper.AddMapping(s => s.ObjectID, "ObjectID");
            //mapper.AddMapping(s => s.StageMapID, "StageMapID");
            //mapper.AddMapping(s => s.MkStatus, "MkStatus");
            //mapper.AddMapping(s => s.CkStatus, "CkStatus");
            mapper.AddMapping(s => s.PassedKey, "PassedKey");

            _tableDependency = new SqlTableDependency<WFM_Object>(ConfigurationManager.ConnectionStrings["AuthContext"].ConnectionString, tableName:"WFM_Objects", mapper: mapper);

            _tableDependency.OnChanged += SqlTableDependency_Changed;
            _tableDependency.OnError += SqlTableDependency_OnError;
            _tableDependency.Start();
        }

        public static NotificationManager Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public void AddConnection(string key,string value) {
            connections[key] = value;
        }

        public void RemoveConnection(string key) {
            connections.Remove(key);
        }
        void SqlTableDependency_OnError(object sender, ErrorEventArgs e)
        {
            throw e.Error;
        }
        void SqlTableDependency_Changed(object sender, RecordChangedEventArgs<WFM_Object> e)
        {
            if (e.ChangeType != ChangeType.None && !e.Entity.PassedKey)
            {
                BroadcastDSM_Object(e.Entity);
            }
        }

        private void BroadcastDSM_Object(WFM_Object mobject)
        {
            //code had to be commented because of error
            string conStr = ConfigurationManager.ConnectionStrings["AuthContext"].ConnectionString;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            string sqlCommand = "";
            sqlCommand = @"SELECT UserID FROM WFM_UserStagePermission where StageMapID=@StageMapID AND MkPermission = 1 

                            SELECT ps.StageName FROM WFM_StageMap sm 
                            INNER JOIN 
                            WFM_ProcessingStage ps ON sm.StageID=ps.StageID
                            WHERE sm.StageMapID = @StageMapID";


            string MkOrCk = "";
            string StageName = "";
            MkOrCk += "to update";
            List<string> userList = new List<string>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);
                cmd.Parameters.AddWithValue("@StageMapID", 268);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var UserTable = ds.Tables[0];
                var StageTable = ds.Tables[1];


                if (UserTable != null)
                {
                    if (UserTable.Rows.Count > 0)
                    {
                        userList = UserTable.AsEnumerable().Select(m => m.Field<string>("UserID")).ToList();
                    }
                }

                if (StageTable != null)
                {
                    if (StageTable.Rows.Count > 0)
                    {
                        StageName = StageTable.Rows[0][0].ToString();
                    }
                }


            }
            //if (!mobject.MkStatus)
            //{
            //    sqlCommand = @"SELECT UserID FROM WFM_UserStagePermission where StageMapID=@StageMapID AND MkPermission = 1 

            //                SELECT ps.StageName FROM WFM_StageMap sm 
            //                INNER JOIN 
            //                WFM_ProcessingStage ps ON sm.StageID=ps.StageID
            //                WHERE sm.StageMapID = @StageMapID";

            //    MkOrCk += "to update";

            //}
            //else if (!mobject.CkStatus)
            //{
            //    sqlCommand = @"SELECT UserID FROM WFM_UserStagePermission where StageMapID=@StageMapID AND CkPermission = 1 

            //                SELECT ps.StageName FROM WFM_StageMap sm 
            //                INNER JOIN 
            //                WFM_ProcessingStage ps ON sm.StageID=ps.StageID
            //                WHERE sm.StageMapID = @StageMapID";

            //    MkOrCk += "for approval";
            //}

            //if (sqlCommand == "")
            //{
            //    return;
            //}

            //string StageName = "";

            //List<string> userList = new List<string>();

            //using (SqlConnection con = new SqlConnection(conStr))
            //{
            //    SqlCommand cmd = new SqlCommand(sqlCommand, con);
            //    cmd.Parameters.AddWithValue("@StageMapID", mobject.StageMapID);
            //    da = new SqlDataAdapter(cmd);
            //    da.Fill(ds);
            //    var UserTable = ds.Tables[0];
            //    var StageTable = ds.Tables[1];


            //    if (UserTable != null)
            //    {
            //        if (UserTable.Rows.Count > 0)
            //        {
            //            userList = UserTable.AsEnumerable().Select(m => m.Field<string>("UserID")).ToList();
            //        }
            //    }

            //    if (StageTable != null)
            //    {
            //        if (StageTable.Rows.Count > 0)
            //        {
            //            StageName = StageTable.Rows[0][0].ToString();
            //        }
            //    }


            //}

            //if (userList.Count < 1)
            //{
            //    return;
            //}
            //if (StageName == "")
            //{
            //    return;
            //}

            //Clients.Clients(connections.Where(z => userList.Contains(z.Key)).Select(a => a.Value).ToList()).notify(new Notification
            //{
            //    ObjectID = mobject.ObjectID,
            //    StageMapID = mobject.StageMapID,
            //    Message = "New item at " + StageName + " " + MkOrCk + ".",
            //    IsNew = true,
            //    NotifyAt = DateTime.Now
            //});

            Clients.Clients(connections.Where(z => userList.Contains(z.Key)).Select(a => a.Value).ToList()).notify(new Notification
            {
                ObjectID = mobject.ObjectID,
                StageMapID = 268,
                Message = "New item at " + StageName + " " + MkOrCk + ".",
                IsNew = true,
                NotifyAt = DateTime.Now
            });
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _tableDependency.Stop();
                }

                disposedValue = true;
            }
        }

        ~NotificationManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}