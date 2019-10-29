// <copyright file="WindowsServiceManagerTest.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using System;

namespace Sonnenberg.IntegrationTests.ServiceManager.Tests
{
    /// <summary>This class contains parameterized unit tests for WindowsServiceManager</summary>
    [PexClass(typeof(Sonnenberg.ServiceManager.ServiceManager))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class ServiceManagerTest
    {
        /// <summary>Test stub for StartShellServer(String[])</summary>
        [PexMethod]
        public void StartShellServerTest([PexAssumeUnderTest] Sonnenberg.ServiceManager.ServiceManager target, string[] args)
        {
            target.StartShellServer();
            // TODO: add assertions to method WindowsServiceManagerTest.StartShellServerTest(WindowsServiceManager, String[])
        }

        /// <summary>Test stub for StopShellServer()</summary>
        [PexMethod]
        public void StopShellServerTest([PexAssumeUnderTest] Sonnenberg.ServiceManager.ServiceManager target)
        {
            target.StopShellServer();
            // TODO: add assertions to method WindowsServiceManagerTest.StopShellServerTest(WindowsServiceManager)
        }
    }
}