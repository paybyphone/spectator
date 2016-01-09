namespace spectator.Sources
{
    public interface IQueryableSource
    {
        int QueryValue(string path);
    }
}