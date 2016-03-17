using NUnit.Framework;
using spectator.Configuration;
using spectator.Sources;
using spectator.Sources.PerformanceCounters;

namespace spectator.Tests.Sources
{
    [TestFixture]
    public class QueryableSourceFactoryTests
    {
        [Test]
        public void supports_performance_counters()
        {
            var sourceFactory = new QueryableSourceFactory();

            var source = sourceFactory.Create(MetricSource.PerformanceCounter);

            Assert.That(source, Is.InstanceOf<PerformanceCounterSource>());
        }

        [Test]
        public void supports_windows_management_instrumentation()
        {
            var sourceFactory = new QueryableSourceFactory();

            var source = sourceFactory.Create(MetricSource.WindowsManagementInstrumentation);

            Assert.That(source, Is.InstanceOf<WindowsManagementInstrumentationSource>());
        }
    }
}
