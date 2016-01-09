namespace spectator.Metrics
{
    public interface IMetricFormatter
    {
        string Format(string metricPrefix, string template);
    }
}