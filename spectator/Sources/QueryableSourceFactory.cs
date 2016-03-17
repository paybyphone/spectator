using System;
using spectator.Configuration;
using spectator.Sources.PerformanceCounters;

namespace spectator.Sources
{
    public class QueryableSourceFactory : IQueryableSourceFactory
    {
        public IQueryableSource Create(MetricSource source)
        {
            switch (source)
            {
                case MetricSource.PerformanceCounter:
                    return new PerformanceCounterSource(PerformanceCounterRegistry.Instance, new PerformanceCounterCategoryRepository());

                case MetricSource.WindowsManagementInstrumentation:
                    return new WindowsManagementInstrumentationSource();
            }

            throw new NotSupportedException("Unsupported metric source requested");
        }
    }
}