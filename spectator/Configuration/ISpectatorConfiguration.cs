using System.Collections.Generic;

namespace spectator.Configuration
{
    public interface ISpectatorConfiguration
    {
        string StatsdHost { get; }

        string MetricPrefix { get; }

        IList<MetricConfiguration> Metrics { get; }
    }
}