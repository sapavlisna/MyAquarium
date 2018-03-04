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
        private static MainController mainController;
        static void Main()
        {
            try
            {
                mainController = new MainController();
                //TODO Wait to don't kill inner threads better way
                Console.WriteLine("Waiting");
                Console.ReadKey();
                Console.WriteLine("END1");
            }
            catch (Exception e)
            {
                //logger.Write(e);
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }

        }
    }
}
