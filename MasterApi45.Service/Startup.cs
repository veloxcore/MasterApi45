using Autofac;
using Autofac.Extras.DynamicProxy2;
using MasterApi45.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Service
{
    public class Startup
    {
        public static void RegisterDependency(ContainerBuilder builder)
        {

            builder.RegisterAssemblyTypes(typeof(Startup).Assembly).Where(o => o.Name.EndsWith("Service"))
                .AsImplementedInterfaces().InstancePerDependency().EnableInterfaceInterceptors().InterceptedBy(typeof(LogInterceptor));

            MasterApi45.Data.Startup.RegisterDependency(builder);
        }
    }
}
