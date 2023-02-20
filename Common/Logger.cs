using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using Log = log4net.LogManager;

namespace Sonnenberg.Common
{
    /// <summary>
    /// The class responsible for logging functionality.
    /// </summary>
    public class Logger
    {
        private const string ConfigFilename = "app.config";

        private static readonly string Logfile =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Sonnenberg\\Logs\\log.log");

        private static readonly string DirectoryTree =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sonnenberg\\Logs");

        private static readonly ILog Log = LogManager.GetLogger(typeof(Logger));

        /// <summary>
        /// Creates a directory inside the user's app data folder to store the log file,
        /// if the folder does not exist.
        /// </summary>
        private static void AddAppDataDirectory()
        {
            if (Directory.Exists(DirectoryTree))
            {
                return;
            }

            Directory.CreateDirectory(DirectoryTree);
            Log.Info("Successfully created directory structure in user AppData folder.");
        }

        /// <summary>
        /// Creates the log file,
        /// if the folder does not exist.
        /// </summary>ee
        private static void AddLogfile()
        {
            if (File.Exists(Logfile)) return;

            try
            {
                using (File.Create(Logfile))
                {
                    Log.Info($"Successfully created log file ({Logfile})");
                }
            }
            catch (ArgumentException ex)
            {
                Log.Error($"{ex.Message} (Log4Net)");
                throw;
            }
        }

        /// <summary>
        /// Creates a folder structure inside the user's add data directory,
        /// if it does not exist already.
        /// </summary>
        public static void SetLogFile()
        {
            AddAppDataDirectory();
            AddLogfile();
        }

        /// <summary>
        /// Configures the logger.
        /// </summary>
        /// <seealso cref="log4net.Config.XmlConfigurator" />
        public static void Configure()
        {
            try
            {
                var path = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                    throw new FileNotFoundException(), ConfigFilename);
                var configFile = new FileInfo(path);
                XmlConfigurator.ConfigureAndWatch(configFile);
            }
            catch (FileNotFoundException ex)
            {
                Log.Error($"{ex.Message} (Logger)");
                throw;
            }
        }
    }
}