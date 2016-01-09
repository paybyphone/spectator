using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace spectator.Sources
{
    public class PerformanceCounterSource : IQueryableSource
    {
        public IEnumerable<Sample> QueryValue(string path)
        {
            var definition = new PerformanceCounterDefinition(path);

            if (definition.AllInstances)
            {
                var counterCategory = new PerformanceCounterCategory(definition.CategoryName);
                var instances = counterCategory.GetInstanceNames();

                foreach (var instance in instances)
                {
                    var counter = new PerformanceCounter(definition.CategoryName, definition.CounterName, instance);

                    float rawValue = counter.NextValue();

                    yield return new Sample(instance, rawValue);
                }
            }
            else
            {
                var counter = definition.InstanceName != null
                    ? new PerformanceCounter(definition.CategoryName, definition.CounterName, definition.InstanceName)
                    : new PerformanceCounter(definition.CategoryName, definition.CounterName);

                float rawValue = counter.NextValue();

                yield return new Sample(definition.InstanceName, rawValue);
            }
        }
    }
}