using System.ComponentModel;
using System.Configuration.Install;

namespace Sonnenberg.WindowsService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
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