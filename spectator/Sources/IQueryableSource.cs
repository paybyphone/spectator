namespace spectator.Sources
{
    public interface IQueryableSource
    {
        double QueryValue(string path);
    }
}