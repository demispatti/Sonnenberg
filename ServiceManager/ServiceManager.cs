using log4net;
using SharpShell;
using System;
using System.Windows.Forms;
using Log = log4net.LogManager;
using Strings = Sonnenberg.Language.Strings;

namespace Sonnenberg.ServiceManager
{
    /// <summary>
    /// The class responsible for messaging starting and stopping commands
    /// invoked by the WindowsService and executed by the ShellServerManager.
    /// </summary>
    /// <seealso cref="ServerManager"/>
    /// <seealso cref="ShellServer"/>
    public sealed class ServiceManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServiceManager));

        /// <summary>
        /// Instantiates the ShellServerManager and creates an Instance of the ShellServer
        /// to start the ShellServer via ShellServerManager.
        /// </summary>
        /// <seealso cref="ServerManager"/>
        /// <seealso cref="ShellServer"/>
        public void StartShellServer()
        {
            var shellServer = new ShellServer.ShellServer();
            try
            {
                new ServerManager.ServerManager().StartShellServer((ISharpShellServer)shellServer);
                shellServer.Dispose();
            }
            catch (Exception ex)
            {
                Log.Info(Strings.startServiceFail);
                MessageBox.Show($"{Strings.startServiceFail} ({ex.Message})");

                throw;
            }
        }

        /// <summary>
        /// Instantiates the ShellServerManager and creates an Instance of the ShellServer
        /// to stop the ShellServer via ShellServerManager.
        /// </summary>
        /// <seealso cref="ServerManager" />
        /// <seealso cref="ShellServer"/>
        public void StopShellServer()
        {
            var serverManager = new ServerManager.ServerManager();
            var shellServer = new ShellServer.ShellServer();
            try
            {
                new ServerManager.ServerManager().StopShellServer(shellServer);
                shellServer.Dispose();
            }
            catch (Exception ex)
            {
                Log.Info(Strings.startServiceSuccess);
                MessageBox.Show($"{Strings.startServiceSuccess} ({ex.Message})");
                throw;
            }
        }
    }
}