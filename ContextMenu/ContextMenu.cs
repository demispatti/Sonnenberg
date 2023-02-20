using Sonnenberg.Common;
using Sonnenberg.ContextMenu.ContextMenus;
using System;
//using System.Collections.Generic;
using System.Windows.Forms;

namespace Sonnenberg.ContextMenu
{
    /// <summary>
    /// The class responsible for creating the context menu
    /// and all its dependencies.
    /// </summary>
    /// <seealso cref="ContextMenu" />
    /// <seealso cref="IDisposable" />
    /// <seealso cref="ContextMenuStrip" />
    public sealed class ContextMenu : IDisposable
    {
        private ContextMenuStrip _contextMenuStrip;

        private bool _disposedValue;

        public ContextMenu()
        {
            ConfigureLogger();
            SetContextMenuStrip();
        }

        private void SetContextMenuStrip()
        {
            _contextMenuStrip = new ContextMenuStrip();
        }

        private static void ConfigureLogger()
        {
            Logger.Configure();
        }

        public ContextMenuStrip GetDirectoryMenu(string clickedItemType, string clickedItemPath, string targetDirectory, bool isDarkTheme)
        {
            return new DirectoryMenu().ItemDisplay(clickedItemType, clickedItemPath, targetDirectory, isDarkTheme);
        }

        public ContextMenuStrip GetFolderMenu(string clickedItemType, string clickedItemPath, string targetDirectory, bool isDarkTheme)
        {
            return new FolderMenu().ItemDisplay(clickedItemType, clickedItemPath, targetDirectory, isDarkTheme);
        }

        public ContextMenuStrip GetFileMenu(string clickedItemType, string clickedItemPath, string selectedItemPath, bool isDarkTheme)
        {
            return new FileMenu().ItemDisplay(clickedItemType, clickedItemPath, selectedItemPath, isDarkTheme);
        }

        public ContextMenuStrip GetFileShortcutMenu(string clickedItemType, string clickedItemPath, string selectedItemPath, string shortcutTargetFolder, bool isDarkTheme)
        {
            return new FileShortcutMenu().ItemDisplay(clickedItemType, clickedItemPath, selectedItemPath, shortcutTargetFolder, isDarkTheme);
        }

        public ContextMenuStrip GetFolderShortcutMenu(string clickedItemType, string clickedItemPath, string shortcutTargetFolder, bool isDarkTheme)
        {
            return new FolderShortcutMenu().ItemDisplay(clickedItemType, clickedItemPath, shortcutTargetFolder, isDarkTheme);
        }

        private void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing) _contextMenuStrip.Dispose();

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}