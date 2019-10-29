using Sonnenberg.ShellServer.CustomEvent;
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using Sonnenberg.ShellServer.MenuWatcher;

namespace Sonnenberg.Tests.ShellServer.MenuWatcher.Tests
{
    /// <summary>This class contains parameterized unit tests for Watcher</summary>
    [TestFixture]
    [PexClass(typeof(/*global::Sonnenberg.ShellServer.MenuWatcher.*/Watcher))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class WatcherTest
    {

		/// <summary>Test stub for .ctor(EventPublisher)</summary>
		[PexMethod]
		internal Watcher ConstructorTest(EventPublisher eventPublisher)
		{
			Watcher target = new Watcher(eventPublisher);
			return target;
			// TODO: add assertions to method WatcherTest.ConstructorTest(EventPublisher)
		}

		/// <summary>Test stub for Dispose()</summary>
		[PexMethod]
		internal void DisposeTest([PexAssumeUnderTest]Watcher target)
		{
			target.Dispose();
			// TODO: add assertions to method WatcherTest.DisposeTest(Watcher)
		}

		/// <summary>Test stub for Watch()</summary>
		[PexMethod]
		internal void WatchTest([PexAssumeUnderTest]Watcher target)
		{
			target.Watch();
			// TODO: add assertions to method WatcherTest.WatchTest(Watcher)
		}
	}
}
