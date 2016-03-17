using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace spectator.Sources.PerformanceCounters
{
    public class PerformanceCounterCategoryRepository : IPerformanceCounterCategoryRepository
    {
        private readonly IIllegalInstanceNameMapper _illegalInstanceNameMapper;

        public PerformanceCounterCategoryRepository(IIllegalInstanceNameMapper illegalInstanceNameMapper)
        {
            _illegalInstanceNameMapper = illegalInstanceNameMapper;
        }

        public IEnumerable<string> GetInstances(string category)
        {
            var counterCategory = new PerformanceCounterCategory(category);
            var instances = counterCategory.GetInstanceNames()
                                           .Select(n => _illegalInstanceNameMapper.Map(n));

            return instances;
        }
    }
}