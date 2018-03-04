using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aquarium;
using Aquarium.Services;
using Autofac;
using Autofac.Integration.WebApi;
using Config.Model.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace API
{
    public class Startup
    {
        public static IContainer Container { get; set; }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };


            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<ConfigManager>().As<IConfigManager>().SingleInstance();
            builder.RegisterType<ArduinoService>().As<IArduinoService>().SingleInstance();
            builder.RegisterType<LightControllService>().As<ILightControllService>().SingleInstance();
            builder.RegisterType<MainController>().As<IMainController>().SingleInstance();
            builder.RegisterType<LightIntensityService>().As<ILightIntensityService>().SingleInstance();
            builder.RegisterType<SurfaceService>().As<ISurfaceService>().SingleInstance();


            var container = builder.Build();
            Container = container;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);


            
        }
    }
}
