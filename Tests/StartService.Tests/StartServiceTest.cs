// <copyright file="StartServiceTest.cs">Copyright © Demis Patti 2019</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.StartService;

namespace Sonnenberg.Tests.StartService.Tests
{
    /// <summary>This class contains parameterized unit tests for StartService</summary>
    [PexClass(typeof(Sonnenberg.StartService.StartService))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class StartServiceTest
    {
    }
}
