namespace spectator.Configuration
{
    public class MetricConfiguration
    {
        public MetricSource Source { get; set; }

        public string SourceKey { get; set; }

        public MetricType MetricType { get; set; }

        public string MetricTemplate { get; set; }
    }
}