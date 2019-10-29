﻿using System.ServiceProcess;
using Sonnenberg.ServiceManager;

namespace Sonnenberg.WindowsService
{
    /// <summary>
    ///     The Windows Service responsible for starting and stopping of the
    ///     ShellServer trough the Windows Service Manager.
    /// </summary>
    /// <seealso cref="ServiceBase" />
    /// <seealso cref="WindowsServiceManager" />
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
            new WindowsServiceManager().StartShellServer();
        }

        /// <summary>
        ///     Instantiates the Windows Service Manager and initiates stopping of the ShellServer.
        /// </summary>
        protected override void OnStop()
        {
            new WindowsServiceManager().StopShellServer();
        }
    }
}