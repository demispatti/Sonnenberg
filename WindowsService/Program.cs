using System;
using System.ServiceProcess;

namespace Sonnenberg.WindowsService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new Service()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}