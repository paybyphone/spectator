using System;
using System.Collections.Generic;

namespace spectator.Configuration
{
    public class EmptyConfiguration : ISpectatorConfiguration
    {
        public string StatsdHost => string.Empty;

        public int StatsdPort => 0;

        public string MetricPrefix => string.Empty;

        public TimeSpan Interval => TimeSpan.FromSeconds(5);

        public IList<MetricConfiguration> Metrics => new List<MetricConfiguration>();
    }
}