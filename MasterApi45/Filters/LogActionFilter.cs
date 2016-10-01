using Autofac.Integration.WebApi;
using MasterApi45.Core.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MasterApi45.Filters
{
    /// <summary>
    /// Action Filters are used to log action methods.
    /// </summary>
    public class LogActionFilter : IAutofacActionFilter
    {
        #region Private Members 
        private ILogger _logger;
        #endregion

        #region Constructor 
        public LogActionFilter(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region Public Methods
        public void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            // Don't handle exceptions here, it should be handled by AppExceptionHandler and Logged by AppExceptionLogger
            if (_logger.IsInfoEnabled && actionExecutedContext.Exception == null)
            {
                var responseData = new object();
                if (actionExecutedContext.Response != null && actionExecutedContext.Response.Content != null)
                {
                    if (actionExecutedContext.Response.Content is ObjectContent)
                    {
                        responseData = ((ObjectContent)actionExecutedContext.Response.Content).Value;
                    }
                    else
                    {
                        responseData = actionExecutedContext.Response.Content.ToString();
                    }
                }

                try
                {
                    var jsonResponse = JsonConvert.SerializeObject(responseData, Formatting.None);
                    _logger.Info(string.Format("{0} Response Code: {1}, Response: {2}", actionExecutedContext.Request.RequestUri.ToString(), actionExecutedContext.Response.StatusCode.ToString(), jsonResponse));
                }
                catch (Newtonsoft.Json.JsonWriterException ex)
                {
                    _logger.Info(string.Format("{0} Response Code: {1}", actionExecutedContext.Request.RequestUri.ToString(), actionExecutedContext.Response.StatusCode.ToString()));
                }
            }
        }
        

        public void OnActionExecuting(HttpActionContext actionContext)
        {
            if (_logger.IsInfoEnabled)
            {
                var jsonParameters = JsonConvert.SerializeObject(actionContext.ActionArguments, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                _logger.Info(string.Format("Request {0} {1}, Parameters: {2}", actionContext.Request.Method.ToString(), actionContext.Request.RequestUri.ToString(), jsonParameters));
            }
        }
        #endregion
    }
}