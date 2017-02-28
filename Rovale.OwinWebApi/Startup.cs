﻿using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Rovale.OwinWebApi.Providers;

namespace Rovale.OwinWebApi
{
    public class Startup
    {
        private readonly IContainer _container;

        public Startup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.Register(c => new TodoListProvider()).As<ITodoListProvider>().SingleInstance();
            _container = builder.Build();
        }

        public Startup Using(ITodoListProvider todoListProvider)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(todoListProvider).As<ITodoListProvider>();
            builder.Update(_container);
            return this;
        }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SerializerSettings =
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

            config.DependencyResolver = new AutofacWebApiDependencyResolver(_container);

            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }
    }
}


