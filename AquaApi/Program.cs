using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Aquarium;
using Autofac;
using Autofac.Core;
using Microsoft.Build.Utilities;
using Microsoft.Owin.Hosting;

namespace AquaApi
{
    class Program
    {
        private static IContainer Container { get; set; }
        private static string baseAddress;

        static void Main(string[] args)
        {
            baseAddress = $"http://{GetLocalIpAddress()}:5000/";
            SetupAutoFac();

            Console.WriteLine("setup owin");

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("Owin running");
                // Create HttpCient and make a request to api/values 
                HttpClient client = new HttpClient();


                Console.WriteLine(baseAddress + "api/values");
                var response = client.GetAsync(baseAddress + "api/values").Result;

                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);

                var response2 = client.GetAsync(baseAddress + "api/pwm").Result;
                Console.WriteLine(response2);
                Console.WriteLine(response2.Content.ReadAsStringAsync().Result);

                Console.ReadLine();
            }
        }

        private static void SetupAutoFac()
        {
            
        }

        private static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

    }
}
