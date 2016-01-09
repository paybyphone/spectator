using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using spectator.Configuration;
using spectator.Metrics;
using spectator.Sources;

namespace spectator
{
    public class SpectatorService
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _eventLoopTask;

        private readonly ISpectatorConfiguration _configuration;
        private readonly IQueryableSourceFactory _queryableSourceFactory;
        private readonly IStatsdPublisher _publisher;
        private readonly IMetricFormatter _metricFormatter;

        public SpectatorService(ISpectatorConfiguration configuration, IQueryableSourceFactory queryableSourceFactory, IStatsdPublisher publisher, IMetricFormatter metricFormatter)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _eventLoopTask = new Task(Spectate, _cancellationTokenSource.Token);

            _configuration = configuration;
            _queryableSourceFactory = queryableSourceFactory;
            _publisher = publisher;
            _metricFormatter = metricFormatter;
        }

        private void Spectate()
        {
            var metricPrefix = _configuration.MetricPrefix;
            var performanceCounterSource = new PerformanceCounterSource();

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    foreach (var metric in _configuration.Metrics)
                    {
                        var queryableSource = _queryableSourceFactory.Create(metric.Source);

                        var metricValue = queryableSource.QueryValue(metric.Path);
                        var metricName = _metricFormatter.Format(metricPrefix, metric.Template);

                        _publisher.Publish(metricName, metricValue, metric.Type);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error occurred in main spectator loop.", ex);
                }

                Thread.Sleep(_configuration.Interval);
            }
        }

        public void Start()
        {
            _eventLoopTask.Start();
            Log.Info("Spectator service started.");
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _eventLoopTask.Wait();

            Log.Info("Spectator service stopped.");
        }
    }
}