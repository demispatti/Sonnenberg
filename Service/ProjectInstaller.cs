using System.ComponentModel;
using System.Configuration.Install;

namespace Sonnenberg.Service
{
    [RunInstaller(true)]
    internal partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void ServiceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
        }
    }
}