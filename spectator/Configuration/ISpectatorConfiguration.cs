using System;
using System.Collections.Generic;

namespace spectator.Configuration
{
    public interface ISpectatorConfiguration
    {
        string StatsdHost { get; }

        int StatsdPort { get; }

        string MetricPrefix { get; }

        TimeSpan Interval { get; }

        IList<MetricConfiguration> Metrics { get; }
    }
}