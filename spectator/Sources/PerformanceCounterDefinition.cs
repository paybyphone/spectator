using System;
using spectator.Infrastructure;

namespace spectator.Sources
{
    public sealed class PerformanceCounterDefinition
    {
        private const char KeySeparator = '\\';

        public PerformanceCounterDefinition(string key)
        {
            Must.NotBeNull(() => key);

            if (!key.StartsWith("\\"))
            {
                throw new ArgumentException("'key' must begin with a '\\'");
            }

            var keyTokens = key.Split(KeySeparator);

            if (keyTokens.Length < 3)
            {
                throw new ArgumentException("'key' must contain a Category and Counter", key);
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

        public string CategoryName { get; private set; }

        public string CounterName { get; private set; }

        public string InstanceName { get; private set; }

        public bool AllInstances
        {
            get { return InstanceName == "*"; }
        }
    }
}