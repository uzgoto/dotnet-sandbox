using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.Dotnet.Sandbox.NotifyService
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                var service = new NotifyService(new Log(Log.Name.Console));

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
                        case "/d":
                            // Interactive mode.
                            service.OnStartByConsole(args);
                            Console.WriteLine("Press any keys to stop service.");
                            Console.ReadKey();
                            service.OnStopByConsole();
                            return;
                        default:
                            break;
                    }
                }
            }
            // Service mode.
            ServiceBase.Run(new NotifyService(new Log(Log.Name.Service)));
        }

        private static void CallInstaller(string option = "")
        {
            var path = Assembly.GetExecutingAssembly().Location;
            ManagedInstallerClass.InstallHelper(
                string.IsNullOrEmpty(option)
                ? new[] { "/LogFile=", path }
                : new[] { option, "/LogFile=", path });
        }

        private static bool IsInstalled(NotifyService service)
        {
            return
                ServiceController.GetServices()
                    .Any(s => s.ServiceName == service.ServiceName);
        }
    }
}
