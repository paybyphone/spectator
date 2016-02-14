namespace spectator.Sources
{
    public interface IPerformanceCounterRegistry
    {
        float? Read(string categoryName, string counterName, string instance);
    }
}