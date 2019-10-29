// <copyright file="ShellServerTest.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using System;
using Sonnenberg.ShellServer;

namespace Sonnenberg.IntegrationTests.ShellServer.Tests
{
    /// <summary>This class contains parameterized unit tests for ShellServer</summary>
    [PexClass(typeof(global::Sonnenberg.ShellServer.ShellServer))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class ShellServerTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public global::Sonnenberg.ShellServer.ShellServer ConstructorTest()
        {
            global::Sonnenberg.ShellServer.ShellServer target
                = new global::Sonnenberg.ShellServer.ShellServer();
            return target;
            // TODO: add assertions to method ShellServerTest.ConstructorTest()
        }

        /// <summary>Test stub for Dispose()</summary>
        [PexMethod]
        public void DisposeTest([PexAssumeUnderTest] global::Sonnenberg.ShellServer.ShellServer target)
        {
            target.Dispose();
            // TODO: add assertions to method ShellServerTest.DisposeTest(ShellServer)
        }
    }
}