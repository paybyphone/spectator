using System;

namespace spectator.Sources.PerformanceCounters
{
    public interface IPerformanceCounter : IDisposable
    {
        float NextValue();
    }
}