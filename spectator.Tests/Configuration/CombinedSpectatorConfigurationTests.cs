using System.Linq;
using NUnit.Framework;
using spectator.Configuration;

namespace spectator.Tests.Configuration
{
    [TestFixture]
    public class CombinedSpectatorConfigurationTests
    {
        private ISpectatorConfiguration _overrideConfig;
        private ISpectatorConfiguration _baseConfig;
        private CombinedSpectatorConfiguration _combinedConfig;

        [SetUp]
        public void SetUp()
        {
            _baseConfig = JsonSpectatorConfiguration.LoadFromString(@"{
                      ""statsdHost"": ""basehost"",
                      ""statsdPort"": 8125,
                      ""metricPrefix"": ""base.prefix"",
                      ""interval"":  ""00:00:01"",
                      ""metrics"": [
                        {
                          ""source"": ""performanceCounter"",
                          ""path"": ""\\Processor(_Total)\\Interrupts/sec"",
                          ""type"": ""gauge"",
                          ""template"": ""base.performance.counter""
                        },
                        {
                          ""source"": ""performanceCounter"",
                          ""path"": ""\\Processor(_Total)\\Interrupts/sec"",
                          ""type"": ""gauge"",
                          ""template"": ""overriden.performance.counter""
                        }
                      ]
                    }");

            _overrideConfig = JsonSpectatorConfiguration.LoadFromString(@"{
                      ""statsdHost"": ""overridehost"",
                      ""statsdPort"": 8126,
                      ""metricPrefix"": ""override.prefix"",
                      ""interval"":  ""00:00:02"",
                      ""metrics"": [
                        {
                          ""source"": ""performanceCounter"",
                          ""path"": ""\\Processor(_Total)\\Interrupts/sec"",
                          ""type"": ""gauge"",
                          ""template"": ""override.performance.counter""
                        },
                        {
                          ""source"": ""performanceCounter"",
                          ""path"": ""\\Processor(_Total)\\% Processor Time"",
                          ""type"": ""gauge"",
                          ""template"": ""overriden.performance.counter""
                        }
                      ]
                    }");

            _combinedConfig = new CombinedSpectatorConfiguration(_baseConfig, _overrideConfig);
        }

        [Test]
        public void the_override_statsd_host_should_be_used()
        {
            Assert.That(_combinedConfig.StatsdHost, Is.EqualTo(_overrideConfig.StatsdHost));
        }

        [Test]
        public void the_override_statsd_port_should_be_used()
        {
            Assert.That(_combinedConfig.StatsdPort, Is.EqualTo(_overrideConfig.StatsdPort));
        }

        [Test]
        public void the_override_metric_prefix_should_be_used()
        {
            Assert.That(_combinedConfig.MetricPrefix, Is.EqualTo(_overrideConfig.MetricPrefix));
        }

        [Test]
        public void the_override_interval_should_be_used()
        {
            Assert.That(_combinedConfig.Interval, Is.EqualTo(_overrideConfig.Interval));
        }

        [Test]
        public void the_metrics_from_the_base_and_override_should_be_combined_together()
        {
            Assert.That(_combinedConfig.Metrics.Count, Is.EqualTo(3));
        }

        [Test]
        public void any_base_metrics_should_be_in_the_combined_config()
        {
            Assert.That(_combinedConfig.Metrics.Any(m => m.Template.Equals("base.performance.counter")));
        }

        [Test]
        public void any_additional_override_metrics_should_be_in_the_combined_config()
        {
            Assert.That(_combinedConfig.Metrics.Any(m => m.Template.Equals("override.performance.counter")));
        }

        [Test]
        public void any_overriden_metrics_should_be_in_the_combined_config()
        {
            Assert.That(_combinedConfig.Metrics.Any(m => m.Template.Equals("overriden.performance.counter")));
        }

        [Test]
        public void the_overriden_metric_should_match_the_definition_from_the_override_config()
        {
            var overridenMetricInBase = _baseConfig.Metrics.SingleOrDefault(m => m.Template.Equals("overriden.performance.counter"));
            var overridenMetricInOverride = _overrideConfig.Metrics.SingleOrDefault(m => m.Template.Equals("overriden.performance.counter"));
            var overridenMetricInCombined = _combinedConfig.Metrics.SingleOrDefault(m => m.Template.Equals("overriden.performance.counter"));

            Assert.That(overridenMetricInCombined, Is.Not.Null);
            Assert.That(overridenMetricInCombined.Path, Is.Not.EqualTo(overridenMetricInBase.Path));
            Assert.That(overridenMetricInCombined.Path, Is.EqualTo(overridenMetricInOverride.Path));
        }
    }
}