using System.Drawing;
using System.Windows.Forms;
using Sonnenberg.Common;
using Sonnenberg.ContextMenu.Properties;
using Sonnenberg.ContextMenu.SubMenuItems;
using Sonnenberg.Language;

namespace Sonnenberg.ContextMenu.ContextMenus
{
    internal sealed class DirectoryMenu
    {
        private ContextMenuStrip _contextMenuStrip;

        public DirectoryMenu()
        {
            SetContextMenuStrip();
        }

        private void SetContextMenuStrip()
        {
            _contextMenuStrip = new ContextMenuStrip();
        }

        internal ContextMenuStrip ItemDisplay(string clickedItemType, string clickedItemPath,
            string targetDirectory, bool isDarkTheme)
        {
            var icon = new Icon(Resources.SonnenbergContextMenuIcon, 40, 40);

            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = Strings.menuStripNameDirectoryMenu,
                Image = new Helper().CreateIcon(icon),
                Name = "Sonnenberg"
            };

            var osMenuItem = new OpenShell();
            toolStripMenuItem = osMenuItem.ItemDisplay(toolStripMenuItem, clickedItemType, targetDirectory, isDarkTheme);

            var cpMenuItem = new CopyPath();
            toolStripMenuItem = cpMenuItem.ItemDisplay(toolStripMenuItem, clickedItemType, clickedItemPath, isDarkTheme);

            _contextMenuStrip.Items.Add(toolStripMenuItem);
            icon.Dispose();

            return _contextMenuStrip;
        }
    }
}