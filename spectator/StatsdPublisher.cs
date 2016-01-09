using System;
using log4net;
using StatsdClient;
using MetricType = spectator.Configuration.MetricType;

namespace spectator
{
    public class StatsdPublisher : IMetricPublisher
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IStatsd _statsdClient;

        public StatsdPublisher(IStatsd statsdClient)
        {
            _statsdClient = statsdClient;
        }

        public void Publish(string metricName, double metricValue, MetricType type = MetricType.Gauge)
        {
            Log.InfoFormat("{0} | {1} | {2}", metricName, metricValue, type);

            switch (type)
            {
                case MetricType.Gauge:
                    _statsdClient.Send<Statsd.Gauge>(metricName, (double)metricValue);
                    return;
            }

            throw new NotSupportedException();
        }
    }
}