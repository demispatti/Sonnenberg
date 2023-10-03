using System;
using System.Diagnostics;
using log4net;
using Sonnenberg.Common;
using Sonnenberg.Language;

namespace Sonnenberg.StopServer
{
    internal class StopServer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StopServer));

        private static void Main(string[] args)
        {
            var stopServer = new StopServer();
            stopServer.Stop();
        }

        private void Stop()
        {
            ConfigureLogger();
            StopShellServer();
        }

        private static void ConfigureLogger()
        {
            Logger.Configure();
        }

        private void StopShellServer()
        {
            try
            {
                foreach (var exe in Process.GetProcesses())
                    if (exe.ProcessName == "explorer")
                        exe.Kill();

                Process.Start("explorer.exe");
                new ServiceManager.ServiceManager().StopShellServer();
            }
            catch (Exception ex)
            {
                var message = $"{Strings.stopServiceFail}";
                Log.Error($"{message} ({ex.Message})");
                //MessageBox.Show($"{message} ({ex.Message})");
                throw ex;
            }
        }
    }
}