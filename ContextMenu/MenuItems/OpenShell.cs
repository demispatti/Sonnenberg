using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
	internal class OpenShell
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(OpenShell));

		internal OpenShell()
		{
			ConfigureLogger();
		}

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

			if ("powershell.exe" == shellExecutableName)
			{
				text = runElevated ? Strings.openPowershellHereElevatedText : Strings.openPowershellHereText;

				if ("folder" == menuType)
				{
					text = runElevated ? Strings.openPowershellInsideElevatedText : Strings.openPowershellInsideText;
				}
			}
			else
			{
				text = runElevated ? Strings.openCmdHereElevatedText : Strings.openCmdHereText;

				if ("folder" == menuType)
				{
					text = runElevated ? Strings.openCmdInsideElevatedText : Strings.openCmdInsideText;
				}
			}

			return text;
		}

		private Bitmap GetToolStripMenuItemImage(string shellExecutableName, bool runElevated)
		{
			if ("powershell.exe" == shellExecutableName)
			{
				return runElevated ? Resources.imgPowershellElevated : Resources.imgPowershell;
			}

			return runElevated ? Resources.imgCmdElevated : Resources.imgCmd;
		}

		private Dictionary<string, string> GetProcessStartInfoParameters(string shellStartUpDirectory, string shellExecutableName, bool runElevated)
		{
			dynamic parameters = new Dictionary<string, string>();

			// Assemble required <c>ProcessStartInfo</c> parameters
			if ("powershell.exe" == shellExecutableName)
			{
				parameters["WorkingDirectory"] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
				parameters["FileName"] = "powershell.exe";
				parameters["Arguments"] = $" -ExecutionPolicy Bypass -NoExit cd '{shellStartUpDirectory}';";
				parameters["Verb"] = runElevated ? "runas" : "";
			}
			else
			{
				parameters["WorkingDirectory"] = @"C:\Windows\System32";
				parameters["FileName"] = "cmd.exe";
				parameters["Arguments"] = $" /K cd {shellStartUpDirectory}";
				parameters["Verb"] = runElevated ? "runas" : "";
			}

			return parameters;
		}

		private static void DoClickAction(Dictionary<string, string> parameters)
		{
			StartProcess(parameters);
		}

		private static void StartProcess(Dictionary<string, string> parameters)
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
			return "powershell.exe" == appName ? File.Exists("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe") : File.Exists("C:\\Windows\\System32\\cmd.exe");
		}
	}
}