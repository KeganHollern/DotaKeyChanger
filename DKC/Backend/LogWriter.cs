using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DKC.Backend
{
    public class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter()
        {

        }
        public void LogWrite(string logMessage)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            try
            {
                File.AppendAllText(m_exePath + "\\" + "log.txt", string.Format("{0} {1}: {2}\r\n", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString(), logMessage));
                
            }
            catch (Exception ex)
            {
            }

        }

    }
}
