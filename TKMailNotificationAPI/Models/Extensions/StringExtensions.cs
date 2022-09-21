using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace TKMailNotificationAPI.Models.Extensions
{
    public static class StringExtensions
    {
        public static void createLog(this string strMsg, string function, string Linea)
        {
            try
            {
                string message = string.Format("{0,19} - {1,40} - {2,4} - {3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), function, Linea, strMsg);
                string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Logs\\Log_" + string.Format("{0:yyyyMMdd}", DateTime.Now).ToString() + ".txt";
                using (StreamWriter logFile = new StreamWriter(path, true))
                {
                    logFile.WriteLine(message);
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}