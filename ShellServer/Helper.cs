using System;
using System.IO;
using System.Linq;
using SharpShell;
using log4net;

namespace Sonnenberg.ShellServer
{
    internal class Helper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Helper));

        /// <summary>
        /// Determines wether the clicked item is a file, a folder or a directory.
        /// </summary>
        /// <param name="shellServer"></param>
        /// <returns>string</returns>
        internal static string GetClickedItemType(ShellExtInitServer shellServer)
        {
            var itemType = "unsupported";

            if (shellServer.FolderPath != null) return "directory";

            try
            {
                var path = shellServer.SelectedItemPaths.First();
                if (null != path)
                {
                    var length = Path.GetExtension(path).Length;
                    itemType = length == 0 ? "folder" : "file";
                }
            }
            catch (ArgumentNullException ex)
            {
                log.Error($"{ex.Message} (getclickeditemtype)");

                throw;
            }

            return itemType;
        }

        private static string ShellStartUpDirectory(string menuType, ShellExtInitServer shellServer)
        {
            switch (menuType)
            {
                case "file":

                    return Path.GetDirectoryName(shellServer.SelectedItemPaths.First());

                case "folder":

                    return shellServer.SelectedItemPaths.First();

                default:

                    return "";
            }
        }

        internal static string GetShellStartUpDirectory(string menuType, ShellServer shellServer)
        {
            return ShellStartUpDirectory(menuType, shellServer);
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