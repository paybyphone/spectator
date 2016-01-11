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
                    yield return new Sample(instance, PerformanceCounterRegistry.Instance.Read(definition.CategoryName, definition.CounterName, instance));
                }
            }
            else
            {
                yield return new Sample(definition.InstanceName, PerformanceCounterRegistry.Instance.Read(definition.CategoryName, definition.CounterName, definition.InstanceName));
            }
        }
    }
}