using MasterApi45.Core.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MasterApi45.Core.Logging
{
    public interface ILogger
    {
        bool IsInfoEnabled { get; }
        bool IsDebugEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }

        void Info(string message, Exception exception = null);
        void Debug(string message, Exception exception = null);
        void Warn(string message, Exception exception = null);
        void Error(string message, Exception exception = null);
        void Fatal(string message, Exception exception = null);

        Task InfoAsync(string message, Exception exception = null);
        Task DebugAsync(string message, Exception exception = null);
        Task WarnAsync(string message, Exception exception = null);
        Task ErrorAsync(string message, Exception exception = null);
        Task FatalAsync(string message, Exception exception = null);
    }
    public class Logger : ILogger
    {
        #region Private Members
        private static string _logDBConnectionString;
        private IEmailer _emailer;
        private Guid _contextIdentifier;

        private string _username = string.Empty;
        private string _path = string.Empty;
        private string _browser = string.Empty;

        private static string LogDBConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_logDBConnectionString))
                {
                    _logDBConnectionString = ConfigurationManager.ConnectionStrings["LogDbContext"].ConnectionString.ToString();
                }
                return _logDBConnectionString;
            }
        }
        #endregion

        #region Public Properties
        public bool IsDebugEnabled
        {
            get
            {
                return IsEnabledFor(Enums.LogLevel.DEBUG);
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return IsEnabledFor(Enums.LogLevel.ERROR);
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return IsEnabledFor(Enums.LogLevel.FATAL);
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return IsEnabledFor(Enums.LogLevel.INFO);
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return IsEnabledFor(Enums.LogLevel.WARN);
            }
        }
        #endregion

        #region Constructor
        public Logger(IEmailer emailer)
        {
            _emailer = emailer;
            _contextIdentifier = Guid.NewGuid();
            FillWebRequestData();
        }
        #endregion

        #region Public Methods       

        public void Debug(string message, Exception exception = null)
        {
            new Task(() =>
            {
                LogToDB(message, exception, Enums.LogLevel.DEBUG);
            }).RunSynchronously();
        }

        public Task DebugAsync(string message, Exception exception = null)
        {
            return Task.Run(() =>
            {
                LogToDB(message, exception, Enums.LogLevel.DEBUG);
            });
        }

        public void Error(string message, Exception exception = null)
        {
            new Task(() =>
            {
                LogToDB(message, exception, Enums.LogLevel.DEBUG);
            }).RunSynchronously();
        }

        public Task ErrorAsync(string message, Exception exception = null)
        {
            return Task.Run(() =>
            {
                LogToDB(message, exception, Enums.LogLevel.DEBUG);
            });
        }

        public void Fatal(string message, Exception exception = null)
        {
            new Task(() =>
            {
                LogToDB(message, exception, Enums.LogLevel.DEBUG);
            }).RunSynchronously();
        }

        public Task FatalAsync(string message, Exception exception = null)
        {
            return Task.Run(() =>
            {
                LogToDB(message, exception, Enums.LogLevel.DEBUG);
            });
        }

        public void Info(string message, Exception exception = null)
        {
            new Task(() =>
            {
                LogToDB(message, exception, Enums.LogLevel.INFO);
            }).RunSynchronously();
        }

        public Task InfoAsync(string message, Exception exception = null)
        {
            return Task.Run(() =>
            {
                LogToDB(message, exception, Enums.LogLevel.DEBUG);
            });
        }

        public void Warn(string message, Exception exception = null)
        {
            new Task(() =>
            {
                LogToDB(message, exception, Enums.LogLevel.DEBUG);
            }).RunSynchronously();
        }

        public Task WarnAsync(string message, Exception exception = null)
        {
            return Task.Run(() =>
            {
                LogToDB(message, exception, Enums.LogLevel.DEBUG);
            });
        }
        #endregion

        #region Private Methods

        private  bool IsEnabledFor(Enums.LogLevel logLevel)
        {
            Enums.LogLevel appLogLevel = default(Enums.LogLevel);
            if (Enum.TryParse(ConfigurationManager.AppSettings["log:Level"], out appLogLevel))
            {
                return logLevel >= appLogLevel;
            }
            else
            {
                return true;
            }
        }

        private string GetFullException(Exception ex)
        {
            Exception e = ex;
            StringBuilder s = new StringBuilder();
            while ((e != null))
            {
                s.AppendLine("Exception type: " + e.GetType().FullName);
                s.AppendLine("Message       : " + e.Message);
                s.AppendLine("Stacktrace:   ");
                s.AppendLine(e.StackTrace);
                s.AppendLine();
                e = e.InnerException;

                if (ex.GetType().FullName == "System.Data.Entity.Validation.DbEntityValidationException")
                {
                    s.AppendLine("EntityValidationErrors");

                    dynamic dbEntityValidationExceptionType = ex.GetType();

                    foreach (var failure in dbEntityValidationExceptionType.GetProperty("EntityValidationErrors").GetValue(ex, null))
                    {
                        string validationErrors = "";

                        foreach (var errorDetails in failure.ValidationErrors)
                        {
                            validationErrors += errorDetails.PropertyName + ": " + errorDetails.ErrorMessage;
                        }
                        s.AppendLine(validationErrors);
                    }
                }
            }
            return s.ToString();
        }

        private void LogToDB(string message, Exception ex, Enums.LogLevel level)
        {
            //' Check log levels
            if (!IsEnabledFor(level))
            {
                return;
            }

            string logMessage = message;

            if (string.IsNullOrEmpty(message.Trim()) & ex == null)
            {
                return;
            }
            else if (ex != null)
            {
                logMessage = logMessage + Environment.NewLine + Environment.NewLine + GetFullException(ex);
            }

            using (SqlConnection sqlConnection = new SqlConnection(LogDBConnectionString))
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO LOGS (Message, Type, Context, Username, Path, Browser) VALUES(@message, @type, @context, @username, @path, @browser)", sqlConnection))
                {

                    if (string.IsNullOrEmpty(_username.Trim()))
                    {
                        _username = System.Threading.Thread.CurrentPrincipal.Identity.Name.Trim();
                    }

                    try
                    {
                        command.Parameters.AddWithValue("@message", logMessage.Trim());
                        command.Parameters.AddWithValue("@type", level.ToString());
                        command.Parameters.AddWithValue("@context", _contextIdentifier.ToString());

                        if (string.IsNullOrEmpty(_path))
                        {
                            command.Parameters.AddWithValue("@path", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@path", _path);
                        }

                        if (string.IsNullOrEmpty(_username))
                        {
                            command.Parameters.AddWithValue("@username", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@username", _username);
                        }

                        if (string.IsNullOrEmpty(_browser))
                        {
                            command.Parameters.AddWithValue("@browser", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@browser", _browser);
                        }

                        sqlConnection.Open();
                        command.ExecuteNonQuery();
                        sqlConnection.Close();

                    }
                    catch (Exception exception)
                    {
                        SendEmail("Exception in logging to Database: " + exception.Message, exception, Enums.LogLevel.ERROR);
                    }
                    finally
                    {
                        if (sqlConnection != null & sqlConnection.State == ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }

                        //' Depending on the log level send email
                        if (level >= Enums.LogLevel.WARN)
                        {
                            SendEmail(logMessage.Trim(), null, level);
                        }
                    }
                }
            }

        }

        #endregion

        #region Private Methods
        private void SendEmail(string message, Exception ex, Enums.LogLevel level)
        {
            try
            {
                string logMessage = message;
                string subject = string.Format("{0} {1} {2}", ConfigurationManager.AppSettings["log:ApplicationName"].ToString(), level.ToString(), message);

                if (ex != null)
                {
                    logMessage = logMessage + Environment.NewLine + Environment.NewLine + GetFullException(ex);
                }

                _emailer.Send(ConfigurationManager.AppSettings["email:WebsiteAdministrator"], subject, logMessage);
            }
            catch (Exception exception)
            {

                
            }
        }

        private void FillWebRequestData()
        {

            if (System.Web.HttpContext.Current != null)
            {
                //' 1. Get current request path
                _path = System.Web.HttpContext.Current.Request.Path;

                //' 2. Get current logged in user
                if (System.Web.HttpContext.Current.User != null)
                {
                    if (System.Web.HttpContext.Current.User.Identity != null)
                    {
                        _username = Convert.ToString(System.Web.HttpContext.Current.User.Identity.Name);
                    }
                }

                //' 3. Get browser details as json so that in future we may query this in better way {"UserAgent":"{0}","Browser":"{1}","Version":"{2}","Platform":"{3}","ECMA":"{4}","Type":"{5}"}
                if (System.Web.HttpContext.Current.Request != null)
                {
                    if (System.Web.HttpContext.Current.Request.Browser != null)
                    {
                        var request = System.Web.HttpContext.Current.Request;
                        string httpUserAgent = "Unknown";
                        if (request.ServerVariables["http_user_agent"] != null)
                        {
                            httpUserAgent = request.ServerVariables["http_user_agent"].ToString();
                        }
                        string broserData = "{" + string.Format("'UserAgent':'{0}','Browser':'{1}','Version':'{2}','Platform':'{3}','ECMA':'{4}','Type':'{5}'", httpUserAgent, request.Browser.Browser.ToString(), request.Browser.Version.ToString(), request.Browser.Platform.ToString(), request.Browser.EcmaScriptVersion.ToString(), request.Browser.Type.ToString()) + "}";
                        if (broserData.Length > 1000)
                        {
                            broserData = broserData.Substring(0, 1000);
                        }
                    }
                }

            }
        }

        #endregion
    }
}
