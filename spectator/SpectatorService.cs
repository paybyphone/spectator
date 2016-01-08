using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;

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
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                Console.WriteLine("I am working");

                Console.WriteLine("   Step 1");
                Thread.Sleep(1000);

                Console.WriteLine("   Step 2");
                Thread.Sleep(1000);

                Console.WriteLine("   Step 3");
                Thread.Sleep(1000);

                Console.WriteLine("   Step 4");
                Thread.Sleep(1000);

                Console.WriteLine("   Step 5");
                Thread.Sleep(1000);
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