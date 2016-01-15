using System.Net;
using NUnit.Framework;
using spectator.Metrics;

namespace spectator.Tests.Metrics
{
    [TestFixture]
    public class MetricFormatterTests
    {
        private MetricFormatter _metricFormatter;

        [SetUp]
        public void SetUp()
        {
            _metricFormatter = new MetricFormatter();
        }

        [Test]
        public void should_replace_machine_template_with_hostname()
        {
            var formatted = _metricFormatter.Format(Some.String(), Some.String(), @"{machine}");
            var hostname = Dns.GetHostName().ToLowerInvariant();

            Assert.That(formatted, Is.EqualTo(hostname));
        }

        [Test]
        public void should_replace_instance_template_with_provided_instance()
        {
            var instance = "iamaninstance";
            var formatted = _metricFormatter.Format(Some.String(), instance, @"{instance}");

            Assert.That(formatted, Is.EqualTo(instance));
        }

        [Test]
        public void metric_prefix_is_optional()
        {
            var someMetricName = @"some.metric.name";
            Assert.That(_metricFormatter.Format(string.Empty, Some.String(), someMetricName), Is.EqualTo(someMetricName));
            Assert.That(_metricFormatter.Format(null, Some.String(), someMetricName), Is.EqualTo(someMetricName));
        }

        [TestCase("this-is.some_metric.$@name")]
        [TestCase("this-is.some_metric.{name}")]
        [TestCase("this-is .som  e_me  t  ric.    {name}")]
        [TestCase("this-is.some_metric.!!name*&^%$#@")]
        public void unsupported_characters_will_be_replaced_by_an_empty_string(string metricNameContainingInvalidCharacters)
        {
            var formatted = _metricFormatter.Format(string.Empty, Some.String(), metricNameContainingInvalidCharacters);
            Assert.That(formatted, Is.EqualTo(@"this-is.some_metric.name"));
        }
    }
}