using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using spectator.Configuration;
using spectator.Metrics;
using spectator.Sources;

namespace spectator.Tests
{
    [TestFixture]
    public class SpectatorServiceTests
    {
        private Mock<ISpectatorConfiguration> _configuration;
        private Mock<IQueryableSource> _source;
        private Mock<IQueryableSourceFactory> _sourceFactory;
        private Mock<IMetricPublisher> _publisher;
        private Mock<IMetricFormatter> _formatter;

        private SpectatorServiceTestHarness _service;

        [SetUp]
        public void SetUp()
        {
            _configuration = new Mock<ISpectatorConfiguration>();
            _source = new Mock<IQueryableSource>();
            _sourceFactory = new Mock<IQueryableSourceFactory>();
            _publisher = new Mock<IMetricPublisher>();
            _formatter = new Mock<IMetricFormatter>();
            _service = new SpectatorServiceTestHarness(_configuration.Object, _sourceFactory.Object, _publisher.Object, _formatter.Object);

            var metricConfiguration = new MetricConfiguration();
            _sourceFactory.Setup(m => m.Create(metricConfiguration.Source)).Returns(_source.Object);
            _source.Setup(m => m.QueryValue(metricConfiguration.Path)).Returns(new[] { new Sample("instance", 10d) });
            _formatter.Setup(m => m.Format(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("metricname");
            _configuration.Setup(m => m.Metrics).Returns(new List<MetricConfiguration> { metricConfiguration });

            _service.Spectate();
        }

        [Test]
        public void sources_are_queried_for_metric_samples()
        {
            _source.Verify(m => m.QueryValue(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void measured_samples_are_published()
        {
            _publisher.Verify(m => m.Publish(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<MetricType>()), Times.Once);
        }


        [Test]
        public void exceptions_do_not_halt_execution()
        {
            _sourceFactory.Reset();
            _sourceFactory.Setup(m => m.Create(It.IsAny<MetricSource>())).Throws(new Exception());

            var service = new SpectatorServiceTestHarness(_configuration.Object, _sourceFactory.Object, _publisher.Object, _formatter.Object);

            Assert.DoesNotThrow(() => service.Spectate());
        }
    }
}
