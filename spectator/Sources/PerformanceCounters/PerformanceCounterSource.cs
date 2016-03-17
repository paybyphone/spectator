using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace spectator.Sources.PerformanceCounters
{
    public class PerformanceCounterSource : IQueryableSource
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IPerformanceCounterRegistry _registry;
        private readonly IPerformanceCounterCategoryRepository _categoryRepository;

        public PerformanceCounterSource(IPerformanceCounterRegistry registry, IPerformanceCounterCategoryRepository categoryRepository)
        {
            _registry = registry;
            _categoryRepository = categoryRepository;
        }

        public IEnumerable<Sample> QueryValue(string path)
        {
            try
            {
                var samples = new List<Sample>();
                var definition = new PerformanceCounterDefinition(path);

                if (definition.AllInstances)
                {
                    var instances = _categoryRepository.GetInstances(definition.CategoryName);

                    foreach (var instance in instances)
                    {
                        var value = _registry.Read(definition.CategoryName, definition.CounterName, instance);

                        if (value.HasValue)
                        {
                            samples.Add(new Sample(instance, value.Value));
                        }
                    }
                }
                else
                {
                    var value = _registry.Read(definition.CategoryName, definition.CounterName, definition.InstanceName);

                    if (value.HasValue)
                    {
                        samples.Add(new Sample(definition.InstanceName, value.Value));
                    }
                }

                return samples;
            }
            catch (Exception ex)
            {
                ex.Data.Add("Path", path);
                Log.Error("Exception occurred querying performance counter", ex);
            }

            return Enumerable.Empty<Sample>();
        }
    }
}