// <copyright file="ContextMenuTest.cs">Copyright ©  2019</copyright>
using System;
using System.Windows.Forms;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using SharpShell;
using Sonnenberg.ContextMenu;

//using Sonnenberg.ContextMenu;

namespace Sonnenberg.UnitTests.ContextMenu.Tests
{
    /// <summary>This class contains parameterized unit tests for ShellContextMenu</summary>
    [PexClass(typeof(Sonnenberg.ContextMenu.Program))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class ContextMenuTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public Sonnenberg.ContextMenu.Program ConstructorTest()
        {
			Sonnenberg.ContextMenu.Program target = new Sonnenberg.ContextMenu.Program();
            return target;
        }

        /// <summary>Test stub for Dispose()</summary>
        [PexMethod]
        public void DisposeTest([PexAssumeUnderTest] Sonnenberg.ContextMenu.Program target)
        {
            target.Dispose();
        }

        /// <summary>Test stub for GetFileMenu(String, String, ShellExtInitServer)</summary>
        [PexMethod]
        public ContextMenuStrip GetFileMenuTest(
            [PexAssumeUnderTest] Sonnenberg.ContextMenu.Program target,
            string menuType,
            string clickedItemPath,
            ShellExtInitServer shellServer
        )
        {
            ContextMenuStrip result
               = target.GetFileMenu(menuType, clickedItemPath, shellServer);
            return result;
        }

        /// <summary>Test stub for GetFolderMenu(String, String, String)</summary>
        [PexMethod]
        public ContextMenuStrip GetFolderMenuTest(
            [PexAssumeUnderTest] Sonnenberg.ContextMenu.Program target,
            string menuType,
            string clickedItemPath,
            string shellStartUpDirectory
        )
        {
            ContextMenuStrip result
               = target.GetFolderMenu(menuType, clickedItemPath, shellStartUpDirectory);
            return result;
        }
    }
}
