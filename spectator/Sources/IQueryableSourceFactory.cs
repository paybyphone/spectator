using spectator.Configuration;

namespace spectator.Sources
{
    public interface IQueryableSourceFactory
    {
        IQueryableSource Create(MetricSource source);
    }
}