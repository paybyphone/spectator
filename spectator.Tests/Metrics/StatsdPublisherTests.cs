using System;
using Moq;
using NUnit.Framework;
using spectator.Configuration;
using spectator.Metrics;
using StatsdClient;

namespace spectator.Tests.Metrics
{
    [TestFixture]
    public class StatsdPublisherTests
    {
        private StatsdPublisher _statsdPublisher;
        private Mock<IStatsd> _mockStatsdClient;

        [SetUp]
        public void SetUp()
        {
            _mockStatsdClient = new Mock<IStatsd>();
            _statsdPublisher = new StatsdPublisher(_mockStatsdClient.Object);
        }

        [Test]
        public void gauge_metrics_are_supported()
        {
            Assert.DoesNotThrow(() => _statsdPublisher.Publish(Some.String(), Some.Double(), MetricType.Gauge));
        }

        [TestCase(MetricType.Count)]
        [TestCase(MetricType.Timing)]
        public void other_types_of_metric_are_not_supported(MetricType metricType)
        {
            Assert.Throws<NotSupportedException>(() => _statsdPublisher.Publish(Some.String(), Some.Double(), metricType));
        }
    }
}