using System;
using NUnit.Framework;
using spectator.Configuration;

namespace spectator.Tests.Configuration
{
    [TestFixture]
    public class JsonSpectatorConfigurationTests
    {
        [Test]
        public void can_load_json_formatted_configuration()
        {
            var config = JsonSpectatorConfiguration.LoadConfigFromString(
                  @"{
                      ""statsdHost"": ""localhost"",
                      ""statsdPort"": 8125,
                      ""metricPrefix"": ""net.verrus.{machine}"",
                      ""interval"":  ""00:00:05"",
                      ""metrics"": [
                        {
                          ""source"": ""performanceCounter"",
                          ""path"": ""\\Processor(_Total)\\Interrupts/sec"",
                          ""type"": ""gauge"",
                          ""template"": ""system.processor.interrupts_per_sec""
                        }
                      ]
                    }");

            Assert.That(config.StatsdHost, Is.EqualTo("localhost"));
            Assert.That(config.StatsdPort, Is.EqualTo(8125));
            Assert.That(config.MetricPrefix, Is.EqualTo("net.verrus.{machine}"));
            Assert.That(config.Interval, Is.EqualTo(TimeSpan.FromSeconds(5)));

            Assert.That(config.Metrics.Count, Is.EqualTo(1));
            Assert.That(config.Metrics[0].Source, Is.EqualTo(MetricSource.PerformanceCounter));
            Assert.That(config.Metrics[0].Path, Is.EqualTo("\\Processor(_Total)\\Interrupts/sec"));
            Assert.That(config.Metrics[0].Type, Is.EqualTo(MetricType.Gauge));
            Assert.That(config.Metrics[0].Template, Is.EqualTo("system.processor.interrupts_per_sec"));
        }
    }
}