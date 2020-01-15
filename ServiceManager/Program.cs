using System;
using log4net;
using Sonnenberg.Language;
using System.Windows.Forms;

namespace Sonnenberg.ServiceManager
{
    /// <summary>
    /// The class responsible for messaging starting and stopping commands
    /// invoked by the WindowsService and executed by the ShellServerManager.
    /// </summary>
    /// <seealso cref="ServerManager.Program"/>
    /// <seealso cref="ShellServer.Program"/>
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// Instantiates the ShellServerManager and creates an Instance of the ShellServer
        /// to start the ShellServer via ShellServerManager.
        /// </summary>
        /// <seealso cref="ServerManager.Program"/>
        /// <seealso cref="ShellServer.Program"/>
        public void StartShellServer()
        {
            var shellServerManager = new ServerManager.Program();

            using (var shellServer = new ShellServer.Program())
            {
                try
                {
                    shellServerManager.StartShellServer(shellServer);
                }
                catch (Exception ex)
                {
                    log.Info(Strings.startServiceFail);
                    MessageBox.Show($"{Strings.startServiceFail} ({ex.Message})");

                    throw;
                }
            }
        }

        /// <summary>
        /// Instantiates the ShellServerManager and creates an Instance of the ShellServer
        /// to stop the ShellServer via ShellServerManager.
        /// </summary>
        /// <seealso cref="ServerManager.Program" />
        /// <seealso cref="ShellServer.Program"/>
        public void StopShellServer()
        {
            var shellServerManager = new ServerManager.Program();

            using (var shellServer = new ShellServer.Program())
            {
                try
                {
                    shellServerManager.StopShellServer(shellServer);
                }
                catch (Exception ex)
                {
                    log.Info(Strings.startServiceSuccess);
                    MessageBox.Show($"{Strings.startServiceSuccess} ({ex.Message})");

                    throw;
                }
            }
        }
    }
}