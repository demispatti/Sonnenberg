using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.ShellServer.CustomEvent;

namespace Sonnenberg.Tests.ShellServer.CustomEvent.Tests
{
    /// <summary>This class contains parameterized unit tests for EventPublisher</summary>
    [TestFixture]
    [PexClass(typeof(EventPublisher))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class EventPublisherTest
    {

		/// <summary>Test stub for PublishCustomEvent()</summary>
		[PexMethod]
		public void PublishCustomEventTest([PexAssumeUnderTest]EventPublisher target)
		{
			target.PublishCustomEvent();
			// TODO: add assertions to method EventPublisherTest.PublishCustomEventTest(EventPublisher)
		}
	}
}
