using System;
using System.IO;
using System.Linq;
using IWshRuntimeLibrary;
using SharpShell;
using log4net;
using Sonnenberg.Language;
using Sonnenberg.ShellServer.Properties;
using File = System.IO.File;

namespace Sonnenberg.ShellServer
{
    internal class Helper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Helper));

        internal static void SetSettings(Program shellServer)
        {
            if (Settings.Default.hasClickedItemType)
            {
                return;
            }
            
            var clickedItemType = ClickedItemType(shellServer);
            var clickedItemPath = ClickedItemPath(clickedItemType, shellServer);
            var ext = Path.GetExtension(clickedItemPath);

            Settings.Default.clickedItemType = ClickedItemType(shellServer);
            Settings.Default.clickedItemPath = ClickedItemPath(clickedItemType, shellServer);
            Settings.Default.clickedItemContainingFolder = ClickedItemContainingFolder(clickedItemType, shellServer);
            
            if ("Folder" == clickedItemType)
            {
                Settings.Default.shellStartUpDirectory = ShellStartUpDirectory(clickedItemType, shellServer);
            }

            if (".lnk" == ext)
            {
                Settings.Default.shortcutTarget = ShortcutTarget(clickedItemPath);
                var targetType = Settings.Default.shortcutTargetType = ShortcutTargetType(clickedItemPath);
                Settings.Default.shortcutTargetFolder = ShortcutTargetFolder(clickedItemPath, targetType);
            }
            
            Settings.Default.hasClickedItemType = true;
        }
        
        internal static void ResetSettings()
        {
            Settings.Default.hasClickedItemType = false;
            Settings.Default.clickedItemType = "";
            Settings.Default.clickedItemPath = "";
            Settings.Default.clickedItemContainingFolder = "";
            Settings.Default.shellStartUpDirectory = "";
            Settings.Default.shortcutTarget = "";
            Settings.Default.shortcutTargetFolder = "";
            Settings.Default.shortcutTargetType = "";
            Settings.Default.hasMenuWatcherSubscribed = false;
        }
        
        private static string ShortcutTarget(string clickedItemPath)
        {
            var wsh = new WshShellClass();
            var sc = (IWshShortcut) wsh.CreateShortcut(clickedItemPath);
            
            return sc.TargetPath;
        }
        
        private static string ShortcutTargetFolder(string clickedItemPath, string targetType)
        {
            var wsh = new WshShellClass();
            var sc = (IWshShortcut) wsh.CreateShortcut(clickedItemPath);
            
            return "Folder" == targetType ? ShortcutTarget(clickedItemPath) : Path.GetDirectoryName(sc.TargetPath);
        }

        private static string ShortcutTargetType(string clickedItemPath)
        {
            var shortcutTarget = ShortcutTarget(clickedItemPath);
            var fileAttributes = File.GetAttributes(shortcutTarget);
            
            return (fileAttributes & FileAttributes.Directory) != 0 ? "Folder" : "File";
        }

        private static string ClickedItemType(ShellExtInitServer shellServer)
        {
            if (shellServer.FolderPath != null)
            {
                return "Directory";
            }
            
            var clickedItemType = "Unsupported";
            var clickedItemPath = shellServer.SelectedItemPaths.First();
            if (null == clickedItemPath)
            {
                throw new ArgumentNullException(clickedItemType, Strings.clickedItemPathArgumentNullException);
            }
            
            try
            {
                var ext = Path.GetExtension(clickedItemPath);
                var fileAttributes = File.GetAttributes(clickedItemPath);

                if (".lnk" != ext)
                {
                    clickedItemType = (fileAttributes & FileAttributes.Directory) != 0 ? "Folder" : "File";
                }
                else
                {
                    clickedItemType = "Folder" == ShortcutTargetType(clickedItemPath) ? "FolderShortcut" : "FileShortcut";
                }

                return clickedItemType;
                
            }
            catch (ArgumentNullException ex)
            {
                log.Error($"{ex.Message} | Path: {clickedItemPath} | (GetClickedItemType)");

                throw;
            }
        }
        
        private static string ClickedItemPath(string menuType, ShellExtInitServer shellServer)
        {
            return "Directory" == menuType ? shellServer.FolderPath : shellServer.SelectedItemPaths.First();
        }

        private static string ShellStartUpDirectory(string clickedItemType, ShellExtInitServer shellServer)
        {
            switch (clickedItemType)
            {
                case "FolderShortcut":

                    return ShortcutTarget(ClickedItemPath(clickedItemType, (Program) shellServer));

                case "Folder":

                    return shellServer.SelectedItemPaths.First();

                default:

                    return "";
            }
        }

        private static string ClickedItemContainingFolder(string clickedItemType, ShellExtInitServer shellServer)
        {
            return "Directory" == clickedItemType ? Path.GetDirectoryName(shellServer.FolderPath) : Path.GetDirectoryName(shellServer.SelectedItemPaths.First());
        }
    }
}
