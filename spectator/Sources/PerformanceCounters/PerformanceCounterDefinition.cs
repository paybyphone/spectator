using System;
using spectator.Infrastructure;

namespace spectator.Sources.PerformanceCounters
{
    internal sealed class PerformanceCounterDefinition
    {
        private const char KeySeparator = '\\';

        public PerformanceCounterDefinition(string path)
        {
            Must.NotBeNull(() => path);

            if (!path.StartsWith("\\"))
            {
                throw new ArgumentException("'path' must begin with a '\\'");
            }

            var keyTokens = path.Split(KeySeparator);

            if (keyTokens.Length < 3)
            {
                throw new ArgumentException("'path' must contain a Category and Counter", path);
            }

            CategoryName = keyTokens[1];
            CounterName = keyTokens[2];

            if (CategoryName.Contains("(") && CategoryName.Contains(")"))
            {
                var categoryNameTokens = CategoryName.Split('(', ')');
                CategoryName = categoryNameTokens[0];
                InstanceName = categoryNameTokens[1];
            }
        }

        public string CategoryName { get; }

        public string CounterName { get; private set; }

        public string InstanceName { get; }

        public bool AllInstances => InstanceName == "*";
    }
}