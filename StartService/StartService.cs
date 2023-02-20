using log4net;
using Sonnenberg.Common;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using Log = log4net.LogManager;
using Strings = Sonnenberg.Language.Strings;

namespace Sonnenberg.StartService
{
    /// <summary>
    /// The executable that initializes starting of the shell server.
    /// </summary>
    public class StartService
    {
        private const string ServiceName = "SonnenbergService";

        private static readonly ILog Log = LogManager.GetLogger(typeof(StartService));

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
                    if (null == service)
                    {
                        throw new ArgumentException(Strings.startServiceError);
                    }

                    if ("Running" == service.Status.ToString())
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
                    var message = $"{Strings.startServiceError} ({ex.Message})";
                    Log.Error(message);
                    MessageBox.Show(message);

                    throw;
                }
                catch (ArgumentException ex)
                {
                    var message = $"{Strings.startServiceError} ({ex.Message})";
                    Log.Error(message);
                    MessageBox.Show(message);

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