// <copyright file="LoggerTest.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.Common;
using System;

namespace Sonnenberg.IntegrationTests.Common.Tests
{
    /// <summary>This class contains parameterized unit tests for Logger</summary>
    [PexClass(typeof(Logger))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class LoggerTest
    {
        /// <summary>Test stub for AddAppDataDi§rectory()</summary>
        /*[PexMethod]
        public void AddAppDataDirectoryTest()
        {
            Logger.AddAppDataDirectory();
            // TODO: add assertions to method LoggerTest.AddAppDataDirectoryTest()
        }*/

        /// <summary>Test stub for AddLogfile()</summary>
        /*[PexMethod]
        public void AddLogfileTest()
        {
            Logger.AddLogfile();
            // TODO: add assertions to method LoggerTest.AddLogfileTest()
        }*/

        /// <summary>Test stub for Configure()</summary>
        [PexMethod]
        public void ConfigureTest()
        {
            Logger.Configure();
            // TODO: add assertions to method LoggerTest.ConfigureTest()
        }
    }
}