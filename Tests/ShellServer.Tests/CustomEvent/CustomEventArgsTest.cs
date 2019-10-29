using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.ShellServer.CustomEvent;

namespace Sonnenberg.Tests.ShellServer.CustomEvent.Tests
{
    /// <summary>This class contains parameterized unit tests for CustomEventArgs</summary>
    [TestFixture]
    [PexClass(typeof(CustomEventArgs))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class CustomEventArgsTest
    {

		/// <summary>Test stub for .ctor(Boolean)</summary>
		[PexMethod]
		public CustomEventArgs ConstructorTest(bool b)
		{
			CustomEventArgs target = new CustomEventArgs(b);
			return target;
			// TODO: add assertions to method CustomEventArgsTest.ConstructorTest(Boolean)
		}
	}
}
