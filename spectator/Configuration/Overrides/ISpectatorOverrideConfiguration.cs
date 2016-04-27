using System;
using System.Collections.Generic;

namespace spectator.Configuration.Overrides
{
    public interface ISpectatorOverrideConfiguration
    {
        string StatsdHost { get; }

        int? StatsdPort { get; }

        string MetricPrefix { get; }

        TimeSpan? Interval { get; }

        IList<MetricConfiguration> Metrics { get; }
    }
}