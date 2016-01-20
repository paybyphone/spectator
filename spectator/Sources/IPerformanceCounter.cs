using System;

namespace spectator.Sources
{
    public interface IPerformanceCounter : IDisposable
    {
        float NextValue();
    }
}