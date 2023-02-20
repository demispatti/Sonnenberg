using Sonnenberg.Common;
using Sonnenberg.ContextMenu.Properties;
using Sonnenberg.ContextMenu.SubMenuItems;
using System.Drawing;
using System.Windows.Forms;
using Strings = Sonnenberg.Language.Strings;

namespace Sonnenberg.ContextMenu.ContextMenus
{
    internal sealed class FolderMenu
    {
        private ContextMenuStrip _contextMenuStrip;

        public FolderMenu()
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
            Icon icon = new Icon(Resources.Context_Menu_Directory, 40, 40);

            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = Strings.menuStripNameFolderMenu,
                Image = new Helper().CreateIcon(icon),
                Name = "Sonnenberg"
            };

            var osMenuItem = new OpenShell();
            toolStripMenuItem =
                osMenuItem.ItemDisplay(toolStripMenuItem, clickedItemType, targetDirectory, isDarkTheme);

            var cpMenuItem = new CopyPath();
            toolStripMenuItem =
                cpMenuItem.ItemDisplay(toolStripMenuItem, clickedItemType, clickedItemPath, isDarkTheme);

            _contextMenuStrip.Items.Add(toolStripMenuItem);
            icon.Dispose();

            return _contextMenuStrip;
        }
    }
}