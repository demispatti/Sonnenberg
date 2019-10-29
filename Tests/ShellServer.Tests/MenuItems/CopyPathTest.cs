using Sonnenberg.ShellServer;
using System.Windows.Forms;
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.ShellServer.MenuItems;

namespace Sonnenberg.ShellServer.MenuItems.Tests
{
    /// <summary>This class contains parameterized unit tests for CopyPath</summary>
    [TestFixture]
    [PexClass(typeof(CopyPath))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class CopyPathTest
    {

		/// <summary>Test stub for .ctor()</summary>
		[PexMethod]
		internal CopyPath ConstructorTest()
		{
			CopyPath target = new CopyPath();
			return target;
			// TODO: add assertions to method CopyPathTest.ConstructorTest()
		}

		/// <summary>Test stub for CreateToolStripMenuItem(String, String, Boolean)</summary>
		[PexMethod]
		internal ToolStripMenuItem CreateToolStripMenuItemTest(
			[PexAssumeUnderTest]CopyPath target,
			string menuType,
			string clickedItemPath,
			bool forwardSlashes
		)
		{
			ToolStripMenuItem result = target.CreateToolStripMenuItem(menuType, clickedItemPath, forwardSlashes)
			  ;
			return result;
			// TODO: add assertions to method CopyPathTest.CreateToolStripMenuItemTest(CopyPath, String, String, Boolean)
		}

		/// <summary>Test stub for GetClickedItemPath(String, ShellServer)</summary>
		[PexMethod]
		internal string GetClickedItemPathTest(string menuType, global::Sonnenberg.ShellServer.ShellServer shellServer)
		{
			string result = CopyPath.GetClickedItemPath(menuType, shellServer);
			return result;
			// TODO: add assertions to method CopyPathTest.GetClickedItemPathTest(String, ShellServer)
		}
	}
}
