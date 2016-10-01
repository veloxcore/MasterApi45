using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy2;
using MasterApi45.Core.Logging;

namespace MasterApi45.Data
{
    public class Startup
    {
        public static void RegisterDependency(ContainerBuilder builder)
        {
            builder.RegisterType<Data.MasterApi45Context>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<Infrastracture.UnitOfWork>().As<Infrastracture.IUnitOfWork>().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(Data.Startup).Assembly).Where(o => o.Name.EndsWith("Repository"))
                .AsImplementedInterfaces().InstancePerDependency().EnableInterfaceInterceptors().InterceptedBy(typeof(LogInterceptor));
        }
    }
}
