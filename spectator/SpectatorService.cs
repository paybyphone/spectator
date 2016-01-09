using System;
using System.Text.RegularExpressions;
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
        private readonly IMetricPublisher _publisher;
        private readonly IMetricFormatter _metricFormatter;

        public SpectatorService(ISpectatorConfiguration configuration, IQueryableSourceFactory queryableSourceFactory, IMetricPublisher publisher, IMetricFormatter metricFormatter)
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

            Log.InfoFormat("Spectating {0} defined metrics", _configuration.Metrics.Count);

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    Parallel.ForEach(_configuration.Metrics, metric =>
                    {
                        var queryableSource = _queryableSourceFactory.Create(metric.Source);

                        var metricValues = queryableSource.QueryValue(metric.Path);

                        foreach (var sample in metricValues)
                        {
                            if (string.IsNullOrEmpty(metric.Exclude) || !Regex.IsMatch(sample.Instance, metric.Exclude))
                            {
                                var metricName = _metricFormatter.Format(metricPrefix, sample.Instance, metric.Template);
                                _publisher.Publish(metricName, sample.Value, metric.Type);
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Log.Error("Error occurred in main spectator loop.", ex);
                }

                Log.DebugFormat("Spectate loop completed, sleeping for '{0}'", _configuration.Interval);
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