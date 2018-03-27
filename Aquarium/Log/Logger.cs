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
    public class Logger : ILogger
    {
        public bool LogInfo { get; set; }
        private static string _logFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/";

        private string _fileName = "log";
        private string _fileExtension = ".txt";
        private int _logNumber = 1;

        private bool _locked;
        private object lockingObject = new object();

        public string GetLogPath()
        {
            return _logFilePath + _fileName + _fileExtension;
        }

        private void Lock()
        {
            lock (lockingObject)
            {
                while (_locked)
                {

                }

                _locked = true;
            }
        }

        private void UnLock()
        {
            lock (lockingObject)
            {
                _locked = false;
            }
        }

        public void Write(string message, LoggerTypes.LogLevel logLevel = LoggerTypes.LogLevel.Error)
        {
            if (logLevel == LoggerTypes.LogLevel.Info && !LogInfo)
                return;

            Lock();
            var logLine = "";
            using (var stream = File.OpenWrite(GetLogPath()))
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    lock (streamWriter)
                    {
                        logLine = $"{DateTime.Now} {logLevel}\t{message}{Environment.NewLine}";
                        streamWriter.Write(logLine);
                        streamWriter.Flush();
                    }
                }
            }
            UnLock();

            Console.Write(logLine);
        }

        public void Write(Exception e)
        {
            Write(GetAllExceptionMessages(e));
        }

        private string GetAllExceptionMessages(Exception e)
        {
            if (e != null)
                return e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + GetAllExceptionMessages(e.InnerException);

            return "";
        }
    }
}
