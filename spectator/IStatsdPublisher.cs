using spectator.Configuration;

namespace spectator
{
    public interface IStatsdPublisher
    {
        void Publish(string metricName, int metricValue, MetricType type);
    }
}