using spectator.Sources;

namespace spectator.Metrics
{
    public class MetricDefinition
    {
        public MetricDefinition(IQueryableSource source, string key)
        {
            Source = source;
            Key = key;
        }

        public IQueryableSource Source { get; private set; }

        public string Key { get; private set; }
    }
}