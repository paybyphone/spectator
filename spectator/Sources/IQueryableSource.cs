using System.Collections.Generic;

namespace spectator.Sources
{
    public interface IQueryableSource
    {
        IEnumerable<Sample> QueryValue(string path);
    }
}