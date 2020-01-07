using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using log4net;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using Sonnenberg.Common;
using Sonnenberg.FileHelper;

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
    [COMServerAssociation(AssociationType.AllFilesAndFolders)]
    [COMServerAssociation(AssociationType.Class, @"Directory\Background")]
    public class ShellServer : SharpContextMenu, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ShellServer));

        private ContextMenu.ContextMenu _contextMenu;

        private bool _disposedValue;

        public ShellServer()
        {
            ConfigureLogger();
            SetContextMenu();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void SetContextMenu()
        {
			_contextMenu = new ContextMenu.ContextMenu();
        }

        private void ConfigureLogger()
        {
            Logger.Configure();
        }

        protected override bool CanShowMenu()
        {
            // is Folder || is Dir
            var canShowMenu = FolderPath != null || SelectedItemPaths.Count() == 1;
            var menuType = Helper.GetClickedItemType(this);

            if ("folder" == menuType && SelectedItemPaths.Count() == 1 ||
                null != FolderPath && "directory" == menuType && 0 != FolderPath.Length) return canShowMenu;

            // Is Supported File
            var path = "file" == menuType ? SelectedItemPaths.First() : FolderPath;
            var ext = Path.GetExtension(path);
            if (! new FileExtensions().BlacklistedFileExtensions.Any(s => s.Equals(ext, StringComparison.OrdinalIgnoreCase) && "directory" != menuType)) return canShowMenu;
            // File type is not supported
            log.Info($"Unsupported file type: {ext}");

            return false;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menuType = Helper.GetClickedItemType(this);
            var clickedItemPath = Helper.GetClickedItemPath(menuType, this);
            var shellStartUpDirectory = Helper.GetShellStartUpDirectory(menuType, this);

            switch (menuType)
            {
                case "directory":
                    return _contextMenu.GetFolderMenu(menuType, clickedItemPath, clickedItemPath);

                case "folder":
                    return _contextMenu.GetFolderMenu(menuType, clickedItemPath, shellStartUpDirectory);

                case "file":
	                return _contextMenu.GetFileMenu(menuType, clickedItemPath, this);

                default:
                    return new ContextMenuStrip();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing) _contextMenu.Dispose();

            _disposedValue = true;
        }
    }
}