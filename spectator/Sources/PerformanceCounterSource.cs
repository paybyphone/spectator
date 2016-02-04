using System.Collections.Generic;

namespace spectator.Sources
{
    public class PerformanceCounterSource : IQueryableSource
    {
        private readonly IPerformanceCounterRegistry _registry;
        private readonly IPerformanceCounterCategoryRepository _categoryRepository;

        public PerformanceCounterSource(IPerformanceCounterRegistry registry, IPerformanceCounterCategoryRepository categoryRepository)
        {
            _registry = registry;
            _categoryRepository = categoryRepository;
        }

        public IEnumerable<Sample> QueryValue(string path)
        {
            var definition = new PerformanceCounterDefinition(path);

            if (definition.AllInstances)
            {
                var instances = _categoryRepository.GetInstances(definition.CategoryName);

                foreach (var instance in instances)
                {
                    var value = _registry.Read(definition.CategoryName, definition.CounterName, instance);

                    if (value.HasValue)
                    {
                        yield return new Sample(instance, value.Value);
                    }
                }
            }
            else
            {
                var value = _registry.Read(definition.CategoryName, definition.CounterName, definition.InstanceName);

                if (value.HasValue)
                {
                    yield return new Sample(definition.InstanceName, value.Value);
                }
            }
        }
    }
}