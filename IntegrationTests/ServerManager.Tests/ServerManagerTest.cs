// <copyright file="ShellServerManagerTest.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using System;

//using ServerManager = Sonnenberg.ServerManager.ServerManager;

namespace Sonnenberg.IntegrationTests.ServerManager.Tests
{
    /// <summary>This class contains parameterized unit tests for ShellServerManager</summary>
    [PexClass(typeof(Sonnenberg.ServerManager.Program))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public class ServerManagerTest
    {

    }
}