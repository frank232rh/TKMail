using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TKMail.Data.Helpers
{
    public class LogMethods
    {
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
