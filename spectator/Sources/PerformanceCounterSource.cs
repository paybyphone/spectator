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
                    var value = PerformanceCounterRegistry.Instance.Read(definition.CategoryName, definition.CounterName, instance);

                    if (value.HasValue)
                    {
                        yield return new Sample(instance, value.Value);
                    }
                }
            }
            else
            {
                var value = PerformanceCounterRegistry.Instance.Read(definition.CategoryName, definition.CounterName, definition.InstanceName);

                if (value.HasValue)
                {
                    yield return new Sample(definition.InstanceName, value.Value);
                }
            }
        }
    }
}