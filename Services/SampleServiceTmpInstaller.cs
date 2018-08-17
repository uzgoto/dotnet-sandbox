using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.Services
{
    [RunInstaller(true)]
    public partial class SampleServiceTmpInstaller : Installer
    {
        public SampleServiceTmpInstaller()
        {
            InitializeComponent();

            var installer = new ServiceInstaller();
            installer.DelayedAutoStart = true;
            installer.Description = "Description of service.";
            installer.DisplayName = "SampleService";
            installer.ServiceName = "SampleService";
            installer.StartType = ServiceStartMode.Automatic;

            var processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;

            this.Installers.Add(installer);
            this.Installers.Add(processInstaller);
        }
    }
}
