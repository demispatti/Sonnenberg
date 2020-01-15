using System.Diagnostics;
using System.Windows.Forms;
using log4net;
using Sonnenberg.Language;
using Sonnenberg.Common;
using Sonnenberg.ContextMenu.Properties;

namespace Sonnenberg.ContextMenu.MenuItems
{
	/// <summary>
	/// The class responsible for creating a <c>ToolStripMenuItem</c> and its click action.
	/// It starts a CMD-, Git-CMD- or Git-Bash-Process and cds into the respective directory.
	/// The processes can be started with limited or elevated privileges.
	/// </summary>
	/// <remarks>
	/// - Creates a ToolStripMenuItem
	/// - Creates a click action
	/// - Checks that the requested executable is installed
	/// - Determines the directory where the clicked item resides
	/// - Determines the directory the CMD process will cd into
	/// - Assembles the required <c>StartInfo</c> parameters
	/// - Starts a CMD-, Git-CMD- or Git-Bash-Process and cds into the respective directory
	/// </remarks>
	/// <seealso cref="Sonnenberg.ContextMenu" />
	/// <seealso cref="Logger" />
	internal class OpenPath
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(OpenPath));

		internal OpenPath()
		{
			ConfigureLogger();
		}

		private static void ConfigureLogger()
		{
			Logger.Configure();
		}

		internal ToolStripMenuItem CreateToolStripMenuItem(string menuType, string shortcutTargetFolder)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = "FileShortcut" == menuType ? Strings.openFileShortcutTargetFolderText : Strings.openFolderShortcutTargetFolderText,
				Image = "FileShortcut" == menuType ? Resources.imgPowershellElevated : Resources.imgPowershell
			};

			//  Add click action.
			toolStripMenuItem.Click += (sender, args) => DoClickAction(shortcutTargetFolder);

			return toolStripMenuItem;
		}

		private static void DoClickAction(string shortcutTargetFolder)
		{
			StartProcess(shortcutTargetFolder);
		}

		private static void StartProcess(string shortcutTargetFolder)
		{
			Process.Start(new ProcessStartInfo() {
				WorkingDirectory = @"C:\Windows\System32",
				FileName = shortcutTargetFolder,
				Verb = "open",
				UseShellExecute = true
			});
		}
	}
}