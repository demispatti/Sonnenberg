using System;
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
    internal class StopService
    {
        private const string ServiceName = "SonnenbergService";

        private static readonly ILog log = LogManager.GetLogger(typeof(StopService));

        [STAThread]
        private static void Main()
        {
            ConfigureLogger();
            Stop();
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
                    if (null != service && "Stopped" == service.Status.ToString())
                    {
                        MessageBox.Show(Strings.serviceStoppedAlready);

                        return;
                    }

                    var timeout = TimeSpan.FromMilliseconds(2000);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    MessageBox.Show(Strings.stopServiceSuccess);
                }
                catch (NullReferenceException ex)
                {
                    log.Error(Strings.failedToStartApplicationError + $" ({ex.Message})");
                    MessageBox.Show(Strings.failedToStopApplicationError + $" ({ex.Message})");

                    throw;
                }
                catch (ArgumentException ex)
                {
                    log.Error(Strings.failedToStartApplicationError + $" ({ex.Message})");
                    MessageBox.Show(Strings.failedToStopApplicationError + $" ({ex.Message})");

                    throw;
                }
            }
        }
    }
}