using System;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using log4net;
using Sonnenberg.Common;
using Sonnenberg.Language;

namespace Sonnenberg.StartService
{
    /// <summary>
    /// The executable that initializes starting of the shell server.
    /// </summary>
    internal class Program
    {
        private const string ServiceName = "SonnenbergService";

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// Triggers the logger class to create said files.
        /// </summary>
        /// ///
        /// <seealso cref="Logger" />
        private static void SetLogFile()
        {
            Logger.SetLogFile();
        }

        [STAThread]
        private static void Main()
        {
            SetLogFile();
            ConfigureLogger();
            Start();
			Notify();
        }

        private static void ConfigureLogger()
        {
            Logger.Configure();
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        /// ///
        /// <seealso cref="ServiceController" />
        private static void Start()
        {
            using (var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == ServiceName))
            {
                try
                {
                    if (null != service && "Running" == service.Status.ToString())
                    {
                        MessageBox.Show(Strings.serviceStartedAlready);

                        return;
                    }

                    var timeout = TimeSpan.FromMilliseconds(2000);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                catch (NullReferenceException ex)
                {
                    log.Error($"{Strings.failedToStartApplicationError} ({ex.Message})");
                    MessageBox.Show($"{Strings.failedToStartApplicationError} ({ex.Message})");

                    throw;
                }
                catch (ArgumentException ex)
                {
                    log.Error($"{Strings.failedToStartApplicationError} ({ex.Message})");
                    MessageBox.Show($"{Strings.failedToStartApplicationError} ({ex.Message})");

                    throw;
                }
            }
        }

		private static void Notify()
		{
			MessageBox.Show(Strings.startServiceSuccess);
		}
	}
}