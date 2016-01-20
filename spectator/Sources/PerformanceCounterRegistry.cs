using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace spectator.Sources
{
    public class PerformanceCounterRegistry : IDisposable
    {
        public static readonly PerformanceCounterRegistry Instance =
            new PerformanceCounterRegistry(new ConcurrentDictionary<Tuple<string, string, string>, IPerformanceCounter>(),
                                           (category, counter, instance) => new PerformanceCounterAdapter(category, counter, instance));

        static PerformanceCounterRegistry()
        {
        }

        private readonly IDictionary<Tuple<string, string, string>, IPerformanceCounter> _registry;
        private readonly Func<string, string, string, IPerformanceCounter> _performanceCounterFactory;

        protected PerformanceCounterRegistry(IDictionary<Tuple<string, string, string>, IPerformanceCounter> registry, Func<string, string, string, IPerformanceCounter> performanceCounterFactory)
        {
            _registry = registry;
            _performanceCounterFactory = performanceCounterFactory;
        }

        public float? Read(string categoryName, string counterName, string instance)
        {
            var lookupKey = new Tuple<string, string, string>(categoryName, counterName, instance);

            var exists = _registry.ContainsKey(lookupKey);

            if (exists)
            {
                return _registry[lookupKey].NextValue();
            }

            var performanceCounter = _performanceCounterFactory(categoryName, counterName, instance);

            _registry.Add(lookupKey, performanceCounter);

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
