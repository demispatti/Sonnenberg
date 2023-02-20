using log4net;
using Sonnenberg.Common;
using System;
using System.Diagnostics;
using Strings = Sonnenberg.Language.Strings;

namespace Sonnenberg.StartServer
{
    internal class StartServer
    {
        private static void Main(string[] args)
        {
            Start();
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(StartServer));

        private static void Start()
        {
            ConfigureLogger();
            StartShellServer();
        }

        private static void ConfigureLogger()
        {
            Logger.Configure();
        }

        private static void StartShellServer()
        {
            try
            {
                Process p = new Process();
                foreach (Process exe in Process.GetProcesses())
                {
                    if (exe.ProcessName == "explorer")
                        exe.Kill();
                }

                Process.Start("explorer.exe");
                new ServiceManager.ServiceManager().StartShellServer();
            }
            catch (Exception ex)
            {
                var message = $"{Strings.startServiceFail}";
                Log.Error($"{message} ({ex.Message})");
                //MessageBox.Show($"{message} ({ex.Message})");
                throw ex;
            }
        }
    }
}