// <copyright file="ServiceTest.cs">Copyright © Demis Patti 2019</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;

namespace Sonnenberg.WindowsService.Tests
{
    /// <summary>This class contains parameterized unit tests for Service</summary>
    [PexClass(typeof(Service))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public class ServiceTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public Service ConstructorTest()
        {
            var target = new Service();
            return target;
            // TODO: add assertions to method ServiceTest.ConstructorTest()
        }
    }
}