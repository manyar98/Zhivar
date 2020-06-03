using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Log;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;

namespace OMF.Common.ExceptionManagement
{
    public class ExceptionManager
    {
        public static Action<Exception> ExceptionViewer;

        public static void HandleException(Exception ex)
        {
            if (ex.Data.Contains((object)"HandledException"))
                return;
            ExceptionManager.SaveException(ex);
            ExceptionManager.ShowException(ex);
            ex.Data.Add((object)"HandledException", (object)true);
        }

        public static void SaveException(Exception ex)
        {
            if (ex.Data.Contains((object)"SavedException"))
                return;
            ExceptionData exData = new ExceptionData(ex);
            ExceptionManager.Save(exData);
            if (ex.Data.Contains((object)"ExceptionCode"))
                ex.Data[(object)"ExceptionCode"] = (object)exData.Code;
            else
                ex.Data.Add((object)"ExceptionCode", (object)exData.Code);
            ex.Data.Add((object)"SavedException", (object)true);
        }

        private static void Save(ExceptionData exData)
        {
            try
            {
                foreach (IExceptionLogger exceptionLogger in ConfigurationController.ExceptionLoggers)
                {
                    try
                    {
                        using (new TransactionScope(TransactionScopeOption.Suppress))
                            exceptionLogger.LogException(exData);
                    }
                    catch (Exception ex)
                    {
                        EventLogHelper.Write(ExceptionManager.GetExceptionMessageWithDebugInfo((Exception)new OMFException(string.Format("{0} exception", (object)exceptionLogger.GetType().FullName), ex)), EventLogEntryType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new LogManagementException("[ExceptionServiceLoggerName] failed and throw an exception", ex);
            }
        }

        public static async Task HandleExceptionAsync(Exception ex)
        {
            if (ex.Data.Contains((object)"HandledException"))
                return;
            await ExceptionManager.SaveExceptionAsync(ex);
            ExceptionManager.ShowException(ex);
            ex.Data.Add((object)"HandledException", (object)true);
        }

        public static async Task SaveExceptionAsync(Exception ex)
        {
            if (ex.Data.Contains((object)"SavedException"))
                return;
            string code = string.Empty;
            ExceptionData exData = new ExceptionData(ex);
            code = exData.Code;
            if (ex.Data.Contains((object)"ExceptionCode"))
                ex.Data[(object)"ExceptionCode"] = (object)code;
            else
                ex.Data.Add((object)"ExceptionCode", (object)code);
            await ExceptionManager.SaveAsync(exData);
            ex.Data.Add((object)"SavedException", (object)true);
        }

        private static async Task SaveAsync(ExceptionData exData)
        {
            try
            {
                foreach (IExceptionLogger exceptionLogger in ConfigurationController.ExceptionLoggers)
                {
                    IExceptionLogger exLogger = exceptionLogger;
                    try
                    {
                        using (new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            if (exLogger is IExceptionLoggerAsync)
                                await ((IExceptionLoggerAsync)exLogger).LogExceptionAsync(exData);
                            else
                                exLogger.LogException(exData);
                        }
                    }
                    catch (Exception ex)
                    {
                        OMFException dwdEx = new OMFException(string.Format("{0} exception: {1}", (object)exLogger.GetType().FullName, (object)ex.Message), exData.Exception);
                        string message = ExceptionManager.GetExceptionMessageWithDebugInfo((Exception)dwdEx);
                        EventLogHelper.Write(message, EventLogEntryType.Error);
                        continue;
                    }
                    exLogger = (IExceptionLogger)null;
                }
            }
            catch (Exception ex)
            {
                throw new LogManagementException("[ExceptionServiceLoggerName] failed and throw an exception", ex);
            }
        }

        public static void ShowException(Exception ex)
        {
            if (ExceptionManager.ExceptionViewer == null)
                return;
            ExceptionManager.ExceptionViewer(ex);
        }

        private static TException GetException<TException>(Exception ex) where TException : Exception
        {
            if (ex is TException)
                return (TException)ex;
            return ExceptionManager.GetException<TException>(ex.InnerException);
        }

        private static bool IsException<TException>(Exception ex) where TException : Exception
        {
            if (ex == null)
                return false;
            if (ex is TException)
                return true;
            return ExceptionManager.IsException<TException>(ex.InnerException);
        }

        public static string GetExceptionMessageWithDebugInfo(Exception ex)
        {
            string str = "";
            for (; ex != null; ex = ex.InnerException)
                str += string.Format("--------------------------------------------------{0}{1}{0}=================================================={0}{2}{0}", (object)Constants.NewLine, (object)ex.Message, (object)ex.ToString());
            return str;
        }

        public static string GetExceptionMessageWithoutDebugInfo(Exception ex)
        {
            string str = "";
            if (ex.Data.Contains((object)"ExceptionCode"))
                str = ex.Data[(object)"ExceptionCode"].ToString();
            return string.Format("بروز خطا در سیستم!!!{0}لطفاً با پشتیبانی تماس بگیرید.", (object)Constants.NewLine) + Constants.NewLine + "کد خطا: " + (string.IsNullOrWhiteSpace(str) ? "ثبت نشده" : str);
        }

        public static string GetExceptionMessage(Exception ex)
        {
            if (ConfigurationController.WithDebugInfo)
                return ExceptionManager.GetExceptionMessageWithDebugInfo(ex);
            return ExceptionManager.GetExceptionMessageWithoutDebugInfo(ex);
        }
    }
}
