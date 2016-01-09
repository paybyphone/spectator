using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using spectator.Sources;

namespace spectator
{
    public class SpectatorService
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _eventLoopTask;

        public SpectatorService()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _eventLoopTask = new Task(DoWork, _cancellationTokenSource.Token);
        }

        private void DoWork()
        {
            var performanceCounterSource = new PerformanceCounterSource();

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var value = performanceCounterSource.QueryValue("Processor\\% Processor Time\\_Total");
                    Log.Info(value);
                }
                catch (Exception ex)
                {
                    Log.Error("Error occurred in main spectator loop.", ex);
                }

                Thread.Sleep(2000);
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