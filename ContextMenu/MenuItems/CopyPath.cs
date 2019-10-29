using System.Drawing;
using System.Windows.Forms;
using Sonnenberg.Common;
using Sonnenberg.ContextMenu.Properties;
using Sonnenberg.Language;

namespace Sonnenberg.ContextMenu.MenuItems
{
	/// <summary>
	/// The class responsible for copying the path of a given, clicked element.
	/// </summary>
	/// <remarks>
	/// - Creates a ToolStripMenuItem
	/// - Creates a click action
	/// - Clears the clipboard and adds the path to the clipboard
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
	}
}