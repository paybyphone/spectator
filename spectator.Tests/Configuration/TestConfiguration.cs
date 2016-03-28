using System;
using System.Collections.Generic;
using spectator.Configuration;

namespace spectator.Tests.Configuration
{
    public class TestConfiguration : ISpectatorConfiguration
    {
        public TestConfiguration()
        {
            StatsdHost = string.Empty;
            MetricPrefix = string.Empty;
            Metrics = new List<MetricConfiguration>();
        }

        public string StatsdHost { get; set; }

        public int StatsdPort { get; set; }

        public string MetricPrefix { get; set; }

        public TimeSpan Interval { get; set; }

        public IList<MetricConfiguration> Metrics { get; set; }
    }
}