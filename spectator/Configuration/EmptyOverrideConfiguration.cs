using System;
using System.Collections.Generic;
using spectator.Configuration.Overrides;

namespace spectator.Configuration
{
    public class EmptyOverrideConfiguration : ISpectatorOverrideConfiguration
    {
        public string StatsdHost => null;

        public int? StatsdPort => null;

        public string MetricPrefix => null;

        public TimeSpan? Interval => null;

        public IList<MetricConfiguration> Metrics => new List<MetricConfiguration>();
    }
}