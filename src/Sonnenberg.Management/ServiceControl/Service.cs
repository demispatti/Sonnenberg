using System.ServiceProcess;

namespace Sonnenberg.Service
{
    /// <summary>
    ///     The Windows Service responsible for starting and stopping of the
    ///     ShellServer trough the Windows Service Manager.
    /// </summary>
    /// <seealso cref="ServiceBase" />
    /// <seealso cref="ServiceManager" />
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Instantiates the Windows Service Manager and initiates starting of the ShellServer.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            new ServiceManager.ServiceManager().StartShellServer();
        }

        /// <summary>
        ///     Instantiates the Windows Service Manager and initiates stopping of the ShellServer.
        /// </summary>
        protected override void OnStop()
        {
            new ServiceManager.ServiceManager().StopShellServer();
        }
    }
}