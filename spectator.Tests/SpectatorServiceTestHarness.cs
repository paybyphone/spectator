using spectator.Configuration;
using spectator.Metrics;
using spectator.Sources;

namespace spectator.Tests
{
    internal class SpectatorServiceTestHarness : SpectatorService
    {
        public SpectatorServiceTestHarness(ISpectatorConfiguration configuration, IQueryableSourceFactory queryableSourceFactory,
            IMetricPublisher publisher, IMetricFormatter metricFormatter) : base(configuration, queryableSourceFactory, publisher, metricFormatter)
        {
        }

        public new void Spectate()
        {
            base.Spectate();
        }
    }
}
