using System;
using System.Collections.Generic;

namespace spectator.Configuration
{
    public class EmptyConfiguration : ISpectatorConfiguration
    {
        public string StatsdHost { get; }

        public int StatsdPort { get; }

        public string MetricPrefix { get; }

        public TimeSpan Interval { get; }

        public IList<MetricConfiguration> Metrics { get; }
    }
}