using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using log4net;
using Sonnenberg.Common;
using Sonnenberg.ContextMenu.Properties;
using Sonnenberg.Language;
using Log = log4net.LogManager;

namespace Sonnenberg.ContextMenu.SubMenuItems
{
    /// <summary>
    ///     The class responsible for creating a <c>ToolStripMenuItem</c> and its click action.
    ///     It starts a CMD-, Git-CMD- or Git-Bash-Process and cds into the respective directory.
    ///     The processes can be started with limited or elevated privileges.
    /// </summary>
    /// <remarks>
    ///     - Creates a ToolStripMenuItem
    ///     - Creates a click action
    ///     - Checks that the requested executable is installed
    ///     - Determines the directory where the clicked item resides
    ///     - Determines the directory the CMD process will cd into
    ///     - Assembles the required <c>StartInfo</c> parameters
    ///     - Starts a CMD-, Git-CMD- or Git-Bash-Process and cds into the respective directory
    /// </remarks>
    /// <seealso cref="ContextMenu" />
    /// <seealso cref="Logger" />
    internal class OpenShell : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenShell));
        private bool _disposedValue;

        internal OpenShell()
        {
            ConfigureLogger();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static void ConfigureLogger()
        {
            Logger.Configure();
        }

        internal ToolStripMenuItem ItemDisplay(ToolStripMenuItem toolStripMenuItem, string clickedItemType,
            string targetDirectory, bool isDarkTheme)
        {
            var openShell = new OpenShell();

            if (AppExists("cmd.exe"))
            {
                var openCmdInsideMenuItem =
                    openShell.CreateItem(clickedItemType, targetDirectory, "cmd.exe", false, isDarkTheme);
                toolStripMenuItem.DropDownItems.Add(openCmdInsideMenuItem);

                var openCmdInsideElevatedMenuItem = openShell.CreateItem(clickedItemType, targetDirectory,
                    "cmd.exe", true, isDarkTheme);
                toolStripMenuItem.DropDownItems.Add(openCmdInsideElevatedMenuItem);
            }

            if (AppExists("powershell.exe"))
            {
                var openPowershellInsideMenuItem = openShell.CreateItem(clickedItemType, targetDirectory,
                    "powershell.exe", false, isDarkTheme);
                toolStripMenuItem.DropDownItems.Add(openPowershellInsideMenuItem);

                var openPowershellInsideElevatedMenuItem = openShell.CreateItem(clickedItemType,
                    targetDirectory, "powershell.exe", true, isDarkTheme);
                toolStripMenuItem.DropDownItems.Add(openPowershellInsideElevatedMenuItem);
            }

            openShell.Dispose();

            return toolStripMenuItem;
        }

        private ToolStripMenuItem CreateItem(string clickedItemType, string shellStartUpDirectory,
            string shellExecutableName, bool runElevated, bool isDarkTheme)
        {
            var icon = GetIcon(shellExecutableName, runElevated, isDarkTheme);

            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = GetText(shellExecutableName, clickedItemType, runElevated),
                Image = icon.ToBitmap()
            };

            //  Add click action.
            toolStripMenuItem.Click += (sender, args) =>
                DoClickAction(ProcessStartInfo(shellStartUpDirectory, shellExecutableName, runElevated));
            icon.Dispose();

            return toolStripMenuItem;
        }

        private string GetText(string shellExecutableName, string clickedItemType, bool runElevated)
        {
            string text;

            switch (shellExecutableName)
            {
                case "cmd.exe":
                    text = runElevated ? Strings.openCmdHereElevatedText : Strings.openCmdHereText;

                    if ("Folder" == clickedItemType || "FolderShortcut" == clickedItemType)
                        text = runElevated ? Strings.openCmdInsideElevatedText : Strings.openCmdInsideText;

                    break;

                case "powershell.exe":
                    text = runElevated ? Strings.openPowershellHereElevatedText : Strings.openPowershellHereText;

                    if ("Folder" == clickedItemType || "FolderShortcut" == clickedItemType)
                        text = runElevated
                            ? Strings.openPowershellInsideElevatedText
                            : Strings.openPowershellInsideText;

                    break;

                default:
                    text = runElevated ? Strings.openCmdHereElevatedText : Strings.openCmdHereText;

                    if ("Folder" == clickedItemType || "FolderShortcut" == clickedItemType)
                        text = runElevated ? Strings.openCmdInsideElevatedText : Strings.openCmdInsideText;

                    break;
            }

            return text;
        }

        private Icon GetIcon(string shellExecutableName, bool runElevated, bool isDarkTheme)
        {
            Icon icon;

            switch (shellExecutableName)
            {
                case "cmd.exe":
                    if (isDarkTheme)
                        icon = runElevated ? Resources.Console_Elevated_dark_theme : Resources.Console_dark_theme;
                    else
                        icon = runElevated ? Resources.Console_Elevated_light_theme : Resources.Console_light_theme;

                    break;

                case "powershell.exe":
                    if (isDarkTheme)
                        icon = runElevated ? Resources.Powershell_Elevated_dark_theme : Resources.Powershell_dark_theme;
                    else
                        icon = runElevated
                            ? Resources.Powershell_Elevated_light_theme
                            : Resources.Powershell_light_theme;

                    break;

                default:
                    if (isDarkTheme)
                        icon = runElevated ? Resources.Powershell_Elevated_dark_theme : Resources.Powershell_dark_theme;
                    else
                        icon = runElevated ? Resources.Console_Elevated_light_theme : Resources.Console_light_theme;

                    break;
            }

            return new Icon(icon, 40, 40);
        }

        private ProcessStartInfo ProcessStartInfo(string shellStartUpDirectory, string shellExecutableName,
            bool runElevated)
        {
            var system32DirectoryPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Windows)}\\System32";
            var systemRoot = $"{Environment.GetFolderPath(Environment.SpecialFolder.Windows)}\\..";

            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var userProfileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            try
            {
                switch (shellExecutableName)
                {
                    case "cmd.exe":
                        return new ProcessStartInfo
                        {
                            WorkingDirectory = $"{system32DirectoryPath}",
                            FileName = "cmd.exe",
                            Arguments = $" /K cd {shellStartUpDirectory}",
                            UseShellExecute = true,
                            CreateNoWindow = false,
                            Verb = runElevated ? "runas" : ""
                        };

                    case "powershell.exe":
                        return new ProcessStartInfo
                        {
                            WorkingDirectory = $"{userProfileDirectory}\\",
                            FileName = "powershell.exe",
                            Arguments = $" -ExecutionPolicy Bypass -NoExit cd '{shellStartUpDirectory}';",
                            Verb = runElevated ? "runas" : ""
                        };

                    default:
                        return new ProcessStartInfo
                        {
                            WorkingDirectory = $"{system32DirectoryPath}\\",
                            FileName = "cmd.exe",
                            Arguments = $" /K cd {shellStartUpDirectory}",
                            Verb = runElevated ? "runas" : ""
                        };
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message} | Shell executable: {shellExecutableName} | (OpenShell\\ProcessStartInfo)");

                throw;
            }
        }

        private static void DoClickAction(ProcessStartInfo processStartInfo)
        {
            StartProcess(processStartInfo);
        }

        private static void StartProcess(ProcessStartInfo processStartInfo)
        {
            Process.Start(processStartInfo);
        }

        private static bool AppExists(string appName)
        {
            bool fileExists;
            var systemRoot = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\..";

            try
            {
                switch (appName)
                {
                    case "cmd.exe":
                        fileExists = File.Exists($"{systemRoot}\\Windows\\System32\\cmd.exe");
                        break;

                    case "powershell.exe":
                        fileExists =
                            File.Exists($"{systemRoot}\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe") ||
                            File.Exists($"{systemRoot}\\Windows\\System32\\WindowsPowerShell\\v2.0\\powershell.exe");
                        break;

                    default:
                        fileExists = false;
                        break;
                }

                return fileExists;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message} | App path: {appName} | (OpenShell\\AppExists)");

                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                _disposedValue = true;
                Dispose();
            }
        }
    }
}