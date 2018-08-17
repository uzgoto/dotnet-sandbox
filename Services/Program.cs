using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.Services
{
    static class Program
    {
        static void Main(string[] args)
        {
            var service = new SampleService();

            if (!Environment.UserInteractive)
            {
                // Service mode.
                ServiceBase.Run(service);
                return;
            }

            if (args.Length == 1)
            {
                // Install mode.
                switch (args[0].ToLower())
                {
                    case "/i":
                        if (!IsInstalled(service))
                        {
                            CallInstaller();
                            return;
                        }
                        Console.WriteLine($"Service {service.ServiceName} is already installed.");
                        return;
                    case "/u":
                        if (IsInstalled(service))
                        {
                            CallInstaller(option: args[0]);
                            return;
                        }
                        Console.WriteLine($"Service {service.ServiceName} is not installed.");
                        return;
                    default:
                        break;
                }
            }

            // Interactive mode.
            service.OnStartByConsole(args);
            Console.WriteLine("Press any keys to stop service.");
            Console.ReadKey();
            service.OnStopByConsole();
        }

        private static void CallInstaller(string option = "")
        {
            var path = Assembly.GetExecutingAssembly().Location;
            ManagedInstallerClass.InstallHelper(new[] { option, path });
        }

        private static bool IsInstalled(SampleService service)
        {
            return
                ServiceController.GetServices()
                    .Any(s => s.ServiceName == service.ServiceName);
        }
    }
}
