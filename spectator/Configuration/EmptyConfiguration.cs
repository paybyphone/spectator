using System;
using System.Collections.Generic;

namespace spectator.Configuration
{
    public class EmptyConfiguration : ISpectatorConfiguration
    {
        public string StatsdHost { get { return string.Empty; } }

        public int StatsdPort { get { return 0; } }

        public string MetricPrefix { get { return string.Empty; } }

        public TimeSpan Interval { get { return TimeSpan.FromSeconds(5); } }

        public IList<MetricConfiguration> Metrics { get { return new List<MetricConfiguration>(); } }
    }
}