using Sonnenberg.Common;
using Sonnenberg.ContextMenu.Properties;
using Sonnenberg.ContextMenu.SubMenuItems;
using System.Drawing;
using System.Windows.Forms;
using Strings = Sonnenberg.Language.Strings;

namespace Sonnenberg.ContextMenu.ContextMenus
{
    internal sealed class FolderShortcutMenu
    {
        private ContextMenuStrip _contextMenuStrip;

        public FolderShortcutMenu()
        {
            SetContextMenuStrip();
        }

        private void SetContextMenuStrip()
        {
            _contextMenuStrip = new ContextMenuStrip();
        }

        internal ContextMenuStrip ItemDisplay(string clickedItemType, string clickedItemPath, string shortcutTargetFolder, bool isDarkTheme)
        {
            var icon = new Icon(Resources.SonnenbergContextMenuIcon, 40, 40);

            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = Strings.menuStripNameFolderShortcutMenu,
                Image = new Helper().CreateIcon(icon),
                Name = "Sonnenberg"
            };

            var osMenuItem = new OpenShell();
            toolStripMenuItem =
                osMenuItem.ItemDisplay(toolStripMenuItem, clickedItemType, shortcutTargetFolder, isDarkTheme);

            var opMenuItem = new OpenPath();
            toolStripMenuItem =
                opMenuItem.ItemDisplay(toolStripMenuItem, clickedItemType, clickedItemPath, isDarkTheme);

            var cpMenuItem = new CopyPath();
            toolStripMenuItem =
                cpMenuItem.ItemDisplay(toolStripMenuItem, clickedItemType, clickedItemPath, isDarkTheme);

            _contextMenuStrip.Items.Add(toolStripMenuItem);
            icon.Dispose();

            return _contextMenuStrip;
        }
    }
}