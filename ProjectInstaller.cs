using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace IpCheckerService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            serviceInstaller = new ServiceInstaller
            {
                ServiceName = "IpCheckerService",
                DisplayName = "IP Checker Service",
                Description = "Service that checks in with an HTTP endpoint at a given interval to report the current IP address.",
                StartType = ServiceStartMode.Automatic
            };

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
