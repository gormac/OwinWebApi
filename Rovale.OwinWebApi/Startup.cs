using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Owin;
using Rovale.OwinWebApi;
using Rovale.OwinWebApi.Controllers;

[assembly: OwinStartup(typeof(Startup))]

namespace Rovale.OwinWebApi
{
    public class Startup
    {
        private readonly IContainer _container;

        public Startup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.Register(c => new ValuesProvider()).As<IValuesProvider>();
            _container = builder.Build();
        }

        public Startup Using(IValuesProvider valuesProvider)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(valuesProvider).As<IValuesProvider>();
            builder.Update(_container);
            return this;
        }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(_container);

            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }
    }
}


