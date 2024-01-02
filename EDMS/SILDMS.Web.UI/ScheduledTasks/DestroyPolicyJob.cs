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
using System.Net;
using System.Net.Mail;
using System.IO;

namespace SILDMS.Web.UI.ScheduledTasks
{
    public class DestroyPolicyJob : IJob
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["AuthContext"].ConnectionString;
        private readonly string spStatusParam = "@p_Status";
        public string HostAdd { get; set; }
        public string FromEmailid { get; set; }
        public string ToEmail { get; set; }
        public string Password { get; set; }
        public void Execute(IJobExecutionContext context)
        {
            List<DocSearch> lstDocSearch = new List<DocSearch>();
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
                    #region Original Document Delete

                    SqlCommand cmd = new SqlCommand("GetAllDeletableDoc", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UserID",""));
                    
                    SqlParameter outputStatusParam = new SqlParameter(spStatusParam, DbType.String)
                    { 
                          Direction = ParameterDirection.Output 
                    };
                    cmd.Parameters.Add(outputStatusParam);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    da.Dispose();


                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt1 = ds.Tables[0];
                        lstDocSearch = dt1.AsEnumerable().Select(reader => new DocSearch
                        {
                            //DocMetaIDVersion = reader.GetString("DocMetaIDVersion"),
                            //DocVersionID = reader.GetString("DocVersionID"),
                            DocMetaID = reader["DocMetaID"].ToString(),
                            FileCodeName = reader["FileCodeName"].ToString(),
                            DocumentID = reader["DocumentID"].ToString(),
                            //ReferenceVersionID = reader.GetString("ReferenceVersionID"),

                            DocPropIdentifyID = reader["DocPropIdentifyID"].ToString(),
                            DocPropIdentifyName = reader["DocPropIdentifyName"].ToString(),
                            MetaValue = reader["MetaValue"].ToString(),
                            //OriginalReference = reader.GetString("OriginalReference"),
                            //DocPropertyID = reader.GetString("DocPropertyID"),
                            //DocPropertyName = reader.GetString("DocPropertyName"),
                            FileServerURL = reader["FileServerURL"].ToString(),
                            ServerIP = reader["ServerIP"].ToString(),
                            ServerPort = reader["ServerPort"].ToString(),
                            FtpUserName = reader["FtpUserName"].ToString(),
                            FtpPassword = reader["FtpPassword"].ToString(),
                            VersionNo = reader["VersionNo"].ToString(),
                            DocDistributionID = reader["DocDistributionID"].ToString(),
                            //OwnerLevelID = reader.GetString("OwnerLevelID"),
                            OwnerID = reader["OwnerID"].ToString(),
                            DocCategoryID = reader["DocCategoryID"].ToString(),
                            DocTypeID = reader["DocTypeID"].ToString(),
                            DocPropertyID = reader["DocPropertyID"].ToString(),
                            DocPropertyName = reader["DocPropertyName"].ToString(),
                            SetOn = Convert.ToDateTime(reader["SetOn"]),
                            DeleteOn = Convert.ToDateTime(reader["DeleteOn"]),
                            AutoDelete = Convert.ToInt16(reader["AutoDelete"]),                           
                            Status = reader["Status"].ToString()
                        }).ToList();
                    }

                    var Totalresult = (from r in lstDocSearch
                                       group r by new
                                       {
                                           r.DocumentID
                                       }
                                           into g

                                           select new
                                           {
                                               DocumentID = g.Key.DocumentID,
                                               FileCodeName = g.Select(o => o.FileCodeName).FirstOrDefault(),
                                               FileServerURL = g.Select(o => o.FileServerURL).FirstOrDefault(),
                                               ServerIP = g.Select(o => o.ServerIP).FirstOrDefault(),
                                               ServerPort = g.Select(o => o.ServerPort).FirstOrDefault(),
                                               FtpUserName = g.Select(o => o.FtpUserName).FirstOrDefault(),
                                               FtpPassword = g.Select(o => o.FtpPassword).FirstOrDefault(),
                                               AutoDelete = g.Select(o => o.AutoDelete).FirstOrDefault(),
                                           }).ToList();


                    if (Totalresult.Count > 0)
                    {
                        foreach (var item in Totalresult)
                        {
                            if (item.AutoDelete == 1)
                            {
                                if (DeleteFile(item.ServerIP, item.ServerPort, item.FtpUserName, item.FtpPassword, item.FileServerURL, item.DocumentID, "Original"))
                                {
                                    DeletedIds.Add(item.DocumentID);
                                }
                            }
                           
                        }
                        if (DeletedIds.Count > 0)
                        {
                            SqlCommand cmdDelete = new SqlCommand("DeleteDocByPolicy", connection);
                            cmdDelete.CommandType = CommandType.StoredProcedure;
                            cmdDelete.Parameters.Add(new SqlParameter("@Documents", string.Join(",", DeletedIds.ToArray())));
                            cmdDelete.Parameters.Add(new SqlParameter("@DocumentNature", "Original"));

                            SqlParameter outputStatusParam_d = new SqlParameter(spStatusParam, DbType.String)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmdDelete.Parameters.Add(outputStatusParam_d);
                            cmdDelete.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region Version Document Delete
                    SqlCommand cmdDeleteVersion = new SqlCommand("GetAllDeletableDocVersion", connection);
                        cmdDeleteVersion.CommandType = CommandType.StoredProcedure;
                        cmdDeleteVersion.Parameters.Add(new SqlParameter("@UserID", ""));

                        SqlParameter outputStatusParamVersion = new SqlParameter(spStatusParam, DbType.String)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmdDeleteVersion.Parameters.Add(outputStatusParamVersion);
                        SqlDataAdapter daVersion = new SqlDataAdapter(cmdDeleteVersion);
                        daVersion.Fill(dsVersion);
                        daVersion.Dispose();


                        if (dsVersion.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt1 = dsVersion.Tables[0];
                            lstDocSearch = dt1.AsEnumerable().Select(reader => new DocSearch
                            {
                                DocMetaIDVersion = reader["DocMetaIDVersion"].ToString(),
                                DocVersionID = reader["DocVersionID"].ToString(),
                                //DocMetaID = reader["DocMetaID"].ToString(),
                                FileCodeName = reader["FileCodeName"].ToString(),
                                //DocumentID = reader["DocumentID"].ToString(),
                                //ReferenceVersionID = reader.GetString("ReferenceVersionID"),

                                DocPropIdentifyID = reader["DocPropIdentifyID"].ToString(),
                                DocPropIdentifyName = reader["DocPropIdentifyName"].ToString(),
                                MetaValue = reader["MetaValue"].ToString(),
                                //OriginalReference = reader.GetString("OriginalReference"),
                                //DocPropertyID = reader.GetString("DocPropertyID"),
                                //DocPropertyName = reader.GetString("DocPropertyName"),
                                FileServerURL = reader["FileServerURL"].ToString(),
                                ServerIP = reader["ServerIP"].ToString(),
                                ServerPort = reader["ServerPort"].ToString(),
                                FtpUserName = reader["FtpUserName"].ToString(),
                                FtpPassword = reader["FtpPassword"].ToString(),
                                VersionNo = reader["VersionNo"].ToString(),
                                DocDistributionID = reader["DocDistributionID"].ToString(),
                                //OwnerLevelID = reader.GetString("OwnerLevelID"),
                                OwnerID = reader["OwnerID"].ToString(),
                                DocCategoryID = reader["DocCategoryID"].ToString(),
                                DocTypeID = reader["DocTypeID"].ToString(),
                                DocPropertyID = reader["DocPropertyID"].ToString(),
                                DocPropertyName = reader["DocPropertyName"].ToString(),
                                SetOn = Convert.ToDateTime(reader["SetOn"]),
                                DeleteOn = Convert.ToDateTime(reader["DeleteOn"]),
                                AutoDelete = Convert.ToInt16(reader["AutoDelete"]),    
                                Status = reader["Status"].ToString()
                            }).ToList();
                        }

                        var TotalresultVersion = (from r in lstDocSearch
                                           group r by new
                                           {
                                               r.DocVersionID
                                           }
                                               into g

                                               select new
                                               {
                                                   DocVersionID = g.Key.DocVersionID,
                                                   FileCodeName = g.Select(o => o.FileCodeName).FirstOrDefault(),
                                                   FileServerURL = g.Select(o => o.FileServerURL).FirstOrDefault(),
                                                   ServerIP = g.Select(o => o.ServerIP).FirstOrDefault(),
                                                   ServerPort = g.Select(o => o.ServerPort).FirstOrDefault(),
                                                   FtpUserName = g.Select(o => o.FtpUserName).FirstOrDefault(),
                                                   FtpPassword = g.Select(o => o.FtpPassword).FirstOrDefault(),
                                                   VersionNo=g.Select(o=>o.VersionNo).FirstOrDefault(),
                                                   AutoDelete = g.Select(o => o.AutoDelete).FirstOrDefault()
                                               }).ToList();


                        if (TotalresultVersion.Count > 0)
                        {
                            foreach (var item in TotalresultVersion)
                            {
                                if (item.AutoDelete == 1)
                                {
                                    if (DeleteFile(item.ServerIP, item.ServerPort, item.FtpUserName, item.FtpPassword, item.FileServerURL, item.DocVersionID, "Version", item.VersionNo))
                                    {
                                        DeletedIdsVersion.Add(item.DocVersionID);
                                    }
                                }
                                
                            }
                            if (DeletedIdsVersion.Count > 0)
                            {
                                SqlCommand cmdDelete = new SqlCommand("DeleteDocByPolicy", connection);
                                cmdDelete.CommandType = CommandType.StoredProcedure;
                                cmdDelete.Parameters.Add(new SqlParameter("@Documents", string.Join(",", DeletedIdsVersion.ToArray())));
                                cmdDelete.Parameters.Add(new SqlParameter("@DocumentNature", "Version"));
                                
                                SqlParameter outputStatusParam_d_v = new SqlParameter(spStatusParam, DbType.String)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                cmdDelete.Parameters.Add(outputStatusParam_d_v);
                                cmdDelete.ExecuteNonQuery();
                            }
                        }


                    #endregion

                    #region SendMail to Owner Of Original Document

                    SqlCommand cmdAlert = new SqlCommand("GetAllAletibleDoc", connection);
                    cmdAlert.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter daAlert = new SqlDataAdapter(cmdAlert);
                    daAlert.Fill(dsAlert);
                    daAlert.Dispose();
                    List<EmailDataViewModel> doclist=new List<EmailDataViewModel>();
                     if (dsAlert.Tables[0].Rows.Count > 0)
                     {
                        DataTable dta = dsAlert.Tables[0];
                        doclist = dta.AsEnumerable().Select(reader => new EmailDataViewModel
                        {
                            DestroyPolicyID = reader["DestroyPolicyID"].ToString(),
                            PolicyFor = reader["PolicyFor"].ToString(),
                            DocumentNature=reader["DocumentNature"].ToString(),
                            OwnerLavelID=reader["OwnerLavelID"].ToString(),
                            OwnerID = reader["OwnerID"].ToString(),
                            DocCategoryID = reader["DocCategoryID"].ToString(),
                            DocTypeID = reader["DocTypeID"].ToString(),
                            DocPropertyID = reader["DocPropertyID"].ToString(),
                            DocPropIdentifyID = reader["DocPropIdentifyID"].ToString(),
                            MetaValue = reader["MetaValue"].ToString(),
                            TimeValue = Convert.ToInt32(reader["TimeValue"]),
                            TimeValueCon = Convert.ToInt32(reader["TimeValueCon"]),
                            TimeUnit = reader["TimeUnit"].ToString(),
                            Emails = reader["Emails"].ToString() 
                        }).ToList();

                        var DocMail = (from r in doclist
                                           group r by new
                                           {
                                               r.DestroyPolicyID
                                           }
                                               into g

                                               select new
                                               {
                                                   DestroyPolicyID = g.Key.DestroyPolicyID,
                                                   PolicyFor = g.Select(o => o.PolicyFor).FirstOrDefault(),
                                                   DocumentNature = g.Select(o=>o.DocumentNature).FirstOrDefault(),
                                                   OwnerLavelID=g.Select(o=>o.OwnerLavelID).FirstOrDefault(),
                                                   OwnerID = g.Select(o => o.OwnerID).FirstOrDefault(),
                                                   DocCategoryID = g.Select(o => o.DocCategoryID).FirstOrDefault(),
                                                   DocTypeID = g.Select(o => o.DocTypeID).FirstOrDefault(),
                                                   DocPropertyID = g.Select(o => o.DocPropertyID).FirstOrDefault(),
                                                   DocPropIdentifyID = g.Select(o => o.DocPropIdentifyID).FirstOrDefault(),
                                                   MetaValue = g.Select(o => o.MetaValue).FirstOrDefault(),
                                                   TimeValue = g.Select(o => o.TimeValueCon).FirstOrDefault(),
                                                   TimeValueCon = g.Select(o => o.Emails).FirstOrDefault(),
                                                   TimeUnit = g.Select(o => o.TimeUnit).FirstOrDefault(),
                                                   Emails   =g.Select(o=>o.Emails).FirstOrDefault()
                                               }).ToList();

                        if (DocMail.Count > 0)
                        {
                            LogMaintain("Data Found");

                            foreach (var item in DocMail)
                            {
                                SendMail(item.Emails,"", "", "Alert for Deleting Document","There is some Documents for Delete Under the policy named: "+item.PolicyFor
                                    +"Type: "+item.DocumentNature+ Environment.NewLine 
                                    + "Please visit the link to manupulate deestroying manualy <a href=\"http://localhost:44300/DocScanningModule/DocDestroyByPolicy/Index#?_OwnerLavelID=" 
                                    + item.OwnerLavelID
                                    + "&_OwnerID=" + item.OwnerID + "&_DocCategoryID=" + item.DocCategoryID
                                    + "&_DocTypeID=" + item.DocTypeID + "&_DocPropertyID=" + item.DocPropertyID
                                    + "&_DestroyPolicyID=" + item.DestroyPolicyID + "\">Manage Destroy</a>");

                                LogMaintain("Mail Send for "+item.PolicyFor+"");
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

        private bool DeleteFile(string ftpIp, string port, string username,string password,string fileServerUrl,string _DocumentID,string _DocNature,string _VersionNo="")
        {
            string documentName;
            if (_DocNature == "Version")
            {
                documentName = _DocumentID + "_v_" + _VersionNo + ".pdf";
            }
            else
                documentName = _DocumentID + ".pdf";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpIp + "/" + fileServerUrl + "/" + documentName);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Proxy = null;

            request.Credentials = new NetworkCredential(username, password);

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else 
                {
                    response.Close();
                    return false;
                }
            }   
        }

        public void SendMail(String toEmail, string ccAddress, string bccAddress, string Subj, string Message)
        {

            //Reading sender Email credential from web.config file
            HostAdd = ConfigurationManager.AppSettings["Host"].ToString();
            FromEmailid = ConfigurationManager.AppSettings["FromMail"].ToString();
            Password = ConfigurationManager.AppSettings["Password"].ToString();
            // ToEmail = "shalim@squaregroup.com";
            //creating the object of MailMessage
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(FromEmailid); //From Email Id
            mailMessage.Subject = Subj; //Subject of Email
            mailMessage.Body = Message; //body or message of Email
            mailMessage.IsBodyHtml = true;
            string[] toMuliId = toEmail.Split(',');
            foreach (string toEMailId in toMuliId)
            {
                if (!string.IsNullOrEmpty(toEMailId))
                {
                    mailMessage.To.Add(new MailAddress(toEMailId)); //adding multiple TO Email Id
                }
            }

            // ccAddress = "jain@squaregroup.com";
            string[] CCId = ccAddress.Split(',');
            foreach (string ccEmail in CCId)
            {
                if (!string.IsNullOrEmpty(ccEmail))
                {
                    mailMessage.CC.Add(new MailAddress(ccEmail)); //Adding Multiple CC email Id
                }
            }

            // bccAddress = "shalim@squaregroup.com";
            string[] bccid = bccAddress.Split(',');
            foreach (string bccEmailId in bccid)
            {
                if (!string.IsNullOrEmpty(bccEmailId))
                {
                    mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                }
            }
            SmtpClient smtp = new SmtpClient(); 
            smtp.Host = HostAdd; 
            smtp.EnableSsl = true;
            NetworkCredential networkCred = new NetworkCredential();
            networkCred.UserName = mailMessage.From.Address;
            networkCred.Password = Password;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCred;
            smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);





            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            try
            {
                smtp.Send(mailMessage);
      
            }
            catch (Exception ex)
            {
            }
            
        }

        #region Log Maintain
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

        #endregion

    }
}