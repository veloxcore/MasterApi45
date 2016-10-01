using MasterApi45.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace MasterApi45.Filters
{
    public class AppExceptionLogger : ExceptionLogger
    {
        #region Private Members
        private ILogger _logger;
        #endregion

        #region Constructor
        public AppExceptionLogger(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region Public Methods
        public override void Log(ExceptionLoggerContext context)
        {
            _logger.Error("Exception in: " + context.Request.RequestUri.ToString(), context.Exception);
        }
        #endregion
    }
}