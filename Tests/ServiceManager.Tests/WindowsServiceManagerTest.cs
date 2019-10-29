// <copyright file="WindowsServiceManagerTest.cs">Copyright © Demis Patti 2019</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.ServiceManager;

namespace Sonnenberg.ServiceManager.Tests
{
    /// <summary>This class contains parameterized unit tests for WindowsServiceManager</summary>
    [PexClass(typeof(WindowsServiceManager))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class WindowsServiceManagerTest
    {
        /// <summary>Test stub for StartShellServer(String[])</summary>
        [PexMethod]
        public void StartShellServerTest([PexAssumeUnderTest] WindowsServiceManager target, string[] args)
        {
            target.StartShellServer();
            // TODO: add assertions to method WindowsServiceManagerTest.StartShellServerTest(WindowsServiceManager, String[])
        }

        /// <summary>Test stub for StopShellServer()</summary>
        [PexMethod]
        public void StopShellServerTest([PexAssumeUnderTest] WindowsServiceManager target)
        {
            target.StopShellServer();
            // TODO: add assertions to method WindowsServiceManagerTest.StopShellServerTest(WindowsServiceManager)
        }
    }
}