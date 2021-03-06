// <copyright file="ContextMenuTest.cs">Copyright ©  2019</copyright>
using System;
using System.Windows.Forms;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using SharpShell;
using Sonnenberg.ContextMenu;

//using Sonnenberg.ContextMenu;

namespace Sonnenberg.IntegrationTests.ContextMenu.Tests
{
    /// <summary>This class contains parameterized unit tests for ShellContextMenu</summary>
    [PexClass(typeof(Sonnenberg.ContextMenu.Program))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class ContextMenuTest
    {

    }
}
