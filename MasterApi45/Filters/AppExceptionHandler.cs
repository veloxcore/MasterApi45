using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace MasterApi45.Filters
{
    public class AppExceptionHandler : ExceptionHandler
    {
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            context.Result = new ErrorResult()
            {
                Request = context.ExceptionContext.Request,
                Content = "Internal server error."
            };
            return base.HandleAsync(context, cancellationToken);
        }

        private class ErrorResult : IHttpActionResult
        {
            public HttpRequestMessage Request { get; set; }
            public string Content { get; set; }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                response.Content = new StringContent(Content);
                response.RequestMessage = Request;
                return Task.FromResult(response);
            }
        }
    }


}