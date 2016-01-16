using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace spectator.Sources
{
    public class PerformanceCounterRegistry : IDisposable
    {
        public static readonly PerformanceCounterRegistry Instance = new PerformanceCounterRegistry();

        static PerformanceCounterRegistry()
        {
        }

        private readonly ConcurrentDictionary<Tuple<string, string, string>, PerformanceCounter> _registry;

        private PerformanceCounterRegistry()
        {
            _registry = new ConcurrentDictionary<Tuple<string, string, string>, PerformanceCounter>();
        }

        public float? Read(string categoryName, string counterName, string instance)
        {
            var lookupKey = new Tuple<string, string, string>(categoryName, counterName, instance);

            var exists = _registry.ContainsKey(lookupKey);

            if (exists)
            {
                return _registry[lookupKey].NextValue();
            }

            var performanceCounter = instance != null
                    ? new PerformanceCounter(categoryName, counterName, instance)
                    : new PerformanceCounter(categoryName, counterName);

            _registry.TryAdd(lookupKey, performanceCounter);

            performanceCounter.NextValue();

            return null;
        }

        public void Dispose()
        {
            foreach (var counter in _registry)
            {
                counter.Value.Dispose();
            }
        }
    }
}
