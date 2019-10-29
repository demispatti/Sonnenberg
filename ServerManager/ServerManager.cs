using System;
using log4net;
using SharpShell;
using SharpShell.ServerRegistration;
using Sonnenberg.Common;
using Sonnenberg.Language;

namespace Sonnenberg.ServerManager
{
    /// <summary>
    /// The class responsible for interacting with the server registration manager.
    /// </summary>
    /// <seealso cref="Logger" />
    /// <seealso cref="ServerRegistrationManager" />
    public class ServerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerManager));

        public ServerManager()
        {
            ConfigureLogger();
        }

        private void ConfigureLogger()
        {
            Logger.Configure();
        }

        /// <summary>
        /// Registers and installs the shell server
        /// in order to add functionality
        /// to the windows explorer context menu.
        /// </summary>
        /// ///
        /// <param name="server"></param>
        private void Start(ISharpShellServer server)
        {
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    ServerRegistrationManager.InstallServer(server, RegistrationType.OS64Bit, true);
                    ServerRegistrationManager.RegisterServer(server, RegistrationType.OS64Bit);
                }
                else
                {
                    ServerRegistrationManager.InstallServer(server, RegistrationType.OS32Bit, true);
                    ServerRegistrationManager.RegisterServer(server, RegistrationType.OS32Bit);
                }

                log.Info(Strings.serviceStarted);
            }
            catch (Exception ex)
            {
                log.Info($"{Strings.startServiceFail} {ex.Message}");

                throw;
            }
        }

        /// <summary>
        /// Unregisters and uninstalls the shell server
        /// in order to remove functionality
        /// from the windows explorer context menu.
        /// </summary>
        /// <param name="server"></param>
        private void Stop(ISharpShellServer server)
        {
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    ServerRegistrationManager.UninstallServer(server, RegistrationType.OS64Bit);
                    ServerRegistrationManager.UnregisterServer(server, RegistrationType.OS64Bit);
                }
                else
                {
                    ServerRegistrationManager.UninstallServer(server, RegistrationType.OS32Bit);
                    ServerRegistrationManager.UnregisterServer(server, RegistrationType.OS32Bit);
                }

                log.Info(Strings.serviceStopped);
            }
            catch (Exception ex)
            {
                log.Info($"{Strings.stopServiceFail} {ex.Message}");

                throw;
            }
        }

        /// <summary>
        /// Calls the method that starts the shell server.
        /// </summary>
        /// <param name="server"></param>
        public void StartShellServer(ISharpShellServer server)
        {
            Start(server);
        }

        /// <summary>
        /// Calls the method that stops the shell server.
        /// </summary>
        /// <param name="server"></param>
        public void StopShellServer(ISharpShellServer server)
        {
            Stop(server);
        }
    }
}