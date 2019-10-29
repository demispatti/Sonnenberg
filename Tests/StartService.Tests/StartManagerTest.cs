// <copyright file="StartManagerTest.cs">Copyright © Demis Patti 2019</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.StartService;

namespace Sonnenberg.StartService.Tests
{
    /// <summary>This class contains parameterized unit tests for StartManager</summary>
    [PexClass(typeof(StartService))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class StartManagerTest
    {
    }
}