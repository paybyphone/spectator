using System.Collections.Generic;

namespace spectator.Sources.PerformanceCounters
{
    public interface IPerformanceCounterCategoryRepository
    {
        IEnumerable<string> GetInstances(string category);
    }
}