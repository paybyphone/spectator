using System.Linq;
using NUnit.Framework;
using spectator.Configuration;
using spectator.Configuration.Overrides;

namespace spectator.Tests.Configuration.CombinedSpectatorConfigurationTests
{
    [TestFixture]
    public class WithEmptyOverridesTests
    {
        private ISpectatorOverrideConfiguration _overrideConfig;
        private ISpectatorConfiguration _baseConfig;
        private CombinedSpectatorConfiguration _combinedConfig;

        [SetUp]
        public void SetUp()
        {
            _baseConfig = JsonSpectatorConfiguration.LoadConfigFromString(@"{
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
                        }
                      ]
                    }");

            _overrideConfig = JsonSpectatorConfiguration.LoadConfigFromString(@"{
                      ""metrics"": [
                      ]
                    }");

            _combinedConfig = new CombinedSpectatorConfiguration(_baseConfig, _overrideConfig);
        }

        [Test]
        public void the_base_statsd_host_should_still_be_used()
        {
            Assert.That(_combinedConfig.StatsdHost, Is.EqualTo(_baseConfig.StatsdHost));
        }

        [Test]
        public void the_base_statsd_port_should_still_be_used()
        {
            Assert.That(_combinedConfig.StatsdPort, Is.EqualTo(_baseConfig.StatsdPort));
        }

        [Test]
        public void the_base_metric_prefix_should_still_be_used()
        {
            Assert.That(_combinedConfig.MetricPrefix, Is.EqualTo(_baseConfig.MetricPrefix));
        }

        [Test]
        public void the_base_interval_should_still_be_used()
        {
            Assert.That(_combinedConfig.Interval, Is.EqualTo(_baseConfig.Interval));
        }

        [Test]
        public void the_metrics_from_the_base_and_override_should_be_combined_together()
        {
            Assert.That(_combinedConfig.Metrics.Count, Is.EqualTo(1));
        }

        [Test]
        public void any_base_metrics_should_be_in_the_combined_config()
        {
            Assert.That(_combinedConfig.Metrics.Any(m => m.Template.Equals("base.performance.counter")));
        }
    }
}