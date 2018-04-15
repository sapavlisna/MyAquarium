using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aquarium;
using Autofac;
using Microsoft.Owin.Hosting;

namespace API
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/";

            //var autofac = new AutofacSetup(Container);
            //Container = autofac.Setup();
            

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                var mainController = Startup.Container.Resolve<IMainController>();

                Thread.Sleep(5000);

                //var client = new HttpClient();

                //var response = client.PostAsync(baseAddress + "api/Lights", new ByteArrayContent(new byte[2])).Result;

                //Console.WriteLine(response);
                //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                //Console.ReadLine();

                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
