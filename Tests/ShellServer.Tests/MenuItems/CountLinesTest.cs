using System.Collections.Generic;
using System.Windows.Forms;
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.ShellServer.MenuItems;

namespace Sonnenberg.ShellServer.MenuItems.Tests
{
    /// <summary>This class contains parameterized unit tests for CountLines</summary>
    [TestFixture]
    [PexClass(typeof(CountLines))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class CountLinesTest
    {

		/// <summary>Test stub for .ctor()</summary>
		[PexMethod]
		internal CountLines ConstructorTest()
		{
			CountLines target = new CountLines();
			return target;
			// TODO: add assertions to method CountLinesTest.ConstructorTest()
		}

		/// <summary>Test stub for CreateToolStripMenuItem(IEnumerable`1&lt;String&gt;, Boolean)</summary>
		[PexMethod]
		internal ToolStripMenuItem CreateToolStripMenuItemTest(
			[PexAssumeUnderTest]CountLines target,
			IEnumerable<string> selectedItemPaths,
			bool doOmitEmptyLines
		)
		{
			ToolStripMenuItem result = target.CreateToolStripMenuItem(selectedItemPaths, doOmitEmptyLines);
			return result;
			// TODO: add assertions to method CountLinesTest.CreateToolStripMenuItemTest(CountLines, IEnumerable`1<String>, Boolean)
		}
	}
}
