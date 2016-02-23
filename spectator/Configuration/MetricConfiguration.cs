namespace spectator.Configuration
{
    public class MetricConfiguration
    {
        public MetricSource Source { get; set; }

        public string Path { get; set; }

        public MetricType Type { get; set; }

        public string Template { get; set; }

        public string Include { get; set; }

        public string Exclude { get; set; }
    }
}