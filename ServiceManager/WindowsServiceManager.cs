using System;
using log4net;
using Sonnenberg.Language;
using System.Windows.Forms;

namespace Sonnenberg.ServiceManager
{
    /// <summary>
    /// 	The class responsible for messaging starting and stopping commands
    /// 	invoked by the WindowsService and executed by the ShellServerManager.
    /// </summary>
    /// <seealso cref="ServerManager.ShellServerManager"/>
    /// <seealso cref="ShellServer.ShellServer"/>
    public class WindowsServiceManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WindowsServiceManager));

        /// <summary>
        /// 	Instantiates the ShellServerManager and creates an Instance of the ShellServer
        /// 	to start the ShellServer via ShellServerManager.
        /// </summary>
        /// <seealso cref="ServerManager.ShellServerManager"/>
        /// <seealso cref="ShellServer.ShellServer"/>
        public void StartShellServer()
        {
            var shellServerManager = new ServerManager.ShellServerManager();

            using (var shellServer = new ShellServer.ShellServer())
            {
                try
                {
                    shellServerManager.StartShellServer(shellServer);
                }
                catch (Exception ex)
                {
                    log.Info(Strings.startServiceFail);
                    MessageBox.Show($"{Strings.startServiceFail} {ex.Message}");

                    throw;
                }
            }
        }

        /// <summary>
        /// 	Instantiates the ShellServerManager and creates an Instance of the ShellServer
        /// 	to stop the ShellServer via ShellServerManager.
        /// </summary>
        /// <seealso cref="ServerManager.ShellServerManager" />
        /// <seealso cref="ShellServer.ShellServer"/>
        public void StopShellServer()
        {
            var shellServerManager = new ServerManager.ShellServerManager();

            using (var shellServer = new ShellServer.ShellServer())
            {
                try
                {
                    shellServerManager.StopShellServer(shellServer);
                }
                catch (Exception ex)
                {
                    log.Info(Strings.startServiceSuccess);
                    MessageBox.Show($"{Strings.startServiceSuccess} {ex.Message}");

                    throw;
                }
            }
        }
    }
}