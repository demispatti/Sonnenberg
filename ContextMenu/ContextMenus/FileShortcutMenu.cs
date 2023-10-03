using System.Drawing;
using System.Windows.Forms;
using Sonnenberg.Common;
using Sonnenberg.ContextMenu.Properties;
using Sonnenberg.ContextMenu.SubMenuItems;
using Sonnenberg.Language;

namespace Sonnenberg.ContextMenu.ContextMenus
{
    internal sealed class FileShortcutMenu
    {
        private ContextMenuStrip _contextMenuStrip;

        public FileShortcutMenu()
        {
            SetContextMenuStrip();
        }

        private void SetContextMenuStrip()
        {
            _contextMenuStrip = new ContextMenuStrip();
        }

        internal ContextMenuStrip ItemDisplay(string clickedItemType, string clickedItemPath,
            string selectedItemPath, string shortcutTargetFolder, bool isDarkTheme)
        {
            var icon = new Icon(Resources.SonnenbergContextMenuIcon, 40, 40);

            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = Strings.menuStripNameFileShortcutMenu,
                Image = new Helper().CreateIcon(icon),
                Name = "Sonnenberg"
            };

            var opMenuItem = new OpenPath();
            toolStripMenuItem =
                opMenuItem.ItemDisplay(toolStripMenuItem, clickedItemType, shortcutTargetFolder, isDarkTheme);

            var cpMenuItem = new CopyPath();
            toolStripMenuItem =
                cpMenuItem.ItemDisplay(toolStripMenuItem, clickedItemType, clickedItemPath, isDarkTheme);

            _contextMenuStrip.Items.Add(toolStripMenuItem);
            icon.Dispose();

            return _contextMenuStrip;
        }
    }
}