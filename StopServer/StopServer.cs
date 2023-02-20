using log4net;
using Sonnenberg.Common;
using System;
using System.Diagnostics;
using Strings = Sonnenberg.Language.Strings;

namespace Sonnenberg.StopServer
{
    internal class StopServer
    {
        private static void Main(string[] args)
        {
            var stopServer = new StopServer();
            stopServer.Stop();
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(StopServer));

        private void Stop()
        {
            ConfigureLogger();
            this.StopShellServer();
        }

        private static void ConfigureLogger()
        {
            Logger.Configure();
        }

        private void StopShellServer()
        {
            try
            {
                foreach (Process exe in Process.GetProcesses())
                {
                    if (exe.ProcessName == "explorer")
                        exe.Kill();
                }

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