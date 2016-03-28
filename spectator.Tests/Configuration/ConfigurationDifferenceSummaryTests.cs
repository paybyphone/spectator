using System;
using NUnit.Framework;
using spectator.Configuration;

namespace spectator.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationDifferenceSummaryTests
    {
        [Test]
        public void when_comparing_two_empty_configurations_then_there_is_no_difference()
        {
            var first = new EmptyConfiguration();
            var second = new EmptyConfiguration();

            var summary = new ConfigurationDifferenceSummary(first, second);

            Assert.That(summary.IsEmpty);
        }

        [Test]
        public void when_statsd_host_is_different_then_it_is_included_in_the_difference_summary()
        {
            var first = new TestConfiguration
                                {
                                    StatsdHost = "first.host"
                                };

            var second = new TestConfiguration
                                {
                                    StatsdHost = "second.host"
                                };

            var summary = new ConfigurationDifferenceSummary(first, second);

            Assert.That(summary.ToString(), Is.EqualTo("StatsdHost: 'first.host' → 'second.host'\r\n"));
        }

        [Test]
        public void when_statsd_port_is_different_then_it_is_included_in_the_difference_summary()
        {
            var first = new TestConfiguration
                                {
                                    StatsdPort = 0
                                };

            var second = new TestConfiguration
                                {
                                    StatsdPort = 5
                                };

            var summary = new ConfigurationDifferenceSummary(first, second);

            Assert.That(summary.ToString(), Is.EqualTo("StatsdPort: '0' → '5'\r\n"));
        }

        [Test]
        public void when_metric_prefix_is_different_then_it_is_included_in_the_difference_summary()
        {
            var first = new TestConfiguration
                                {
                                    MetricPrefix = "first.metric.prefix"
                                };

            var second = new TestConfiguration
                                {
                                    MetricPrefix = "second.metric.prefix"
                                };

            var summary = new ConfigurationDifferenceSummary(first, second);

            Assert.That(summary.ToString(), Is.EqualTo("MetricPrefix: 'first.metric.prefix' → 'second.metric.prefix'\r\n"));
        }

        [Test]
        public void when_interval_is_different_then_it_is_included_in_the_difference_summary()
        {
            var first = new TestConfiguration
                                {
                                    Interval = TimeSpan.Zero
                                };

            var second = new TestConfiguration
                                {
                                    Interval = TimeSpan.FromMinutes(5)
                                };

            var summary = new ConfigurationDifferenceSummary(first, second);

            Assert.That(summary.ToString(), Is.EqualTo("Interval: '00:00:00' → '00:05:00'\r\n"));
        }

        [Test]
        public void when_a_metric_is_added_then_it_is_included_in_the_difference_summary()
        {
            var first = new TestConfiguration();
            var second = new TestConfiguration();
            second.Metrics.Add(new MetricConfiguration { Path = "\\New Metric", Template = "new.metric" });

            var summary = new ConfigurationDifferenceSummary(first, second);

            Assert.That(summary.ToString(), Is.EqualTo("Metrics:\r\n    Added '\\New Metric' → 'new.metric'\r\n"));
        }

        [Test]
        public void when_an_old_metric_is_removed_then_it_is_included_in_the_difference_summary()
        {
            var first = new TestConfiguration();
            first.Metrics.Add(new MetricConfiguration { Path = "\\Old Metric", Template = "old.metric" });
            var second = new TestConfiguration();

            var summary = new ConfigurationDifferenceSummary(first, second);

            Assert.That(summary.ToString(), Is.EqualTo("Metrics:\r\n    Removed '\\Old Metric' → 'old.metric'\r\n"));
        }

        [Test]
        public void when_an_metric_is_replaced_then_it_is_included_in_the_difference_summary()
        {
            var first = new TestConfiguration();
            first.Metrics.Add(new MetricConfiguration { Path = "\\Old Metric", Template = "metric" });
            var second = new TestConfiguration();
            second.Metrics.Add(new MetricConfiguration { Path = "\\New Metric", Template = "metric" });

            var summary = new ConfigurationDifferenceSummary(first, second);

            Assert.That(summary.ToString(), Is.EqualTo("Metrics:\r\n    Replaced '\\Old Metric' → 'metric' with '\\New Metric' → 'metric'\r\n"));
        }

        [Test]
        public void when_given_two_configurations_with_differences_then_a_summary_is_created()
        {
            var first = JsonSpectatorConfiguration.LoadFromString(@"{
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

            var second = JsonSpectatorConfiguration.LoadFromString(@"{
                      ""statsdHost"": ""overridehost"",
                      ""statsdPort"": 8126,
                      ""metricPrefix"": ""override.prefix"",
                      ""interval"":  ""00:00:02"",
                      ""metrics"": [
                        {
                          ""source"": ""performanceCounter"",
                          ""path"": ""\\Processor(_Total)\\% Processor Time"",
                          ""type"": ""gauge"",
                          ""template"": ""base.performance.counter""
                        },
                        {
                          ""source"": ""performanceCounter"",
                          ""path"": ""\\Processor(_Total)\\Some Other Thing"",
                          ""type"": ""gauge"",
                          ""template"": ""overridden.performance.counter""
                        }
                      ]
                    }");

            var summary = new ConfigurationDifferenceSummary(first, second);

            Assert.That(summary.ToString(), Is.EqualTo("StatsdHost: 'basehost' → 'overridehost'\r\nStatsdPort: '8125' → '8126'\r\nMetricPrefix: 'base.prefix' → 'override.prefix'\r\nInterval: '00:00:01' → '00:00:02'\r\nMetrics:\r\n    Replaced '\\Processor(_Total)\\Interrupts/sec' → 'base.performance.counter' with '\\Processor(_Total)\\% Processor Time' → 'base.performance.counter'\r\n    Added '\\Processor(_Total)\\Some Other Thing' → 'overridden.performance.counter'\r\n"));
        }
    }
}