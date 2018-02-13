using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aquarium;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;

namespace AquaApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //HttpConfiguration config = new HttpConfiguration();
            ////config.Routes.MapHttpRoute("Default", "{controller}", new { controller = "Pwm"});
            //config.Routes.MapHttpRoute( 
            //    name: "DefaultApi", 
            //    routeTemplate: "api/{controller}/{id}", 
            //    defaults: new { id = RouteParameter.Optional } 
            //);

            ////config.Formatters.XmlFormatter.UseXmlSerializer = true;
            ////config.Formatters.Remove(config.Formatters.JsonFormatter);
            ////config.Formatters.JsonFormatter.UseDataContractJsonSerializer = true;

            //app.UseWebApi(config);



            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            



            //autofac
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());



            builder.RegisterType<ArduinoComunication>().As<IArduinoComunication>().SingleInstance();
            builder.RegisterType<Pwm>().As<IPwm>();




            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            //app.UseAutofacMiddleware(container);
            //app.useautofacwebapi(config);
            app.UseWebApi(config);
        }
    }
}
