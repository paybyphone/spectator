using System;
using System.Collections.Generic;

namespace spectator.Configuration
{
    public class JsonConfiguration : ISpectatorConfiguration
    {
        public string StatsdHost { get; set; }

        public int StatsdPort { get; set; }

        public string MetricPrefix { get; set; }

        public TimeSpan Interval { get; set; }

        public IList<MetricConfiguration> Metrics { get; set; }
    }
}