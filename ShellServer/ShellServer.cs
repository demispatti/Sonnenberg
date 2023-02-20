using IWshRuntimeLibrary;
using log4net;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using Sonnenberg.Common;
using Sonnenberg.Settings.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using File = System.IO.File;
using Log = log4net.LogManager;

namespace Sonnenberg.ShellServer
{
    /// <summary>
    /// This is "Sonnenberg". It's the application's main class.
    /// It instantiates the <c>Menu</c>.
    /// This is the class that derives from <c>SharpContextMenu</c>.
    /// </summary>
    /// <seealso cref="IDisposable" />
    /// <seealso cref="SharpContextMenu" />
    /// <seealso cref="ContextMenu" />
    /// <seealso cref="Logger" />
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Class, @"Desktop\Background")]
    [COMServerAssociation(AssociationType.Class, @"Directory\Background")]
    [COMServerAssociation(AssociationType.Drive)]
    [COMServerAssociation(AssociationType.Directory)]
    [COMServerAssociation(AssociationType.AllFilesAndFolders)]
    public class ShellServer : SharpContextMenu, IDisposable
    {
        private new static readonly ILog Log = LogManager.GetLogger(typeof(ShellServer));

        public ContextMenu.ContextMenu ContextMenu;

        public MenuWatcher.MenuWatcher MenuWatcher;

        private bool _disposedValue;

        public ShellServer()
        {
            ConfigureLogger();
            SetContextMenu();
            SetMenuWatcher();
        }
        
        private void SetContextMenu()
        {
            ContextMenu = new ContextMenu.ContextMenu();
        }

        private void ConfigureLogger()
        {
            Logger.Configure();
        }

        private void SetMenuWatcher()
        {
            MenuWatcher = new MenuWatcher.MenuWatcher();
        }

        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            SetSettings();
            bool isDarkMode = new Helper().IsDarkMode();

            // Start watching for user action for when the context menu is open aud prevent multiple calls to Menu Watcher
            if (true != UserSettings.Default.hasMenuWatcherSubscribed)
            {
                MenuWatcher.Subscribe();
                UserSettings.Default.hasMenuWatcherSubscribed = true;
            }

            var clickedItemType = UserSettings.Default.clickedItemType;
            var clickedItemPath = UserSettings.Default.clickedItemPath;
            var shellStartUpDirectory = UserSettings.Default.shellStartUpDirectory;
            var shortcutTargetFolder = UserSettings.Default.shortcutTargetFolder;
            var selectedItemPaths = this.GetSelectedItemPaths();
            var selectedItemPath = 0 != selectedItemPaths.Count() ? GetSelectedItemPaths().First() : "";

            try
            {
                // Return the appropriate menu
                switch (clickedItemType)
                {
                    case "FileShortcut":
                        return ContextMenu.GetFileShortcutMenu(clickedItemType, clickedItemPath, selectedItemPath,
                            shortcutTargetFolder, isDarkMode);

                    case "FolderShortcut":
                        return ContextMenu.GetFolderShortcutMenu(clickedItemType, clickedItemPath, shortcutTargetFolder, isDarkMode);

                    case "Directory":
                        return ContextMenu.GetDirectoryMenu(clickedItemType, clickedItemPath, clickedItemPath,
                            isDarkMode);

                    case "Folder":
                        return ContextMenu.GetFolderMenu(clickedItemType, clickedItemPath, shellStartUpDirectory,
                            isDarkMode);

                    case "File":
                        return ContextMenu.GetFileMenu(clickedItemType, clickedItemPath, selectedItemPath, isDarkMode);

                    default:
                        return new ContextMenuStrip {Name = "Sonnenberg"};
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message} | Shell executable: {clickedItemType} | (ShellServer\\ContextMenuStrip)");

                throw;
            }
        }

        internal void SetSettings()
        {
            if (UserSettings.Default.hasClickedItemType)
            {
                return;
            }

            var clickedItemType = GetClickedItemType();
            var clickedItemPath = GetClickedItemPath(clickedItemType);
            var ext = Path.GetExtension(clickedItemPath);

            UserSettings.Default.clickedItemType = GetClickedItemType();
            UserSettings.Default.clickedItemPath = GetClickedItemPath(clickedItemType);
            UserSettings.Default.clickedItemContainingFolder = GetContainingDirectory(clickedItemType);

            if ("Folder" == clickedItemType)
            {
                UserSettings.Default.shellStartUpDirectory = ShellStartUpDirectory(clickedItemType);
            }

            if (".lnk" == ext)
            {
                UserSettings.Default.shortcutTarget = GetShortcutTargetPath(clickedItemPath);
                var targetType = UserSettings.Default.shortcutTargetType = GetShortcutTargetType(clickedItemPath);
                UserSettings.Default.shortcutTargetFolder = GetShortcutTargetContainingFolder(clickedItemPath, targetType);
            }

            UserSettings.Default.hasClickedItemType = true;
        }

        public IEnumerable<string> GetSelectedItemPaths()
        {
            return null != SelectedItemPaths && 0 != SelectedItemPaths.Count() ? SelectedItemPaths : new List<string>();
        }

        private static string GetShortcutTargetPath(string clickedItemPath)
        {
            var wsh = new WshShellClass();
            var sc = (IWshShortcut)wsh.CreateShortcut(clickedItemPath);

            return sc.TargetPath;
        }

        private static string GetShortcutTargetContainingFolder(string clickedItemPath, string targetType)
        {
            var wsh = new WshShellClass();
            var sc = (IWshShortcut)wsh.CreateShortcut(clickedItemPath);

            return "Folder" == targetType ? GetShortcutTargetPath(clickedItemPath) : Path.GetDirectoryName(sc.TargetPath);
        }

        private static string GetShortcutTargetType(string clickedItemPath)
        {
            var shortcutTarget = GetShortcutTargetPath(clickedItemPath);
            if (".lnk" == Path.GetExtension(shortcutTarget))
            {
                return "link";
            }

            var fileAttributes = File.GetAttributes(shortcutTarget);

            return (fileAttributes & FileAttributes.Directory) != 0 ? "Folder" : "File";
        }

        private string GetClickedItemType()
        {
            if (FolderPath != null)
            {
                return "Directory";
            }

            var clickedItemType = "";
            var clickedItemPath = this.SelectedItemPaths.First();

            try
            {
                var ext = Path.GetExtension(clickedItemPath);
                var fileAttributes = File.GetAttributes(clickedItemPath);

                // @todo: Works in the debugger, but fails for shortcuts in release mode...
                if (".lnk" != ext)
                {
                    clickedItemType = (fileAttributes & FileAttributes.Directory) != 0 ? "Folder" : "File";
                }
                else
                {
                    clickedItemType = "Folder" == GetShortcutTargetType(clickedItemPath)
                        ? "FolderShortcut"
                        : "FileShortcut";
                }

                return clickedItemType;
            }
            catch (ArgumentNullException ex)
            {
                Log.Error($"{ex.Message} | Path: {clickedItemPath} | (GetClickedItemType)");

                throw;
            }
        }

        private string GetClickedItemPath(string clickedItemType)
        {
            return "Directory" == clickedItemType ? this.FolderPath : this.SelectedItemPaths.First();
        }

        private string ShellStartUpDirectory(string clickedItemType)
        {
            switch (clickedItemType)
            {
                case "FolderShortcut":

                    return GetShortcutTargetPath(GetClickedItemPath(clickedItemType));

                case "Folder":

                    return SelectedItemPaths.First();

                default:

                    return "";
            }
        }

        private string GetContainingDirectory(string clickedItemType)
        {
            return "Directory" == clickedItemType
                ? Path.GetDirectoryName(this.FolderPath)
                : Path.GetDirectoryName(this.SelectedItemPaths.First());
        }

        internal static void ResetSettings()
        {
            UserSettings.Default.hasClickedItemType = false;
            UserSettings.Default.clickedItemType = "";
            UserSettings.Default.clickedItemPath = "";
            UserSettings.Default.clickedItemContainingFolder = "";
            UserSettings.Default.shellStartUpDirectory = "";
            UserSettings.Default.shortcutTarget = "";
            UserSettings.Default.shortcutTargetFolder = "";
            UserSettings.Default.shortcutTargetType = "";
            UserSettings.Default.hasMenuWatcherSubscribed = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing) ContextMenu.Dispose();

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}