using Quartz;
using SILDMS.Model.DocScanningModule;
using SILDMS.Service.DocDestroyPolicy;
using SILDMS.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.IO;

namespace SILDMS.Web.UI.ScheduledTasks
{
    public class EmailNotificationJob4DU : IJob
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["AuthContext"].ConnectionString;
        private readonly string spStatusParam = "@p_Status";
        public string HostAdd { get; set; }
        public string FromEmailid { get; set; }
        public string ToEmail { get; set; }
        public string Password { get; set; }
        public void Execute(IJobExecutionContext context)
        {
            List<EmailNotificationJob4DUModel> lstDocSearch = new List<EmailNotificationJob4DUModel>();
            List<string> DeletedIds = new List<string>();
            List<string> DeletedIdsVersion = new List<string>();
            DataSet ds = new DataSet();
            DataSet dsVersion = new DataSet();
            DataSet dsAlert = new DataSet();

            using (SqlConnection connection = new SqlConnection(connStr))
            {
                try
                {
                    connection.Open();
                  

                    SqlCommand cmd = new SqlCommand("GetAllEmailNotification", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter outputStatusParam = new SqlParameter(spStatusParam, DbType.String)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputStatusParam);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    da.Dispose();
                    connection.Close();

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        lstDocSearch = dt1.AsEnumerable().Select(reader => new EmailNotificationJob4DUModel
                        {
                            OwnerID = reader["OwnerID"].ToString(),
                            DocCategoryName = reader["DocCategoryName"].ToString(),
                            DocTypeName = reader["DocTypeName"].ToString(),
                            DocPropertyName = reader["DocPropertyName"].ToString(),
                            IdentificationAttribute = reader["IdentificationAttribute"].ToString(),
                            MetaValue = reader["MetaValue"].ToString(),
                            Emails = reader["Emails"].ToString(),
                            EmailSubject = reader["EmailSubject"].ToString(),
                            EmailBody = reader["EmailBody"].ToString(),
                            NotificationDate = reader["NotificationDate"].ToString(),
                            DocumentExpDate = reader["DocumentExpDate"].ToString(),
                            DocumentID = reader["DocumentID"].ToString(),
                            MessageLogID = reader["MessageLogID"].ToString()
                        }).ToList();
                    }


                    List<EmailNotificationJob4DUModel> tempLstDocSearch;
                    List<string> owners = lstDocSearch.Select(o => o.OwnerID).Distinct().ToList();
                    for (int i = 0; i < owners.Count; i++)
                    {
                        tempLstDocSearch = new List<EmailNotificationJob4DUModel>();
                        tempLstDocSearch = (from t in lstDocSearch where t.OwnerID == owners[i] select t).ToList();

                        if (tempLstDocSearch.Count > 0)
                        {
                            SendMailRequest sendOb = new SendMailRequest();
                            sendOb.recipient = tempLstDocSearch[0].Emails;
                            sendOb.subject = tempLstDocSearch[0].EmailSubject;
                            string emailBody = "";
                            string messageLogIds = "";
                            foreach (var item in tempLstDocSearch)
                            {
                                if (emailBody != "")
                                {
                                    emailBody += item.IdentificationAttribute + " : " + item.MetaValue +" ( Exp Date: "+Convert.ToDateTime(item.DocumentExpDate).ToShortDateString()+ " ) </br>";
                                }
                                else {
                                    emailBody = item.EmailBody + " </br>" + item.IdentificationAttribute + " : " + item.MetaValue + " ( Exp Date:" + Convert.ToDateTime(item.DocumentExpDate).ToShortDateString() + " )</br>"; ;
                                }
                                messageLogIds += item.MessageLogID.ToString()+"|";
                            }
                            sendOb.body = emailBody;
                            bool status = SendEmail(sendOb);
                            if(status)
                            {
                                connection.Open();
                                #region Original Document Delete
                                SqlCommand coms = new SqlCommand("UpdateMessageLogStatus", connection);
                                coms.CommandType = CommandType.StoredProcedure;
                                coms.Parameters.AddWithValue("@LogIds", messageLogIds);
                                coms.ExecuteNonQuery();
                                connection.Close();
                            }
                        }

                    }

                 

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogMaintain("Mail Send Failed");
                }
            }
        }

        public bool SendEmail(SendMailRequest mailModel)
        {

            // Send the email
            System.Net.Mail.MailMessage msg = new MailMessage();

            // Separate the recipient array
            string[] emailAddress = mailModel.recipient.Split(',');

            foreach (string currentEmailAddress in emailAddress)
            {
                msg.To.Add(new MailAddress(currentEmailAddress.Trim()));
            }


            // Separate the cc array , if not null
            string[] ccAddress = null;

            if (!String.IsNullOrWhiteSpace(mailModel.cc))
            {
                ccAddress = mailModel.cc.Split(',');
                foreach (string currentCCAddress in ccAddress)
                {
                    msg.CC.Add(new MailAddress(currentCCAddress.Trim()));
                }
            }


            // Include the reply to if not null
            if (!String.IsNullOrWhiteSpace(mailModel.replyto))
            {
                msg.ReplyToList.Add(new MailAddress(mailModel.replyto));
            }


            string sendFromEmail = Properties.Settings.Default["SendFromEmail"].ToString();
            string sendFromName = Properties.Settings.Default["SendFromName"].ToString();
            string sendFromPassword = Properties.Settings.Default["SendFromPassword"].ToString();
            string Host = Properties.Settings.Default["EmailHost"].ToString();
            int Port = Convert.ToInt32(Properties.Settings.Default["EmailPort"]);

            msg.From = new MailAddress(sendFromEmail, sendFromName);
            msg.Subject = mailModel.subject;
            msg.Body = mailModel.body;
            msg.IsBodyHtml = true;


            SmtpClient client = new SmtpClient();
            client.Host = Host;
            client.Port = Port;
            client.EnableSsl = true;
            client.UseDefaultCredentials = true;
            NetworkCredential cred = new System.Net.NetworkCredential(sendFromEmail, sendFromPassword);
            client.Credentials = cred;
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
            try
            {
                client.Send(msg);
                msg.Dispose();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public void LogMaintain(string message)
        {
            StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\log.txt", true);
            try
            {
                string logMessage = message + DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString();
                sw.WriteLine(logMessage);
            }
            catch (Exception ex)
            {
                LogMaintain(ex.Message.ToString());
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                    sw.Close();
                }
            }

        }

     
    }
}