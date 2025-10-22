using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IWshRuntimeLibrary;
using log4net;
using Sonnenberg.Core.Interfaces;
using Sonnenberg.Core.Models;
using File = System.IO.File;

namespace Sonnenberg.Core.Services
{
    /// <summary>
    /// Resolves the context of items clicked in Windows Explorer
    /// </summary>
    public class ItemContextResolver : IItemContextResolver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ItemContextResolver));

        public ItemContext ResolveContext(string folderPath, string[] selectedItemPaths)
        {
            var context = new ItemContext
            {
                SelectedPaths = selectedItemPaths ?? new string[0]
            };

            // Determine if this is a directory background click
            if (folderPath != null)
            {
                context.Type = ItemType.Directory;
                context.Path = folderPath;
                context.ContainingFolder = Path.GetDirectoryName(folderPath);
                return context;
            }

            // Get the first selected item
            if (!context.SelectedPaths.Any())
            {
                return context;
            }

            var clickedItemPath = context.SelectedPaths.First();
            context.Path = clickedItemPath;

            try
            {
                var extension = Path.GetExtension(clickedItemPath);
                var fileAttributes = File.GetAttributes(clickedItemPath);

                // Handle shortcuts
                if (extension == ".lnk")
                {
                    context.ShortcutInfo = ResolveShortcut(clickedItemPath);
                    context.Type = context.ShortcutInfo.TargetType == ItemType.Folder
                        ? ItemType.FolderShortcut
                        : ItemType.FileShortcut;
                }
                else
                {
                    // Determine if folder or file
                    context.Type = (fileAttributes & FileAttributes.Directory) != 0
                        ? ItemType.Folder
                        : ItemType.File;
                }

                context.ContainingFolder = Path.GetDirectoryName(clickedItemPath);
                context.ShellStartUpDirectory = DetermineShellStartUpDirectory(context);

                return context;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to resolve item context for path: {clickedItemPath}", ex);
                throw;
            }
        }

        private ShortcutInfo ResolveShortcut(string shortcutPath)
        {
            var wsh = new WshShellClass();
            var sc = (IWshShortcut)wsh.CreateShortcut(shortcutPath);

            var targetPath = sc.TargetPath;
            var targetExtension = Path.GetExtension(targetPath);

            // Handle nested shortcuts
            if (targetExtension == ".lnk")
            {
                return new ShortcutInfo
                {
                    TargetPath = targetPath,
                    TargetType = ItemType.FileShortcut
                };
            }

            var fileAttributes = File.GetAttributes(targetPath);
            var isFolder = (fileAttributes & FileAttributes.Directory) != 0;

            return new ShortcutInfo
            {
                TargetPath = targetPath,
                TargetFolder = isFolder ? targetPath : Path.GetDirectoryName(targetPath),
                TargetType = isFolder ? ItemType.Folder : ItemType.File
            };
        }

        private string DetermineShellStartUpDirectory(ItemContext context)
        {
            switch (context.Type)
            {
                case ItemType.Folder:
                    return context.Path;

                case ItemType.FolderShortcut:
                    return context.ShortcutInfo?.TargetPath ?? string.Empty;

                default:
                    return string.Empty;
            }
        }
    }
}
