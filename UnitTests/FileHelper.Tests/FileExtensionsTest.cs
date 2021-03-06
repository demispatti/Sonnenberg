// <copyright file="FileExtensionsTest.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.FileHelper;
using System;

namespace Sonnenberg.UnitTests.FileHelper.Tests
{
    /// <summary>This class contains parameterized unit tests for FileExtensions</summary>
    [PexClass(typeof(FileExtensions))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class FileExtensionsTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public FileExtensions ConstructorTest()
        {
            FileExtensions target = new FileExtensions();
            return target;
        }
    }
}