using System.Windows.Forms;
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.ShellServer;

namespace Sonnenberg.ShellServer.Tests
{
    /// <summary>This class contains parameterized unit tests for ContextMenu</summary>
    [TestFixture]
    [PexClass(typeof(ContextMenu))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ContextMenuTest
    {

		/// <summary>Test stub for .ctor()</summary>
		[PexMethod]
		internal ContextMenu ConstructorTest()
		{
			ContextMenu target = new ContextMenu();
			return target;
			// TODO: add assertions to method ContextMenuTest.ConstructorTest()
		}

		/// <summary>Test stub for CanShowMenu(ShellServer)</summary>
		[PexMethod]
		internal bool CanShowMenuTest([PexAssumeUnderTest]ContextMenu target, global::Sonnenberg.ShellServer.ShellServer shellServer)
		{
			bool result = target.CanShowMenu(shellServer);
			return result;
			// TODO: add assertions to method ContextMenuTest.CanShowMenuTest(ContextMenu, ShellServer)
		}

		/// <summary>Test stub for CreateMenu(ShellServer)</summary>
		[PexMethod]
		internal ContextMenuStrip CreateMenuTest([PexAssumeUnderTest]ContextMenu target, global::Sonnenberg.ShellServer.ShellServer shellServer)
		{
			ContextMenuStrip result = target.CreateMenu(shellServer);
			return result;
			// TODO: add assertions to method ContextMenuTest.CreateMenuTest(ContextMenu, ShellServer)
		}

		/// <summary>Test stub for Dispose()</summary>
		[PexMethod]
		internal void DisposeTest([PexAssumeUnderTest]ContextMenu target)
		{
			target.Dispose();
			// TODO: add assertions to method ContextMenuTest.DisposeTest(ContextMenu)
		}
	}
}
