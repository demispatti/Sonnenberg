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
		
		private ContextMenuStrip DirectoryMenu(string menuType, string clickedItemPath, string targetDirectory)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameDirectoryMenu,
				Image = Resources.imgSonnenberg,
				Name = "Sonnenberg"
			};

			toolStripMenuItem = CreateOpenShellMenuItem(toolStripMenuItem, menuType, targetDirectory);
			toolStripMenuItem = CreateCopyPathMenuItem(toolStripMenuItem, menuType, clickedItemPath);

			_contextMenuStrip.Items.Add(toolStripMenuItem);

			return _contextMenuStrip;
		}

		private ContextMenuStrip FolderMenu(string menuType, string clickedItemPath, string targetDirectory)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameFolderMenu,
				Image = Resources.imgSonnenberg,
				Name = "Sonnenberg"
			};

			toolStripMenuItem = CreateOpenShellMenuItem(toolStripMenuItem, menuType, targetDirectory);
			toolStripMenuItem = CreateCopyPathMenuItem(toolStripMenuItem, menuType, clickedItemPath);

			_contextMenuStrip.Items.Add(toolStripMenuItem);

			return _contextMenuStrip;
		}

		private ContextMenuStrip FileMenu(string menuType, string clickedItemPath, ShellExtInitServer shellServer)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameFileMenu,
				Image = Resources.imgSonnenberg,
				Name = "Sonnenberg"
			};

			toolStripMenuItem = CreateCountLinesMenuItem(toolStripMenuItem, clickedItemPath, shellServer);
			toolStripMenuItem = CreateCopyPathMenuItem(toolStripMenuItem, menuType, clickedItemPath);

			_contextMenuStrip.Items.Add(toolStripMenuItem);

			return _contextMenuStrip;
		}

		private ContextMenuStrip FileShortcutMenu(string menuType, string clickedItemPath, ShellExtInitServer shellServer, string shortcutTargetFolder)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameFileShortcutMenu,
				Image = Resources.imgSonnenberg,
				Name = "Sonnenberg"
			};

			toolStripMenuItem = CreateCountLinesMenuItem(toolStripMenuItem, shortcutTargetFolder, shellServer);
			toolStripMenuItem = CreateOpenPathMenuItem(toolStripMenuItem, menuType,  shortcutTargetFolder);
			toolStripMenuItem = CreateCopyPathMenuItem(toolStripMenuItem, menuType, clickedItemPath);

			_contextMenuStrip.Items.Add(toolStripMenuItem);

			return _contextMenuStrip;
		}
		
		private ContextMenuStrip FolderShortcutMenu(string menuType, string clickedItemPath, string shortcutTargetFolder)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = Strings.menuStripNameFolderShortcutMenu,
				Image = Resources.imgSonnenberg,
				Name = "Sonnenberg"
			};
			
			toolStripMenuItem = CreateOpenShellMenuItem(toolStripMenuItem, menuType, shortcutTargetFolder);
			toolStripMenuItem = CreateOpenPathMenuItem(toolStripMenuItem, menuType,  shortcutTargetFolder);
			toolStripMenuItem = CreateCopyPathMenuItem(toolStripMenuItem, menuType, clickedItemPath);

			_contextMenuStrip.Items.Add(toolStripMenuItem);

			return _contextMenuStrip;
		}

		private ToolStripMenuItem CreateOpenShellMenuItem(ToolStripMenuItem toolStripMenuItem, string menuType, string targetDirectory)
		{
			var openShell = new OpenShell();
			if (OpenShell.AppExists("powershell.exe"))
			{
				var openPowershellInsideMenuItem = openShell.CreateToolStripMenuItem(menuType, targetDirectory, "powershell.exe", false);
				toolStripMenuItem.DropDownItems.Add(openPowershellInsideMenuItem);

				var openPowershellInsideElevatedMenuItem = openShell.CreateToolStripMenuItem(menuType, targetDirectory, "powershell.exe", true);
				toolStripMenuItem.DropDownItems.Add(openPowershellInsideElevatedMenuItem);
			}
			
			if (OpenShell.AppExists("cmd.exe"))
			{
				var openCmdInsideMenuItem = openShell.CreateToolStripMenuItem(menuType, targetDirectory, "cmd.exe", false);
				toolStripMenuItem.DropDownItems.Add(openCmdInsideMenuItem);

				var openCmdInsideElevatedMenuItem = openShell.CreateToolStripMenuItem(menuType, targetDirectory, "cmd.exe", true);
				toolStripMenuItem.DropDownItems.Add(openCmdInsideElevatedMenuItem);
			}

			return toolStripMenuItem;
		}

		private ToolStripMenuItem CreateOpenPathMenuItem(ToolStripMenuItem toolStripMenuItem, string menuType, string targetDirectory)
		{
			var openPath = new OpenPath();
			var openPathMenuItem = openPath.CreateToolStripMenuItem(menuType, targetDirectory);
			toolStripMenuItem.DropDownItems.Add(openPathMenuItem);

			return toolStripMenuItem;
		}
		
		private ToolStripMenuItem CreateCopyPathMenuItem(ToolStripMenuItem toolStripMenuItem, string menuType, string clickedItemPath)
		{
			var copyPath = new CopyPath();
			var escapedFilePathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath, true);
			toolStripMenuItem.DropDownItems.Add(escapedFilePathMenuItem);
			
			var copyPathMenuItem = copyPath.CreateToolStripMenuItem(menuType, clickedItemPath);
			toolStripMenuItem.DropDownItems.Add(copyPathMenuItem);

			return toolStripMenuItem;
		}

		private ToolStripMenuItem CreateCountLinesMenuItem(ToolStripMenuItem toolStripMenuItem, string clickedItemPath, ShellExtInitServer shellServer)
		{
			if ("Text" == new FileTypes().GetFileType(Path.GetExtension(clickedItemPath)))
			{
				return toolStripMenuItem;
			}

			var countLines = new CountLines();
            var countLinesMenuItem = countLines.CreateToolStripMenuItem(shellServer.SelectedItemPaths);
            toolStripMenuItem.DropDownItems.Add(countLinesMenuItem);

            var countCleanLinesMenuItem = countLines.CreateToolStripMenuItem(shellServer.SelectedItemPaths, true);
            toolStripMenuItem.DropDownItems.Add(countCleanLinesMenuItem);

            return toolStripMenuItem;
		}
		
		public ContextMenuStrip GetDirectoryMenu(string menuType, string clickedItemPath, string targetDirectory)
		{
			return DirectoryMenu(menuType, clickedItemPath, targetDirectory);
		}
		
		public ContextMenuStrip GetFolderMenu(string menuType, string clickedItemPath, string targetDirectory)
		{
			return FolderMenu(menuType, clickedItemPath, targetDirectory);
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
		
		public ContextMenuStrip GetFolderShortcutMenu(string menuType, string clickedItemPath, ShellExtInitServer shellServer, string shortcutTargetFolder)
		{
			if(null != shellServer)
			{
				return FolderShortcutMenu(menuType, clickedItemPath, shortcutTargetFolder);
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