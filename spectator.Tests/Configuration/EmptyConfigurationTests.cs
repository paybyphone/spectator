using System;
using NUnit.Framework;
using spectator.Configuration;

namespace spectator.Tests.Configuration
{
    [TestFixture]
    public class EmptyConfigurationTests
    {
        private EmptyConfiguration _emptyConfig;

        [SetUp]
        public void SetUp()
        {
            _emptyConfig = new EmptyConfiguration();
        }

        [Test]
        public void empty_configuration_has_empty_statsd_host()
        {
            Assert.That(_emptyConfig.StatsdHost, Is.EqualTo(string.Empty));
        }

        [Test]
        public void empty_configuration_has_zero_statsd_port()
        {
            Assert.That(_emptyConfig.StatsdPort, Is.EqualTo(0));
        }

        [Test]
        public void empty_configuration_has_a_five_second_interval_so_that_using_it_does_not_constantly_publish_nowhere()
        {
            Assert.That(_emptyConfig.Interval, Is.EqualTo(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public void empty_configuration_has_empty_prefix()
        {
            Assert.That(_emptyConfig.MetricPrefix, Is.EqualTo(string.Empty));
        }

        [Test]
        public void empty_configuration_has_no_metrics()
        {
            Assert.That(_emptyConfig.Metrics, Is.Empty);
        }
    }
}