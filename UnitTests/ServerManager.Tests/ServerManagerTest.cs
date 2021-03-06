// <copyright file="ShellServerManagerTest.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using System;

//using ServerManager = Sonnenberg.ServerManager.ServerManager;

namespace Sonnenberg.UnitTests.ServerManager.Tests
{
    /// <summary>This class contains parameterized unit tests for ShellServerManager</summary>
    [PexClass(typeof(Sonnenberg.ServerManager.Program))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public class ServerManagerTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public Sonnenberg.ServerManager.Program ConstructorTest()
        {
            var target = new Sonnenberg.ServerManager.Program();
            return target;
        }

        /// <summary>Test stub for StartShellServer(ShellServer)</summary>
        [PexMethod]
        public void StartShellServerTest(
            [PexAssumeUnderTest] Sonnenberg.ServerManager.Program target,
            ShellServer.Program server
        )
        {
            target.StartShellServer(server);
        }

        /// <summary>Test stub for StopShellServer(ShellServer)</summary>
        [PexMethod]
        public void StopShellServerTest(
            [PexAssumeUnderTest] Sonnenberg.ServerManager.Program target,
            ShellServer.Program server
        )
        {
            target.StopShellServer(server);
        }
    }
}