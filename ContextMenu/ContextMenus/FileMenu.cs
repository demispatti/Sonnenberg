﻿using System.Drawing;
using System.Windows.Forms;
using Sonnenberg.Common;
using Sonnenberg.ContextMenu.Properties;
using Sonnenberg.ContextMenu.SubMenuItems;
using Sonnenberg.Language;

namespace Sonnenberg.ContextMenu.ContextMenus
{
    internal sealed class FileMenu
    {
        private ContextMenuStrip _contextMenuStrip;

        public FileMenu()
        {
            SetContextMenuStrip();
        }

        private void SetContextMenuStrip()
        {
            _contextMenuStrip = new ContextMenuStrip();
        }

        internal ContextMenuStrip ItemDisplay(string clickedItemType, string clickedItemPath,
            string selectedItemPath, bool isDarkTheme)
        {
            var icon = new Icon(Resources.SonnenbergContextMenuIcon, 40, 40);

            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = Strings.menuStripNameFileMenu,
                Image = new Helper().CreateIcon(icon),
                Name = "Sonnenberg"
            };

            var cpMenuItem = new CopyPath();
            toolStripMenuItem =
                cpMenuItem.ItemDisplay(toolStripMenuItem, clickedItemType, clickedItemPath, isDarkTheme);

            var clMenuItem = new CountLines();
            toolStripMenuItem =
                clMenuItem.ItemDisplay(toolStripMenuItem, clickedItemPath, selectedItemPath, isDarkTheme);

            _contextMenuStrip.Items.Add(toolStripMenuItem);
            icon.Dispose();

            return _contextMenuStrip;
        }
    }
}