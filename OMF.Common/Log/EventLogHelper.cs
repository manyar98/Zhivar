using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement;

namespace OMF.Common.Log
{
    public static class EventLogHelper
    {
        public static void Write(string message, EventLogEntryType type)
        {
            EventLog eventLog = new EventLog();
            try
            {
                eventLog.Log = "Application";
                eventLog.Source = "BPJ.Framework";
                eventLog.WriteEntry(message, type);
            }
            catch (Exception ex1)
            {
                try
                {
                    string str = Path.Combine(Path.Combine(ConfigurationController.ExceptionLogPath, "NotEventLogs"), DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".txt");
                    FileInfo fileInfo = new FileInfo(str);
                    if (!fileInfo.Directory.Exists)
                        fileInfo.Directory.Create();
                    if (!fileInfo.Exists)
                        fileInfo.Create().Close();
                    if (ex1 is SecurityException)
                        File.WriteAllText(str, string.Format("{0} - LogName: '{1}' - SourceName: '{2}'", (object)ex1.Message, (object)eventLog.Log, (object)eventLog.Source));
                    File.AppendAllText(str, "\r\n");
                    File.AppendAllText(str, message);
                }
                catch (Exception ex2)
                {
                    StringBuilder stringBuilder = new StringBuilder(ex2.ToString());
                    stringBuilder.Append("-----------------------------------------");
                    stringBuilder.Append(ex1.ToString());
                    stringBuilder.Append("-----------------------------------------");
                    stringBuilder.Append(message);
                    ExceptionManager.ShowException(new Exception(stringBuilder.ToString()));
                }
            }
        }
    }
}
