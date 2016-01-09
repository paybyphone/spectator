using log4net;
using spectator.Sources;
using StatsdClient;
using MetricType = spectator.Configuration.MetricType;

namespace spectator
{
    public class StatsdPublisher : IStatsdPublisher
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IStatsd _statsdClient;

        public StatsdPublisher(IStatsd statsdClient)
        {
            _statsdClient = statsdClient;
        }

        public void Publish(string metricName, int metricValue, MetricType type)
        {
            Log.InfoFormat("{0} | {1} | {2}", metricName, metricValue, type);

            switch (type)
            {
                case MetricType.Count:
                    _statsdClient.LogCount(metricName, metricValue);
                    break;
                case MetricType.Gauge:
                    _statsdClient.LogGauge(metricName, metricValue);
                    break;
                case MetricType.Timing:
                    _statsdClient.LogTiming(metricName, metricValue);
                    break;
            }
        }
    }
}