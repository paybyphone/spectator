using spectator.Configuration;

namespace spectator.Metrics
{
    public class Metric
    {
        public Metric(string name, double value, MetricType type = MetricType.Gauge)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        public string Name { get; private set; }

        public double Value { get; private set; }

        public MetricType Type { get; private set; }
    }
}