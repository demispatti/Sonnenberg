// <copyright file="ShellServerManagerTest.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using System;

//using ServerManager = Sonnenberg.ServerManager.ServerManager;

namespace Sonnenberg.IntegrationTests.ServerManager.Tests
{
    /// <summary>This class contains parameterized unit tests for ShellServerManager</summary>
    [PexClass(typeof(Sonnenberg.ServerManager.ServerManager))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public class ServerManagerTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public Sonnenberg.ServerManager.ServerManager ConstructorTest()
        {
            var target = new Sonnenberg.ServerManager.ServerManager();
            return target;
            // TODO: add assertions to method ShellServerManagerTest.ConstructorTest()
        }

        /// <summary>Test stub for StartShellServer(ShellServer)</summary>
        [PexMethod]
        public void StartShellServerTest(
            [PexAssumeUnderTest] Sonnenberg.ServerManager.ServerManager target,
            ShellServer.ShellServer server
        )
        {
            target.StartShellServer(server);
            // TODO: add assertions to method ShellServerManagerTest.StartShellServerTest(ShellServerManager, ShellServer)
        }

        /// <summary>Test stub for StopShellServer(ShellServer)</summary>
        [PexMethod]
        public void StopShellServerTest(
            [PexAssumeUnderTest] Sonnenberg.ServerManager.ServerManager target,
            ShellServer.ShellServer server
        )
        {
            target.StopShellServer(server);
            // TODO: add assertions to method ShellServerManagerTest.StopShellServerTest(ShellServerManager, ShellServer)
        }
    }
}