using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using Sonnenberg.Common;

namespace Sonnenberg.ShellServer
{
    /// <summary>
    /// This is "Sonnenberg". It's the application's main class.
    /// It instantiates all core classes and registers the custom event.
    /// And it is the class that's inheriting from SharpContextMenu.
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
        private ContextMenu _contextMenu;

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
            _contextMenu = new ContextMenu();
        }

        private void ConfigureLogger()
        {
            Logger.Configure();
        }

        protected override bool CanShowMenu()
        {
            return _contextMenu.CanShowMenu(this);
        }

        protected override ContextMenuStrip CreateMenu()
        {
            return _contextMenu.CreateMenu(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing) _contextMenu.Dispose();

            _disposedValue = true;
        }
    }
}