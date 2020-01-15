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
	/// <seealso cref="Program" />
	/// <seealso cref="IDisposable" />
	/// <seealso cref="ContextMenuStrip" />
	/// <seealso cref="Logger" />
	public class Program : IDisposable
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		private ContextMenuStrip _contextMenuStrip;

		private bool _disposedValue;

		public Program()
		{
			ConfigureLogger();
			SetContextMenuStrip();
		}

		private void SetContextMenuStrip()
		{
			_contextMenuStrip = new ContextMenuStrip();
		}

		private static void ConfigureLogger()
		{
			Logger.Configure();
		}
		
		private ContextMenuStrip DirectoryMenu(string menuType, string clickedItemPath, string shellStartUpDirectory)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameDirectoryMenu,
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

		private ContextMenuStrip FolderMenu(string menuType, string clickedItemPath, string shellStartUpDirectory)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameFolderMenu,
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

		private ContextMenuStrip FileMenu(string menuType, string clickedItemPath, ShellExtInitServer shellServer)
		{
			// Create our main menu.
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameFileMenu,
				Image = Resources.imgSonnenberg,
				Name = "Sonnenberg"
			};

			// Add a "count lines" option for file types considered text files
			if ("Text" == new FileTypes().GetFileType(Path.GetExtension(clickedItemPath)))
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

		private ContextMenuStrip FileShortcutMenu(string menuType, string clickedItemPath, ShellExtInitServer shellServer, string shortcutTargetFolder)
		{
			// Create our main menu.
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameFileShortcutMenu,
				Image = Resources.imgSonnenberg,
				Name = "Sonnenberg"
			};

			// Add a "count lines" option for file types considered text files
			if ("Text" == new FileTypes().GetFileType(Path.GetExtension(clickedItemPath)))
			{
				var countLines = new CountLines();
				var countLinesMenuItem = countLines.CreateToolStripMenuItem(shellServer.SelectedItemPaths);
				toolStripMenuItem.DropDownItems.Add(countLinesMenuItem);

				var countCleanLinesMenuItem = countLines.CreateToolStripMenuItem(shellServer.SelectedItemPaths, true);
				toolStripMenuItem.DropDownItems.Add(countCleanLinesMenuItem);
			}

			// Add items
						
			var openPath = new OpenPath();
			var openPathMenuItem = openPath.CreateToolStripMenuItem(menuType, shortcutTargetFolder);
			toolStripMenuItem.DropDownItems.Add(openPathMenuItem);
			
			var copyPath = new CopyPath();
			var fwdSlashesFilePathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath, true);
			toolStripMenuItem.DropDownItems.Add(fwdSlashesFilePathMenuItem);

			var filePathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath);
			toolStripMenuItem.DropDownItems.Add(filePathMenuItem);

			_contextMenuStrip.Items.Add(toolStripMenuItem);

			return _contextMenuStrip;
			//return menuType == "FileShortcut" ? GetFileMenu(menuType, clickedItemPath, shellServer) : GetFolderMenu(menuType, clickedItemPath, shellStartUpDirectory);
		}
		
		private ContextMenuStrip FolderShortcutMenu(string menuType, string clickedItemPath, string shellStartUpDirectory, string shortcutTargetFolder)
		{
			// Create our main menu.
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameFolderShortcutMenu,
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
			
			// Add items.
			var openPath = new OpenPath();
			var openPathMenuItem = openPath.CreateToolStripMenuItem(menuType, shortcutTargetFolder);
			toolStripMenuItem.DropDownItems.Add(openPathMenuItem);

			var copyPath = new CopyPath();
			var escapedFilePathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath, true);
			toolStripMenuItem.DropDownItems.Add(escapedFilePathMenuItem);

			var copyPathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath);
			toolStripMenuItem.DropDownItems.Add(copyPathMenuItem);

			//// Add our main menu.
			_contextMenuStrip.Items.Add(toolStripMenuItem);

			return _contextMenuStrip;
		}
		
		public ContextMenuStrip GetDirectoryMenu(string menuType, string clickedItemPath, string shellStartUpDirectory)
		{
			return DirectoryMenu(menuType, clickedItemPath, shellStartUpDirectory);
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
		
		public ContextMenuStrip GetFileShortcutMenu(string menuType, string clickedItemPath, ShellExtInitServer shellServer, string shortcutTargetFolder)
		{
			if(null != shellServer)
			{
				return FileShortcutMenu(menuType, clickedItemPath, shellServer, shortcutTargetFolder);
			}

			var message = $"{Strings.shellserverArgumentMissingError}";
			var ex = new Exception(message);
			log.Error(message);
			MessageBox.Show(message);

			throw ex;
		}
		
		public ContextMenuStrip GetFolderShortcutMenu(string menuType, string clickedItemPath, ShellExtInitServer shellServer, string shellStartUpDirectory, string shortcutTargetFolder)
		{
			if(null != shellServer)
			{
				return FolderShortcutMenu(menuType, clickedItemPath, shellStartUpDirectory, shortcutTargetFolder);
			}

			var message = $"{Strings.shellserverArgumentMissingError}";
			var ex = new Exception(message);
			log.Error(message);
			MessageBox.Show(message);

			throw ex;
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue) return;

			if (disposing) _contextMenuStrip.Dispose();

			_disposedValue = true;
		}
	}
}