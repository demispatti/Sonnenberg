using System;
using System.Drawing;
using System.Windows.Forms;
using Sonnenberg.Core.Interfaces;
using Sonnenberg.Core.Models;

namespace Sonnenberg.UI.Menus
{
    /// <summary>
    /// Factory for creating context menus based on item context
    /// </summary>
    public class MenuFactory : IMenuFactory
    {
        private readonly IMenuItemProvider _menuItemProvider;

        public MenuFactory(IMenuItemProvider menuItemProvider)
        {
            _menuItemProvider = menuItemProvider ?? throw new ArgumentNullException(nameof(menuItemProvider));
        }

        public ContextMenuStrip CreateMenu(ItemContext context, bool isDarkTheme)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var menuStrip = new ContextMenuStrip { Name = "Sonnenberg" };

            switch (context.Type)
            {
                case ItemType.Directory:
                    return CreateDirectoryMenu(context, isDarkTheme);

                case ItemType.Folder:
                case ItemType.FolderShortcut:
                    return CreateFolderMenu(context, isDarkTheme);

                case ItemType.File:
                case ItemType.FileShortcut:
                    return CreateFileMenu(context, isDarkTheme);

                default:
                    return menuStrip;
            }
        }

        private ContextMenuStrip CreateDirectoryMenu(ItemContext context, bool isDarkTheme)
        {
            var menuStrip = new ContextMenuStrip();
            var mainItem = _menuItemProvider.CreateMainMenuItem("Sonnenberg", isDarkTheme);

            // Add shell launcher items
            var shellItems = _menuItemProvider.CreateShellLauncherItems(context, isDarkTheme);
            foreach (var item in shellItems)
            {
                mainItem.DropDownItems.Add(item);
            }

            // Add copy path items
            var copyPathItems = _menuItemProvider.CreateCopyPathItems(context, isDarkTheme);
            foreach (var item in copyPathItems)
            {
                mainItem.DropDownItems.Add(item);
            }

            menuStrip.Items.Add(mainItem);
            return menuStrip;
        }

        private ContextMenuStrip CreateFolderMenu(ItemContext context, bool isDarkTheme)
        {
            var menuStrip = new ContextMenuStrip();
            var mainItem = _menuItemProvider.CreateMainMenuItem("Sonnenberg", isDarkTheme);

            // Add shell launcher items
            var shellItems = _menuItemProvider.CreateShellLauncherItems(context, isDarkTheme);
            foreach (var item in shellItems)
            {
                mainItem.DropDownItems.Add(item);
            }

            // Add copy path items
            var copyPathItems = _menuItemProvider.CreateCopyPathItems(context, isDarkTheme);
            foreach (var item in copyPathItems)
            {
                mainItem.DropDownItems.Add(item);
            }

            menuStrip.Items.Add(mainItem);
            return menuStrip;
        }

        private ContextMenuStrip CreateFileMenu(ItemContext context, bool isDarkTheme)
        {
            var menuStrip = new ContextMenuStrip();
            var mainItem = _menuItemProvider.CreateMainMenuItem("Sonnenberg", isDarkTheme);

            // Add line counter items
            var lineCounterItems = _menuItemProvider.CreateLineCounterItems(context, isDarkTheme);
            foreach (var item in lineCounterItems)
            {
                mainItem.DropDownItems.Add(item);
            }

            // Add copy path items
            var copyPathItems = _menuItemProvider.CreateCopyPathItems(context, isDarkTheme);
            foreach (var item in copyPathItems)
            {
                mainItem.DropDownItems.Add(item);
            }

            menuStrip.Items.Add(mainItem);
            return menuStrip;
        }
    }
}
