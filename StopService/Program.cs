using System;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using log4net;
using Sonnenberg.Common;
using Sonnenberg.Language;

namespace Sonnenberg.StopService
{
    /// <summary>
    /// The executable that initializes stopping of the shell server.
    /// </summary>
    /// <seealso cref="Logger" />
    internal class Program
    {
        private const string ServiceName = "SonnenbergService";

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        [STAThread]
        private static void Main()
        {
            ConfigureLogger();
            Stop();
			Notify();
        }

        private static void ConfigureLogger()
        {
            Logger.Configure();
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <seealso cref="ServiceController" />
        private static void Stop()
        {
            using (var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == ServiceName))
            {
                try
                {
                    if (null == service)
                    {
                        throw new ArgumentException(Strings.stopServiceError);
                    }
                    
                    if ("Stopped" == service.Status.ToString())
                    {
                        MessageBox.Show(Strings.serviceStoppedAlready);

                        return;
                    }

                    var timeout = TimeSpan.FromMilliseconds(2000);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
                catch (NullReferenceException ex)
                {
                    var message = $"{Strings.stopServiceError} ({ex.Message})";
                    log.Error(message);
                    MessageBox.Show(message);

                    throw;
                }
                catch (ArgumentException ex)
                {
                    var message = $"{Strings.stopServiceError} ({ex.Message})";
                    log.Error(message);
                    MessageBox.Show(message);

                    throw;
                }
            }
        }

		private static void Notify()
		{
			MessageBox.Show(Strings.stopServiceSuccess);
		}
    }
}