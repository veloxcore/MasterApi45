using Autofac;
using Autofac.Integration.WebApi;
using MasterApi45.Core.Logging;
using MasterApi45.Core.Mail;
using MasterApi45.Filters;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace MasterApi45
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();
            ConfigureAutofac(app, httpConfig);

            WebApiConfig.Register(httpConfig);

            httpConfig.Services.Replace(typeof(IExceptionHandler), httpConfig.DependencyResolver.GetService(typeof(AppExceptionHandler)));
            httpConfig.Services.Replace(typeof(IExceptionLogger), httpConfig.DependencyResolver.GetService(typeof(AppExceptionLogger)));

            app.UseAutofacWebApi(httpConfig);
            app.UseWebApi(httpConfig);
        }

        #region Private Methods
        private void ConfigureAutofac(IAppBuilder app, HttpConfiguration httpConfig)
        {
            var builder = new ContainerBuilder();

            Service.Startup.RegisterDependency(builder);

            builder.RegisterAssemblyTypes(typeof(Startup).Assembly).Where(t => t.Name.EndsWith("Controller")).AsSelf().InstancePerDependency();
            builder.RegisterType<NustacheTemplateRenderer>().As<ITemplateRenderer>().InstancePerDependency();
            builder.RegisterType<Emailer>().As<IEmailer>().InstancePerDependency();
            builder.RegisterType<Logger>().As<ILogger>().InstancePerDependency();
            builder.RegisterType<LogInterceptor>().AsSelf().InstancePerDependency();
            builder.RegisterType<LogActionFilter>().AsWebApiActionFilterFor<ApiController>().InstancePerDependency();
            builder.RegisterType<AppExceptionHandler>().AsSelf().InstancePerDependency();
            builder.RegisterType<AppExceptionLogger>().AsSelf().InstancePerDependency();

            //Added to register action Filter.
            builder.RegisterWebApiFilterProvider(httpConfig);
            IContainer container = builder.Build();
            httpConfig.DependencyResolver = new Autofac.Integration.WebApi.AutofacWebApiDependencyResolver(container);
            app.UseAutofacMiddleware(container);
        }
        #endregion
    }
}