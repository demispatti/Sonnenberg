using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using log4net;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using Sonnenberg.Core.Interfaces;
using Sonnenberg.Core.Services;

namespace Sonnenberg.Core
{
    /// <summary>
    /// Main entry point for the Sonnenberg Windows Explorer context menu extension.
    /// This class integrates with Windows Explorer and displays custom context menus.
    /// </summary>
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Class, @"Desktop\Background")]
    [COMServerAssociation(AssociationType.Class, @"Directory\Background")]
    [COMServerAssociation(AssociationType.Drive)]
    [COMServerAssociation(AssociationType.Directory)]
    [COMServerAssociation(AssociationType.AllFilesAndFolders)]
    public class ShellExtension : SharpContextMenu, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ShellExtension));

        private readonly IItemContextResolver _contextResolver;
        private readonly IMenuFactory _menuFactory;
        private readonly IThemeDetector _themeDetector;
        private readonly IContextMenuStateManager _stateManager;

        private bool _disposedValue;

        public ShellExtension()
            : this(
                new ItemContextResolver(),
                null, // Will be set from UI project
                null, // Will be set from Infrastructure project
                null) // Will be set from Infrastructure project
        {
        }

        public ShellExtension(
            IItemContextResolver contextResolver,
            IMenuFactory menuFactory,
            IThemeDetector themeDetector,
            IContextMenuStateManager stateManager)
        {
            _contextResolver = contextResolver ?? throw new ArgumentNullException(nameof(contextResolver));
            _menuFactory = menuFactory;
            _themeDetector = themeDetector;
            _stateManager = stateManager;
        }

        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            try
            {
                // Resolve the context of the clicked item
                var selectedPaths = SelectedItemPaths?.ToArray() ?? new string[0];
                var context = _contextResolver.ResolveContext(FolderPath, selectedPaths);

                // Detect theme (dark/light mode)
                var isDarkTheme = _themeDetector?.IsDarkTheme() ?? false;

                // Subscribe to state manager events if not already subscribed
                _stateManager?.SubscribeIfNeeded();

                // Create and return the appropriate menu
                var menu = _menuFactory?.CreateMenu(context, isDarkTheme);

                return menu ?? new ContextMenuStrip { Name = "Sonnenberg" };
            }
            catch (Exception ex)
            {
                Log.Error("Failed to create context menu", ex);
                return new ContextMenuStrip { Name = "Sonnenberg" };
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                _stateManager?.Dispose();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
