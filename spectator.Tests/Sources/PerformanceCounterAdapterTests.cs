using System;
using NUnit.Framework;
using spectator.Sources.PerformanceCounters;

namespace spectator.Tests.Sources
{
    [TestFixture]
    public class PerformanceCounterAdapterTests
    {
        [Test]
        public void when_performance_counter_does_not_exist_then_an_exception_is_thrown()
        {
            Assert.Throws<InvalidOperationException>(() => new PerformanceCounterAdapter("does", "not", "exist"));
        }
    }
}