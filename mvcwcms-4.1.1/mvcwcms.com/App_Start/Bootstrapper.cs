using Autofac;
using Autofac.Integration.Mvc;
using MVCwCMS.Infrastructure;
using MVCwCMS.Mappings;
using MVCwCMS.Repositories;
using MVCwCMS.Services;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace MVCwCMS.App_Start
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            //Set Autofac to Dependency Injection
            SetAutofacContainer();

            //Configure AutoMapper
            AutoMapperConfiguration.Configure();
        }

        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<DbFactory>().As<IDbFactory>().InstancePerRequest();

            // Repositories
            builder.RegisterAssemblyTypes(typeof(GadgetRepository).Assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces().InstancePerRequest();

            // Services
            builder.RegisterAssemblyTypes(typeof(GadgetService).Assembly)
               .Where(t => t.Name.EndsWith("Service"))
               .AsImplementedInterfaces().InstancePerRequest();

            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}