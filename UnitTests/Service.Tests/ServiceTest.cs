// <copyright file="ServiceTest.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using System;

namespace Sonnenberg.UnitTests.Service.Tests
{
    /// <summary>This class contains parameterized unit tests for Service</summary>
    [PexClass(typeof(Sonnenberg.Service.Service))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public class ServiceTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public Sonnenberg.Service.Service ConstructorTest()
        {
            var target = new Sonnenberg.Service.Service();
            return target;
        }
    }
}