using System.Diagnostics;

namespace spectator.Sources
{
    public class PerformanceCounterSource : IQueryableSource
    {
        public int QueryValue(string path)
        {
            var definition = new PerformanceCounterDefinition(path);

            var counter = definition.InstanceName != null ? new PerformanceCounter(definition.CategoryName, definition.CounterName, definition.InstanceName)
                : new PerformanceCounter(definition.CategoryName, definition.CounterName);

            float rawValue = counter.NextValue();

            return (int)rawValue;
        }
    }

    internal class PerformanceCounterDefinition
    {
        private const char KeySeparator = '\\';

        public PerformanceCounterDefinition(string key)
        {
            var keyTokens = key.Split(KeySeparator);

            CategoryName = keyTokens[0];
            CounterName = keyTokens[1];

            if (keyTokens.Length > 2)
            {
                InstanceName = keyTokens[2];
            }
        }

        public string CategoryName { get; private set; }

        public string CounterName { get; private set; }

        public string InstanceName { get; private set; }
    }
}