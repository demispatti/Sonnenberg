using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.FileHelper;
using System;

namespace Sonnenberg.IntegrationTests.FileHelper.Tests
{
    /// <summary>This class contains parameterized unit tests for FileTypes</summary>
    [TestFixture]
    [PexClass(typeof(FileTypes))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class FileTypesTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public FileTypes ConstructorTest()
        {
            FileTypes target = new FileTypes();
            return target;
            // TODO: add assertions to method FileTypesTest.ConstructorTest()
        }

        /// <summary>Test stub for GetFileType(String)</summary>
        [PexMethod]
        public string GetFileTypeTest([PexAssumeUnderTest] FileTypes target, string ext)
        {
            string result = target.GetFileType(ext);
            return result;
            // TODO: add assertions to method FileTypesTest.GetFileTypeTest(FileTypes, String)
        }
    }
}