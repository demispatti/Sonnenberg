using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace Sonnenberg.Infrastructure.Logging
{
    /// <summary>
    /// Configures the log4net logger for the application
    /// </summary>
    public static class LoggerConfigurator
    {
        private static bool _isConfigured;

        public static void Configure()
        {
            if (_isConfigured)
                return;

            var logFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Sonnenberg",
                "Logs",
                "Sonnenberg.log");

            var logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var repository = LogManager.GetRepository();
            var layout = new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
            };
            layout.ActivateOptions();

            var appender = new RollingFileAppender
            {
                File = logFilePath,
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "10MB",
                Layout = layout,
                Threshold = Level.Info
            };
            appender.ActivateOptions();

            BasicConfigurator.Configure(repository, appender);
            _isConfigured = true;
        }
    }
}
