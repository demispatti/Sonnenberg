using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using log4net;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using Sonnenberg.Common;
using Sonnenberg.ShellServer.MouseKeyHook;
using Sonnenberg.ShellServer.Properties;

namespace Sonnenberg.ShellServer
{
	/// <summary>
	/// This is "Sonnenberg". It's the application's main class.
	/// It instantiates the <c>ContextMenu</c>.
	/// This is the class that derives from <c>SharpContextMenu</c>.
	/// </summary>
	/// <seealso cref="IDisposable" />
	/// <seealso cref="SharpContextMenu" />
	/// <seealso cref="ContextMenu" />
	/// <seealso cref="Logger" />
	[ComVisible(true)]
    [COMServerAssociation(AssociationType.Class, @"Desktop\Background")]
    [COMServerAssociation(AssociationType.Class, @"Directory\Background")]
    [COMServerAssociation(AssociationType.AllFilesAndFolders)]
    public class Program : SharpContextMenu, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        
        private ContextMenu.Program _contextMenu;
        
        private MenuWatcher _menuWatcher;

        private bool _disposedValue;

        public Program()
        {
            ConfigureLogger();
            SetContextMenu();
            SetMenuWatcher();
        }

        private void SetContextMenu()
        {
			_contextMenu = new ContextMenu.Program();
        }
        
        private void ConfigureLogger()
        {
            Logger.Configure();
        }

        private void SetMenuWatcher()
        {
            _menuWatcher = new MenuWatcher();
        }
        
        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            Helper.SetSettings(this);
            
            var clickedItemType = Settings.Default.clickedItemType;
            var clickedItemPath = Settings.Default.clickedItemPath;
            var shellStartUpDirectory = Settings.Default.shellStartUpDirectory;
            var shortcutTargetFolder = Settings.Default.shortcutTargetFolder;
            
            
            // Start watching for user action for when the context menu is open aud prevent multiple calls to Menu Watcher
            if (true != Settings.Default.hasMenuWatcherSubscribed)
            {
                _menuWatcher.Subscribe();
                Settings.Default.hasMenuWatcherSubscribed = true;
            }

            // Return the appropriate menu
            switch (clickedItemType)
            {
                case "FileShortcut":
                    return _contextMenu.GetFileShortcutMenu(clickedItemType, clickedItemPath, this, shortcutTargetFolder);
                
                case "FolderShortcut":
                    return _contextMenu.GetFolderShortcutMenu(clickedItemType, clickedItemPath, this, shortcutTargetFolder);
                
                case "Directory":
                    return _contextMenu.GetDirectoryMenu(clickedItemType, clickedItemPath, clickedItemPath);

                case "Folder":
                    return _contextMenu.GetFolderMenu(clickedItemType, clickedItemPath, shellStartUpDirectory);

                case "File":
	                return _contextMenu.GetFileMenu(clickedItemType, clickedItemPath, this);

                default:
                    return new ContextMenuStrip{Name = "Sonnenberg"};
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing) _contextMenu.Dispose();

            _disposedValue = true;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}