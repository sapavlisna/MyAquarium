using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aquarium.Log;
using Autofac;

namespace Aquarium
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main()
        {
            try
            {
                while (true)
                {
                    
                    Console.WriteLine("Start");
                    SetupAutofac();
                    using (var autofacScope = Container.BeginLifetimeScope())
                    {
                        var configManager = autofacScope.Resolve<IConfigManager>();

                        var logger = autofacScope.Resolve<ILogger>();
                        logger.LogInfo = configManager.GetConfig().LogInfo;
                        logger.Write("Started", LoggerTypes.LogLevel.Info);

                        Console.WriteLine("Try to use arduino communication");
                        var arduino = autofacScope.Resolve<IArduinoComunication>();
                        arduino.FindSerialPort();

                        if (!arduino.IsConnected)
                        {
                            Console.WriteLine("Not connected Arduino");
                            //Logger.Write("Arduino not connected", LoggerTypes.LogLevel.Info);
                            Thread.Sleep(1000 * 60 * 10);
                            continue;
                        }

                        var lights = autofacScope.Resolve<ILightManager>();
                        Task.Run(() => lights.Run());
                        break;

                    }
                }
                
                //TODO Wait to don't kill inner threads better way
                Console.WriteLine("Waiting");
                Console.ReadKey();
                Console.WriteLine("END1");

                while (true)
                {
                    Thread.Sleep(1000 * 60 * 30);
                }
            }
            catch (Exception e)
            {
                //logger.Write(e);
                Console.WriteLine(e.Message);

            }

        }

        static void SetupAutofac()
        {
            //Logger.Write("Setup Autofac", LoggerTypes.LogLevel.Info);
            Console.WriteLine("Setup Autofac");
            var builder = new ContainerBuilder();
            builder.RegisterType<ArduinoComunication>().As<IArduinoComunication>().SingleInstance();
            builder.RegisterType<ConfigManager>().As<IConfigManager>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<LightManager>().As<ILightManager>().SingleInstance();


            Container = builder.Build();
            //Logger.Write("Setup Autofac completed", LoggerTypes.LogLevel.Info);
            Console.WriteLine("Autofac setup completed");
        }
    }
}
