using System;
using System.IO;
using System.Windows.Forms;
using log4net;
using SharpShell;
using Sonnenberg.Common;
using Sonnenberg.ContextMenu.MenuItems;
using Sonnenberg.FileHelper;
using Sonnenberg.Language;
using Sonnenberg.ContextMenu.Properties;

namespace Sonnenberg.ContextMenu
{
	/// <summary>
	/// The class responsible for creating the context menu
	/// and all its dependencies.
	/// </summary>
	/// <seealso cref="ContextMenu" />
	/// <seealso cref="IDisposable" />
	/// <seealso cref="ContextMenuStrip" />
	/// <seealso cref="Logger" />
	public class ContextMenu : IDisposable
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ContextMenu));

		private ContextMenuStrip _contextMenuStrip;

		private bool _disposedValue;

		public ContextMenu()
		{
			ConfigureLogger();
			SetContextMenuStrip();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void SetContextMenuStrip()
		{
			_contextMenuStrip = new ContextMenuStrip();
		}

		private static void ConfigureLogger()
		{
			Logger.Configure();
		}

		/// <summary>
		/// Creates and returns the context menu strip that makes up the context menu
		/// that gets served in case the user right-clicked the directory inside the Windows Explorer.
		/// </summary>
		/// <param name="menuType"></param>
		/// <param name="clickedItemPath"></param>
		/// <param name="shellStartUpDirectory"></param>
		/// <returns>ContextMenuStrip</returns>
		private ContextMenuStrip FolderMenu(string menuType, string clickedItemPath, string shellStartUpDirectory)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = "folder" == menuType ? Strings.menuStripNameFolderMenu : Strings.menuStripNameDirectoryMenu,
				Image = Resources.imgSonnenberg,
				Name = "Sonnenberg"
			};

			var openShell = new OpenShell();
			if (OpenShell.AppExists("powershell.exe"))
			{
				var openPowershellInsideMenuItem = openShell.CreateToolStripMenuItem(menuType, shellStartUpDirectory, "powershell.exe", false);
				toolStripMenuItem.DropDownItems.Add(openPowershellInsideMenuItem);

				var openPowershellInsideElevatedMenuItem = openShell.CreateToolStripMenuItem(menuType, shellStartUpDirectory, "powershell.exe", true);
				toolStripMenuItem.DropDownItems.Add(openPowershellInsideElevatedMenuItem);
			}

			if (OpenShell.AppExists("cmd.exe"))
			{
				var openCmdInsideMenuItem = openShell.CreateToolStripMenuItem(menuType, shellStartUpDirectory, "cmd.exe", false);
				toolStripMenuItem.DropDownItems.Add(openCmdInsideMenuItem);

				var openCmdInsideElevatedMenuItem = openShell.CreateToolStripMenuItem(menuType, shellStartUpDirectory, "cmd.exe", true);
				toolStripMenuItem.DropDownItems.Add(openCmdInsideElevatedMenuItem);
			}

			var copyPath = new CopyPath();
			var escapedFilePathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath, true);
			toolStripMenuItem.DropDownItems.Add(escapedFilePathMenuItem);

			var copyPathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath);
			toolStripMenuItem.DropDownItems.Add(copyPathMenuItem);

			//// Add our main menu.
			_contextMenuStrip.Items.Add(toolStripMenuItem);

			return _contextMenuStrip;
		}

		/// <summary>
		/// Creates and returns the context menu strip that makes up the context menu
		/// that gets served in case the user right-clicked an item inside a Windows Explorer directory.
		/// </summary>
		/// <param name="menuType"></param>
		/// <param name="clickedItemPath"></param>
		/// <param name="shellServer"></param>
		/// <returns>ContextMenuStrip</returns>
		private ContextMenuStrip FileMenu(string menuType, string clickedItemPath, ShellExtInitServer shellServer)
		{
			// Create our main menu.
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameFilesMenu,
				Image = Resources.imgSonnenberg,
				Name = "Sonnenberg"
			};

			// Add a "count lines" option for file types considered text files
			if ("text" == new FileTypes().GetFileType(Path.GetExtension(clickedItemPath)))
			{
				var countLines = new CountLines();
				var countLinesMenuItem = countLines.CreateToolStripMenuItem(shellServer.SelectedItemPaths);
				toolStripMenuItem.DropDownItems.Add(countLinesMenuItem);

				var countCleanLinesMenuItem = countLines.CreateToolStripMenuItem(shellServer.SelectedItemPaths, true);
				toolStripMenuItem.DropDownItems.Add(countCleanLinesMenuItem);
			}

			// Add items
			var copyPath = new CopyPath();
			var fwdSlashesFilePathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath, true);
			toolStripMenuItem.DropDownItems.Add(fwdSlashesFilePathMenuItem);

			var filePathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath);
			toolStripMenuItem.DropDownItems.Add(filePathMenuItem);

			_contextMenuStrip.Items.Add(toolStripMenuItem);

			return _contextMenuStrip;
		}

		public ContextMenuStrip GetFolderMenu(string menuType, string clickedItemPath, string shellStartUpDirectory)
		{
			return FolderMenu(menuType, clickedItemPath, shellStartUpDirectory);
		}

		public ContextMenuStrip GetFileMenu(string menuType, string clickedItemPath, ShellExtInitServer shellServer)
		{
			if(null != shellServer)
			{
				return FileMenu(menuType, clickedItemPath, shellServer);
			}

			var message = $"{Strings.shellserverArgumentMissingError}";
			var ex = new Exception(message);
			log.Error(message);
			MessageBox.Show(message);

			throw ex;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue) return;

			if (disposing) _contextMenuStrip.Dispose();

			_disposedValue = true;
		}
	}
}