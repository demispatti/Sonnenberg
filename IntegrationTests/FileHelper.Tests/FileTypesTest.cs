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

    }
}