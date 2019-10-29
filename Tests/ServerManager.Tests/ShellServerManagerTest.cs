// <copyright file="ShellServerManagerTest.cs">Copyright © Demis Patti 2019</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;

namespace Sonnenberg.ServerManager.Tests
{
    /// <summary>This class contains parameterized unit tests for ShellServerManager</summary>
    [PexClass(typeof(ShellServerManager))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public class ShellServerManagerTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public ShellServerManager ConstructorTest()
        {
            var target = new ShellServerManager();
            return target;
            // TODO: add assertions to method ShellServerManagerTest.ConstructorTest()
        }

        /// <summary>Test stub for StartShellServer(ShellServer)</summary>
        [PexMethod]
        public void StartShellServerTest(
            [PexAssumeUnderTest] ShellServerManager target,
            ShellServer.ShellServer server
        )
        {
            target.StartShellServer(server);
            // TODO: add assertions to method ShellServerManagerTest.StartShellServerTest(ShellServerManager, ShellServer)
        }

        /// <summary>Test stub for StopShellServer(ShellServer)</summary>
        [PexMethod]
        public void StopShellServerTest(
            [PexAssumeUnderTest] ShellServerManager target,
            ShellServer.ShellServer server
        )
        {
            target.StopShellServer(server);
            // TODO: add assertions to method ShellServerManagerTest.StopShellServerTest(ShellServerManager, ShellServer)
        }
    }
}