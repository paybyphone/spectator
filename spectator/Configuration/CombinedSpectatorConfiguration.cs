using System;
using System.Collections.Generic;
using System.Linq;

namespace spectator.Configuration
{
    public class CombinedSpectatorConfiguration : ISpectatorConfiguration
    {
        private readonly ISpectatorConfiguration _baseConfiguration;
        private readonly ISpectatorConfiguration _overrideConfiguration;
        private readonly IList<MetricConfiguration> _combinedMetricConfigurations;

        public string StatsdHost { get { return _overrideConfiguration.StatsdHost ?? _baseConfiguration.StatsdHost; } }

        public int StatsdPort { get { return _overrideConfiguration.StatsdPort; } }

        public string MetricPrefix { get { return _overrideConfiguration.MetricPrefix ?? _baseConfiguration.MetricPrefix; } }

        public TimeSpan Interval { get { return _overrideConfiguration.Interval; } }

        public IList<MetricConfiguration> Metrics { get { return _combinedMetricConfigurations; } }

        private IList<MetricConfiguration> CombineMetricConfigurations(IList<MetricConfiguration> baseMetrics, IList<MetricConfiguration> overrideMetrics)
        {
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

        public CombinedSpectatorConfiguration(ISpectatorConfiguration baseConfiguration, ISpectatorConfiguration overrideConfiguration)
        {
            _baseConfiguration = baseConfiguration;
            _overrideConfiguration = overrideConfiguration;

            _combinedMetricConfigurations = CombineMetricConfigurations(_baseConfiguration.Metrics, _overrideConfiguration.Metrics);
        }
    }
}