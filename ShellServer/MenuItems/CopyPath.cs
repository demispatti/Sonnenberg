using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SharpShell;
using Sonnenberg.Common;
using Sonnenberg.Language;
using Sonnenberg.ShellServer.Properties;

namespace Sonnenberg.ShellServer.MenuItems
{
    /// <summary>
    /// The class responsible for copying the path of a given, clicked element.
    /// </summary>
    /// <remarks>
    ///     - Creates a ToolStripMenuItem
    ///     - Creates a click action
    ///     - Determines the path of the clicked item
    ///     - Clears the clipboard and adds the path to the clipboard
    /// </remarks>
    /// <seealso cref="ContextMenu" />
    /// <seealso cref="Logger" />
    internal class CopyPath
    {
        internal CopyPath()
        {
            ConfigureLogger();
        }
        private static void ConfigureLogger()
        {
            Logger.Configure();
        }
        internal ToolStripMenuItem CreateToolStripMenuItem(string menuType, string clickedItemPath, bool forwardSlashes = false)
        {
            string text;
            Bitmap image;
            
            // Set text and image based on the clicked item.
            switch (menuType)
            {
                case "directory":
                {
                    text = forwardSlashes ? Strings.copyForwardslashedDirectoryPathText : Strings.copyDirectoryPathText;
                    image = forwardSlashes ? Resources.imgCopyFolderPathFs : Resources.imgCopyFolderPathBs;
                    break;
                }
                case "folder":
                {
                    text = forwardSlashes ? Strings.copyForwardslashedFolderPathText : Strings.copyFolderPathText;
                    image = forwardSlashes ? Resources.imgCopyFolderPathFs : Resources.imgCopyFolderPathBs;
                    break;
                }
                default:
                    text = forwardSlashes ? Strings.copyForwardslashedFilePathText : Strings.copyFilePathText;
                    image = forwardSlashes ? Resources.imgCopyFilePathFs : Resources.imgCopyFilePathBs;
                    break;
            }
            
            //  Creates the item.
            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = text,
                Image = image
            };
            
            //  Adds click action.
            toolStripMenuItem.Click += (sender, args) => DoClickAction(clickedItemPath, forwardSlashes);

            return toolStripMenuItem;
        }
        private void DoClickAction(string clickedItemPath, bool forwardslashes)
        {
            if (forwardslashes) clickedItemPath = clickedItemPath.Replace('\\', '/');

            Clipboard.Clear();
            Clipboard.SetText(clickedItemPath);
        }

        /// <summary>
        /// Determines and returns the full path of the item or directory that got right-clicked inside Windows Explorer,
        /// based on possible paths <c>ShellExtInitServer</c> provides us with.
        /// </summary>
        /// <seealso cref="ShellExtInitServer" />
        /// <param name="menuType"></param>
        /// <param name="shellServer"></param>
        /// <returns></returns>
        private static string ClickedItemPath(string menuType, ShellExtInitServer shellServer)
        {
            if ("file" == menuType || "folder" == menuType) return shellServer.SelectedItemPaths.First();

            return shellServer.FolderPath;
        }
        
        /// <summary>
        /// Returns the full path to the item or directory that was right-clicked by the user inside Windows Explorer.
        /// </summary>
        /// <param name="menuType"></param>
        /// <param name="shellServer"></param>
        /// <returns>string</returns>
        public static string GetClickedItemPath(string menuType, ShellServer shellServer)
        {
            return ClickedItemPath(menuType, shellServer);
        }
    }
}