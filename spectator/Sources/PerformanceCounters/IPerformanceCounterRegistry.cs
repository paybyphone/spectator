namespace spectator.Sources.PerformanceCounters
{
    public interface IPerformanceCounterRegistry
    {
        float? Read(string categoryName, string counterName, string instance);
    }
}