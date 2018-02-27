using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aquarium.Log;

namespace Aquarium
{
    public  class Logger : ILogger
    {
        public bool LogInfo { get; set; }
        private static string logFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/log.txt";
        private static string logFileError = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/logError.txt";


        public string GetLogPath()
        {
            return logFile;
        }

        public  void Write(string message, LoggerTypes.LogLevel logLevel = LoggerTypes.LogLevel.Error)
        {
            if (logLevel == LoggerTypes.LogLevel.Info && !LogInfo)
                return;

            var logLine = $"{DateTime.Now} {logLevel}\t{message}{Environment.NewLine}";

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(logFile, append: true))
                {
                    streamWriter.Write(logLine);
                }
            }
            catch (IOException)
            {
                using (StreamWriter streamWriter = new StreamWriter(logFileError, append: true))
                {
                    streamWriter.Write(logLine);
                }
            }

            Console.Write(logLine);
        }

        public  void Write(Exception e)
        {
            Write(GetAllExceptionMessages(e));
        }

        private string GetAllExceptionMessages(Exception e)
        {
            if (e != null)
                return  e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + GetAllExceptionMessages(e.InnerException);

            return "";
        }
    }
}
