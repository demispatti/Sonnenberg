// <copyright file="WindowsServiceManagerTest.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using System;

namespace Sonnenberg.UnitTests.ServiceManager.Tests
{
    /// <summary>This class contains parameterized unit tests for WindowsServiceManager</summary>
    [PexClass(typeof(Sonnenberg.ServiceManager.Program))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class ServiceManagerTest
    {
        /// <summary>Test stub for StartShellServer(String[])</summary>
        [PexMethod]
        public void StartShellServerTest([PexAssumeUnderTest] Sonnenberg.ServiceManager.Program target, string[] args)
        {
            target.StartShellServer();
        }

        /// <summary>Test stub for StopShellServer()</summary>
        [PexMethod]
        public void StopShellServerTest([PexAssumeUnderTest] Sonnenberg.ServiceManager.Program target)
        {
            target.StopShellServer();
        }
    }
}