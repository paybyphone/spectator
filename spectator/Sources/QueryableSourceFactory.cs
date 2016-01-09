using System;
using spectator.Configuration;

namespace spectator.Sources
{
    public class QueryableSourceFactory : IQueryableSourceFactory
    {
        public IQueryableSource Create(MetricSource source)
        {
            switch (source)
            {
                case MetricSource.PerformanceCounter:
                    return new PerformanceCounterSource();

                case MetricSource.WindowsManagementInstrumentation:
                    return new WindowsManagementInstrumentationSource();
            }

            throw new NotSupportedException();
        }
    }
}