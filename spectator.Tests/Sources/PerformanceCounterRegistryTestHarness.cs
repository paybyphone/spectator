using System;
using System.Collections.Generic;
using spectator.Sources;
using spectator.Sources.PerformanceCounters;

namespace spectator.Tests.Sources
{
    public class PerformanceCounterRegistryTestHarness : PerformanceCounterRegistry
    {
        public PerformanceCounterRegistryTestHarness(IDictionary<Tuple<string, string, string>, IPerformanceCounter> registry, Func<string, string, string, IPerformanceCounter> performanceCounterFactory)
            : base(registry, performanceCounterFactory)
        {
        }
    }
}