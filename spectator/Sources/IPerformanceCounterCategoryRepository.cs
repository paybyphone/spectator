using System.Collections.Generic;

namespace spectator.Sources
{
    public interface IPerformanceCounterCategoryRepository
    {
        IEnumerable<string> GetInstances(string category);
    }
}