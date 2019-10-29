
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpShell;
using Sonnenberg.Language;
using Sonnenberg.Common;
using Sonnenberg.ShellServer.Properties;

namespace Sonnenberg.ShellServer.MenuItems
{
	/// <summary>
	/// 	The class responsible for creating a <c>ToolStripMenuItem</c> and its click action.
	/// 	It starts a CMD-, Git-CMD- or Git-Bash-Process and cds into the respective directory.
	/// 	The processes can be started with limited or elevated privileges.
	/// </summary>
	/// <remarks>
	///		- Creates a ToolStripMenuItem
	/// 	- Creates a click action
	/// 	- Checks that the requested executable is installed
	/// 	- Determines the directory where the clicked item resides
	/// 	- Determines the directory the CMD process will cd into
	/// 	- Assembles the required <c>StartInfo</c> parameters
	/// 	- Starts a CMD-, Git-CMD- or Git-Bash-Process and cds into the respective directory
	/// </remarks>
	internal class OpenCmd
	{
		internal OpenCmd()
		{
			ConfigureLogger();
		}

		/// <summary>
		/// 	Triggers the logger class to configure the logger.
		/// </summary>
		/// <seealso cref="Sonnenberg.Common.Logger" />
		private static void ConfigureLogger()
		{
			Logger.Configure();
		}

		internal ToolStripMenuItem CreateToolStripMenuItem(string menuType, string shellStartUpDirectory, string shellExecutableName, bool runElevated)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = GetToolStripMenuItemText(shellExecutableName, menuType, runElevated),
				Image = GetToolStripMenuItemImage(shellExecutableName, runElevated)
			};

			//  Add click action.
			toolStripMenuItem.Click += (sender, args) => DoClickAction(GetProcessStartInfoParameters(shellStartUpDirectory, shellExecutableName, runElevated));

			return toolStripMenuItem;
		}

		private string GetToolStripMenuItemText(string shellExecutableName, string menuType, bool runElevated)
		{
			string text;

			if ("powershell" == shellExecutableName)
			{
				text = runElevated ? Strings.openPowershellHereAsAdminText : Strings.openPowershellHereText;

				if ("folder" == menuType)
				{
					text = runElevated ? Strings.openPowershellInsideAsAdminText : Strings.openPowershellInsideText;
				}
			}
			else
			{
				text = runElevated ? Strings.openCmdHereAsAdminText : Strings.openCmdHereText;

				if ("folder" == menuType)
				{
					text = runElevated ? Strings.openCmdInsideAsAdminText : Strings.openCmdInsideText;
				}
			}

			return text;
		}

		private Bitmap GetToolStripMenuItemImage(string shellExecutableName, bool runElevated)
		{
			if ("powershell" == shellExecutableName)
			{

				return runElevated ? Resources.imgPowershellElevated : Resources.imgPowershell;
			}

			return runElevated ? Resources.imgCmdUac : Resources.imgCmd;
		}

		private Dictionary<string, string> GetProcessStartInfoParameters(string shellStartUpDirectory, string shellExecutableName, bool runElevated)
		{
			dynamic parameters = new Dictionary<string, string>();

			// Assemble required <c>ProcessStartInfo</c> parameters
			if ("powershell" == shellExecutableName)
			{
				parameters["WorkingDirectory"] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
				parameters["FileName"] = "powershell.exe";
				parameters["Arguments"] = $" -ExecutionPolicy Bypass -NoExit cd '{shellStartUpDirectory}';";
				parameters["Verb"] = runElevated ? "runas" : "";
			}
			else
			{
				parameters["WorkingDirectory"] = @"C:\Windows\System32";
				parameters["FileName"] = "shellExecutableName.exe";
				parameters["Arguments"] = $" /K cd {shellStartUpDirectory}";
				parameters["Verb"] = runElevated ? "runas" : "";
			}

			return parameters;
		}

		private static void DoClickAction(dynamic parameters)
		{
			var processStartInfo = new ProcessStartInfo
			{
				WorkingDirectory = parameters["WorkingDirectory"],
				FileName = parameters["FileName"],
				Arguments = parameters["Arguments"],
				Verb = parameters["Verb"]
			};
			Process.Start(processStartInfo);
		}

		public static bool AppExists(string appName)
		{
			return "powershell.exe" == appName ? File.Exists("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe") : File.Exists("C:\\Windows\\System32\\shellExecutableName.exe");
		}

		public static string GetShellStartUpDirectory(string menuType, ShellServer shellServer)
		{
			return CmdTargetDirectory(menuType, shellServer);
		}

		private static string CmdTargetDirectory(string menuType, ShellExtInitServer shellServer)
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
	}
}