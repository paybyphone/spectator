using System;
using System.Collections.Generic;
using System.Linq;
using spectator.Configuration.Overrides;
using spectator.Infrastructure;

namespace spectator.Configuration
{
    public class CombinedSpectatorConfiguration : ISpectatorConfiguration
    {
        private readonly ISpectatorConfiguration _baseConfiguration;
        private readonly ISpectatorOverrideConfiguration _overrideConfiguration;

        public string StatsdHost => _overrideConfiguration.StatsdHost ?? _baseConfiguration.StatsdHost;

        public int StatsdPort => _overrideConfiguration.StatsdPort ?? _baseConfiguration.StatsdPort;

        public string MetricPrefix => _overrideConfiguration.MetricPrefix ?? _baseConfiguration.MetricPrefix;

        public TimeSpan Interval => _overrideConfiguration.Interval ?? _baseConfiguration.Interval;

        public IList<MetricConfiguration> Metrics { get; }

        private static IList<MetricConfiguration> CombineMetricConfigurations(IList<MetricConfiguration> baseMetrics, IList<MetricConfiguration> overrideMetrics)
        {
            if (overrideMetrics.IsNull() || !overrideMetrics.Any())
            {
                return baseMetrics;
            }

            if (baseMetrics.IsNull() || !baseMetrics.Any())
            {
                return overrideMetrics;
            }

            var combinedMetricConfiguration = new List<MetricConfiguration>();

            combinedMetricConfiguration.AddRange(baseMetrics);

            foreach (var metric in overrideMetrics)
            {
                var matchingMetric = combinedMetricConfiguration.SingleOrDefault(m => m.Template.Equals(metric.Template));

                if (matchingMetric != null)
                {
                    combinedMetricConfiguration.Remove(matchingMetric);
                }

                combinedMetricConfiguration.Add(metric);
            }

            return combinedMetricConfiguration;
        }

        public CombinedSpectatorConfiguration(ISpectatorConfiguration baseConfiguration, ISpectatorOverrideConfiguration overrideConfiguration)
        {
            _baseConfiguration = baseConfiguration;
            _overrideConfiguration = overrideConfiguration;

            Metrics = CombineMetricConfigurations(_baseConfiguration.Metrics, _overrideConfiguration.Metrics);
        }
    }
}