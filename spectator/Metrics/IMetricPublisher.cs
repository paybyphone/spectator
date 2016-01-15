using spectator.Configuration;

namespace spectator.Metrics
{
    public interface IMetricPublisher
    {
        void Publish(string metricName, double metricValue, MetricType type = MetricType.Gauge);
    }
}