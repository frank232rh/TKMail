using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TKMail.NotificationService.Entities.Extensions
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
            catch (Exception)
            {

            }
        }
        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public static void writeException(Exception ex, string source)
        {
            try
            {
                string ruta = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Logs";
                //string ruta = Path.Combine(Assembly.GetEntryAssembly().Location,"Logs");
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }
                string fileName = Path.Combine(ruta, string.Format("Log_{0}.txt", DateTime.Now.ToString("yyyyMMdd")));
                List<string> lines = new List<string>();
                lines.Add(string.Format("{1,19} - Error: {0} ", source, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                lines.Add(string.Format("Stack: {0}", ex.StackTrace));
                while (ex != null)
                {
                    lines.Add(string.Format("Message: {0}", ex.Message));
                    ex = ex.InnerException;
                }
                File.AppendAllLines(fileName, lines);
            }
            catch (Exception ex2)
            {

            }

        }
    }
}
