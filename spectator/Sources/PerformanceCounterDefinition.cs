namespace spectator.Sources
{
    internal class PerformanceCounterDefinition
    {
        private const char KeySeparator = '\\';

        public PerformanceCounterDefinition(string key)
        {
            var keyTokens = key.Split(KeySeparator);

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