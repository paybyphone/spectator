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
            _eventLoopTask = new Task(BeginSpectating, _cancellationTokenSource.Token);

            _configuration = configuration;
            _queryableSourceFactory = queryableSourceFactory;
            _publisher = publisher;
            _metricFormatter = metricFormatter;
        }

        private void BeginSpectating()
        {
            Log.InfoFormat("Spectating {0} defined metrics", _configuration.Metrics.Count);

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                Spectate();

                Log.DebugFormat("Spectate loop completed, waiting for '{0}' until next loop", _configuration.Interval);
                _cancellationTokenSource.Token.WaitHandle.WaitOne(_configuration.Interval);
            }
        }

        protected void Spectate()
        {
            var metricPrefix = _configuration.MetricPrefix;

            try
            {
                Parallel.ForEach(_configuration.Metrics, metric =>
                {
                    var queryableSource = _queryableSourceFactory.Create(metric.Source);

                    var metricValues = queryableSource.QueryValue(metric.Path);

                    foreach (var sample in metricValues)
                    {
                        if (Included(metric.Include, sample.Instance) && !Excluded(metric.Exclude, sample.Instance))
                        {
                            var metricName = _metricFormatter.Format(metricPrefix, sample.Instance, metric.Template);
                            _publisher.Publish(metricName, sample.Value, metric.Type);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Error("Exception occurred while spectating", ex);
            }
        }

        private bool Included(string includePattern, string instance)
        {
            return string.IsNullOrEmpty(includePattern) || Regex.IsMatch(instance, includePattern);
        }

        private bool Excluded(string excludePattern, string instance)
        {
            return !string.IsNullOrEmpty(excludePattern) && Regex.IsMatch(instance, excludePattern);
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

            PerformanceCounterRegistry.Instance.Dispose();

            Log.Info("Spectator service stopped.");
        }
    }
}