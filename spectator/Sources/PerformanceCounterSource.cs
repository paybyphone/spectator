using System.Diagnostics;

namespace spectator.Sources
{
    public class PerformanceCounterSource : IQueryableSource
    {
        public double QueryValue(string path)
        {
            var definition = new PerformanceCounterDefinition(path);

            var counter = definition.InstanceName != null ? new PerformanceCounter(definition.CategoryName, definition.CounterName, definition.InstanceName)
                : new PerformanceCounter(definition.CategoryName, definition.CounterName);

            float rawValue = counter.NextValue();

            return rawValue;
        }
    }
}