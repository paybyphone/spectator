using System;
using log4net;
using spectator.Configuration;
using StatsdClient;

namespace spectator.Metrics
{
    public class StatsdPublisher : IMetricPublisher
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IStatsd _statsdClient;

        public StatsdPublisher(IStatsd statsdClient)
        {
            _statsdClient = statsdClient;
        }

        public void Publish(Metric metric)
        {
            Log.DebugFormat("{0} | {1} | {2}", metric.Name, metric.Value, metric.Type);

            switch (metric.Type)
            {
                case MetricType.Gauge:
                    _statsdClient.Send<Statsd.Gauge>(metric.Name, metric.Value);
                    return;
                case MetricType.Count:
                    break;
                case MetricType.Timing:
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}