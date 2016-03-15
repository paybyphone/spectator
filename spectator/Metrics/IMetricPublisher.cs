namespace spectator.Metrics
{
    public interface IMetricPublisher
    {
        void Publish(Metric metric);
    }
}