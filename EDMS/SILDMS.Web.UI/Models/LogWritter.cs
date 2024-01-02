using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SILDMS.Web.UI.Models
{
    public static class LogWritter
    {
        public static void WriteLog(Exception ex)
        {
            try
            {
                string filepath = "D:\\DebugHelper\\" + DateTime.Today.ToString("dd-MM-yy") + ".txt"; 
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString();
                    sw.WriteLine(error+"=================================");
                    sw.WriteLine(ex.Message);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        public static void WriteLog(String message)
        {
            try
            {

                string filepath = "D:\\DebugHelper\\" + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString();
                    sw.WriteLine(error + "=================================");
                    sw.WriteLine(message);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch (Exception e)
            {
                e.ToString();
            }
        } 

    }
}