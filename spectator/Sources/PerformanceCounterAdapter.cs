using System.Diagnostics;

namespace spectator.Sources
{
    public class PerformanceCounterAdapter : IPerformanceCounter
    {
        private readonly PerformanceCounter _performanceCounter;

        public PerformanceCounterAdapter(string categoryName, string counterName, string instance)
        {
            _performanceCounter = instance != null
                    ? new PerformanceCounter(categoryName, counterName, instance)
                    : new PerformanceCounter(categoryName, counterName);
        }

        public float NextValue()
        {
            return _performanceCounter.NextValue();
        }

        public void Dispose()
        {
            _performanceCounter.Dispose();
        }
    }
}