using System;
using System.Diagnostics;
using System.IO;
using log4net;

namespace Sonnenberg.Features.ShellLauncher
{
    /// <summary>
    /// Service for launching command-line shells (CMD, PowerShell) in specific directories
    /// </summary>
    public class ShellLauncherService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ShellLauncherService));

        public void LaunchShell(ShellType shellType, string workingDirectory, bool runAsAdministrator = false)
        {
            if (string.IsNullOrWhiteSpace(workingDirectory))
                throw new ArgumentException("Working directory cannot be null or empty", nameof(workingDirectory));

            if (!Directory.Exists(workingDirectory))
                throw new DirectoryNotFoundException($"Directory not found: {workingDirectory}");

            if (!IsShellInstalled(shellType))
                throw new InvalidOperationException($"Shell {shellType} is not installed");

            try
            {
                var startInfo = CreateProcessStartInfo(shellType, workingDirectory, runAsAdministrator);
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to launch {shellType} in directory: {workingDirectory}", ex);
                throw;
            }
        }

        public bool IsShellInstalled(ShellType shellType)
        {
            try
            {
                switch (shellType)
                {
                    case ShellType.CommandPrompt:
                        return File.Exists(GetCmdPath());

                    case ShellType.PowerShell:
                        return File.Exists(GetPowerShellPath());

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to check if shell is installed: {shellType}", ex);
                return false;
            }
        }

        private ProcessStartInfo CreateProcessStartInfo(ShellType shellType, string workingDirectory, bool runAsAdministrator)
        {
            switch (shellType)
            {
                case ShellType.CommandPrompt:
                    return new ProcessStartInfo
                    {
                        WorkingDirectory = GetSystem32Path(),
                        FileName = "cmd.exe",
                        Arguments = $"/K cd /d \"{workingDirectory}\"",
                        UseShellExecute = true,
                        CreateNoWindow = false,
                        Verb = runAsAdministrator ? "runas" : string.Empty
                    };

                case ShellType.PowerShell:
                    return new ProcessStartInfo
                    {
                        WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        FileName = "powershell.exe",
                        Arguments = $"-ExecutionPolicy Bypass -NoExit -Command \"Set-Location '{workingDirectory}'\"",
                        UseShellExecute = true,
                        CreateNoWindow = false,
                        Verb = runAsAdministrator ? "runas" : string.Empty
                    };

                default:
                    throw new ArgumentException($"Unsupported shell type: {shellType}", nameof(shellType));
            }
        }

        private string GetSystem32Path()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32");
        }

        private string GetCmdPath()
        {
            return Path.Combine(GetSystem32Path(), "cmd.exe");
        }

        private string GetPowerShellPath()
        {
            var system32 = GetSystem32Path();
            var ps1 = Path.Combine(system32, "WindowsPowerShell", "v1.0", "powershell.exe");
            var ps2 = Path.Combine(system32, "WindowsPowerShell", "v2.0", "powershell.exe");

            return File.Exists(ps1) ? ps1 : ps2;
        }
    }
}
