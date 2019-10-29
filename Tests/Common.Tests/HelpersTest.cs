using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.Common;

namespace Sonnenberg.Tests.Common.Tests
{
    /// <summary>This class contains parameterized unit tests for Helpers</summary>
    [TestFixture]
    [PexClass(typeof(Helpers))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class HelpersTest
    {

		/// <summary>Test stub for DelayedMessageBox(String)</summary>
		[PexMethod]
		public void DelayedMessageBoxTest(string message)
		{
			Helpers.DelayedMessageBox(message);
			// TODO: add assertions to method HelpersTest.DelayedMessageBoxTest(String)
		}
	}
}
