using System.Windows.Forms;
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.ShellServer.MenuItems;

namespace Sonnenberg.Tests.ShellServer.MenuItems.Tests
{
    /// <summary>This class contains parameterized unit tests for ImageInfo</summary>
    [TestFixture]
    [PexClass(typeof(ImageInfo))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ImageInfoTest
    {

		/// <summary>Test stub for .ctor()</summary>
		[PexMethod]
		internal ImageInfo ConstructorTest()
		{
			ImageInfo target = new ImageInfo();
			return target;
			// TODO: add assertions to method ImageInfoTest.ConstructorTest()
		}

		/// <summary>Test stub for CreateToolStripMenuItem(String)</summary>
		[PexMethod]
		internal ToolStripMenuItem CreateToolStripMenuItemTest([PexAssumeUnderTest]ImageInfo target, string filePath)
		{
			ToolStripMenuItem result = target.CreateToolStripMenuItem(filePath);
			return result;
			// TODO: add assertions to method ImageInfoTest.CreateToolStripMenuItemTest(ImageInfo, String)
		}
	}
}
