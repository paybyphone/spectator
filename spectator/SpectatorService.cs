using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using spectator.Configuration;
using spectator.Metrics;
using spectator.Sources;
using spectator.Sources.PerformanceCounters;

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
                Spectate(_cancellationTokenSource.Token);

                Log.DebugFormat("Spectate loop completed, waiting for '{0}' until next loop", _configuration.Interval);
                _cancellationTokenSource.Token.WaitHandle.WaitOne(_configuration.Interval);
            }
        }

        protected void Spectate(CancellationToken token, TaskScheduler scheduler = null)
        {
            var metricPrefix = _configuration.MetricPrefix;

            try
            {
                Log.DebugFormat("Sampling for {0} configured metric definitions", _configuration.Metrics.Count);

                var options = new ParallelOptions
                {
                    CancellationToken = token,
                    MaxDegreeOfParallelism = 8,
                    TaskScheduler = scheduler
                };

                var allMetrics = new ConcurrentBag<Metric>();

                Parallel.ForEach(_configuration.Metrics, options, metric =>
                {
                    var queryableSource = _queryableSourceFactory.Create(metric.Source);

                    var metricValues = queryableSource.QueryValue(metric.Path).ToList();

                    Log.DebugFormat("  -> Queried '{0}' from '{1}', resulting in {2} metric samples (unfiltered)", metric.Path, metric.Source, metricValues.Count());

                    foreach (var sample in metricValues)
                    {
                        if (Included(metric.Include, sample.Instance) && !Excluded(metric.Exclude, sample.Instance))
                        {
                            var metricName = _metricFormatter.Format(metricPrefix, sample.Instance, metric.Template);

                            allMetrics.Add(new Metric(metricName, sample.Value, metric.Type));
                        }
                    }
                });

                Log.DebugFormat("Obtained {0} metrics to publish, publishing now...", allMetrics.Count);

                foreach (var metric in allMetrics)
                {
                    _publisher.Publish(metric);
                }
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

            Log.Info("Cleaning up performance counters.");
            PerformanceCounterRegistry.Instance.Dispose();

            Log.Info("Spectator service stopped.");
        }
    }
}