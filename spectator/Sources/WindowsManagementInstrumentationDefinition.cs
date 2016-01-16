using System;
using System.Linq;

namespace spectator.Sources
{
    internal class WindowsManagementInstrumentationDefinition
    {
        private const char KeySeparator = '\\';

        public WindowsManagementInstrumentationDefinition(string key)
        {
            var keyTokens = key.Split(KeySeparator);

            if (keyTokens.Length < 3)
            {
                throw new ArgumentException("'key' must contain a QuerySource and PropertyName", key);
            }

            QuerySource = keyTokens[1];

            CanOnlyContainAlphanumericOrUnderscore(QuerySource);

            PropertyName = keyTokens[2];
        }

        private void CanOnlyContainAlphanumericOrUnderscore(string querySource)
        {
            if (!querySource.All(l => char.IsLetterOrDigit(l) || l == '_'))
            {
                throw new ArgumentException("The query source specified for a WMI query can only contain letters, digits or underscore ('_')", querySource);
            }
        }

        public string QuerySource { get; private set; }

        public string PropertyName { get; private set; }
    }
}