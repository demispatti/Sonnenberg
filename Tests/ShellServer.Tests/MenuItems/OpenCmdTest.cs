using Sonnenberg.ShellServer;
using System.Windows.Forms;
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.ShellServer.MenuItems;

namespace Sonnenberg.ShellServer.MenuItems.Tests
{
    /// <summary>This class contains parameterized unit tests for OpenShell</summary>
    [TestFixture]
    [PexClass(typeof(OpenShell))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class OpenCmdTest
    {

		/// <summary>Test stub for .ctor()</summary>
		[PexMethod]
		internal OpenShell ConstructorTest()
		{
			OpenShell target = new OpenShell();
			return target;
			// TODO: add assertions to method OpenCmdTest.ConstructorTest()
		}

		/// <summary>Test stub for AppExists(String)</summary>
		[PexMethod]
		internal bool AppExistsTest(string appName)
		{
			bool result = OpenShell.AppExists(appName);
			return result;
			// TODO: add assertions to method OpenCmdTest.AppExistsTest(String)
		}

		/// <summary>Test stub for CreateToolStripMenuItem(String, String, String, Boolean)</summary>
		[PexMethod]
		internal ToolStripMenuItem CreateToolStripMenuItemTest(
			[PexAssumeUnderTest]OpenShell target,
			string menuType,
			string shellStartUpDirectory,
			string shellExecutableName,
			bool runElevated
		)
		{
			ToolStripMenuItem result = target.CreateToolStripMenuItem
										   (menuType, shellStartUpDirectory, shellExecutableName, runElevated);
			return result;
			// TODO: add assertions to method OpenCmdTest.CreateToolStripMenuItemTest(OpenShell, String, String, String, Boolean)
		}

		/// <summary>Test stub for GetShellStartUpDirectory(String, ShellServer)</summary>
		[PexMethod]
		internal string GetShellStartUpDirectoryTest(string menuType, global::Sonnenberg.ShellServer.ShellServer shellServer)
		{
			string result = OpenShell.GetShellStartUpDirectory(menuType, shellServer);
			return result;
			// TODO: add assertions to method OpenCmdTest.GetShellStartUpDirectoryTest(String, ShellServer)
		}
	}
}
