using Autofac;
using Autofac.Integration.WebApi;
using BarcoderApp.RestApi.Providers;
using MasterApi45.Core.Logging;
using MasterApi45.Core.Mail;
using MasterApi45.Filters;
using MasterApi45.Providers;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            ConfigureOAuthTokenGeneration(app, httpConfig);
            ConfigureOAuthTokenConsumption(app);
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
            builder.RegisterType<OAuthProvider>().AsSelf().InstancePerLifetimeScope();

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

        private void ConfigureOAuthTokenGeneration(IAppBuilder app, HttpConfiguration config)
        {
            var OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = Convert.ToBoolean(ConfigurationManager.AppSettings["oauth:AllowInsecureHttp"]),
                TokenEndpointPath = new Microsoft.Owin.PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["oauth:TokenExpireTime"])),
                Provider = (IOAuthAuthorizationServerProvider)config.DependencyResolver.GetService(typeof(OAuthProvider)),
                AccessTokenFormat = new JWTAccessTokenFormater(ConfigurationManager.AppSettings["oauth:TokenIssuerUrl"])
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            var issuer = ConfigurationManager.AppSettings["oauth:TokenIssuerUrl"];
            string audienceId = ConfigurationManager.AppSettings["jwt:AudienceId"];
            byte[] audienceSecret = Microsoft.Owin.Security.DataHandler.Encoder.TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["jwt:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                AllowedAudiences = new string[] { audienceId },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[] { new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret) }
            });
        }
        #endregion
    }
}