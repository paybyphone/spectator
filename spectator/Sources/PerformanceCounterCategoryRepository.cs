using System.Collections.Generic;
using System.Diagnostics;

namespace spectator.Sources
{
    public class PerformanceCounterCategoryRepository : IPerformanceCounterCategoryRepository
    {
        public IEnumerable<string> GetInstances(string category)
        {
            var counterCategory = new PerformanceCounterCategory(category);
            var instances = counterCategory.GetInstanceNames();

            return instances;
        }
    }
}