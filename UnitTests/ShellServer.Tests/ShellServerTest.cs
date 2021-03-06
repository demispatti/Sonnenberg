// <copyright file="ShellServerTest.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using System;
using Sonnenberg.ShellServer;

namespace Sonnenberg.UnitTests.ShellServer.Tests
{
    /// <summary>This class contains parameterized unit tests for ShellServer</summary>
    [PexClass(typeof(global::Sonnenberg.ShellServer.Program))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class ShellServerTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public global::Sonnenberg.ShellServer.Program ConstructorTest()
        {
            global::Sonnenberg.ShellServer.Program target
                = new global::Sonnenberg.ShellServer.Program();
            return target;
        }

        /// <summary>Test stub for Dispose()</summary>
        [PexMethod]
        public void DisposeTest([PexAssumeUnderTest] global::Sonnenberg.ShellServer.Program target)
        {
            target.Dispose();
        }
    }
}