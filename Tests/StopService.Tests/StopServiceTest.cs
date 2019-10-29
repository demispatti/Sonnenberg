// <copyright file="StopServiceTest.cs">Copyright © Demis Patti 2019</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.StopService;

namespace Sonnenberg.Tests.StopService.Tests
{
    /// <summary>This class contains parameterized unit tests for StopService</summary>
    [PexClass(typeof(global::Sonnenberg.StopService.StopService))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class StopServiceTest
    {
    }
}
