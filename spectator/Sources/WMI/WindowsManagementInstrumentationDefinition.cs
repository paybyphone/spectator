using System;
using System.Linq;
using spectator.Infrastructure;

namespace spectator.Sources.WMI
{
    internal class WindowsManagementInstrumentationDefinition
    {
        private const char KeySeparator = '\\';

        public WindowsManagementInstrumentationDefinition(string path)
        {
            Must.NotBeNull(() => path);

            var keyTokens = path.Split(KeySeparator);

            if (keyTokens.Length < 3)
            {
                throw new ArgumentException("'path' must contain a QuerySource and PropertyName", path);
            }

            QuerySource = keyTokens[1];

            CanOnlyContainAlphanumericOrUnderscore(QuerySource);

            PropertyName = keyTokens[2];
        }

        private void CanOnlyContainAlphanumericOrUnderscore(string querySource)
        {
            Must.NotBeNull(() => querySource);

            if (!querySource.All(l => char.IsLetterOrDigit(l) || l == '_'))
            {
                throw new ArgumentException("The query source specified for a WMI query can only contain letters, digits or underscore ('_')", querySource);
            }
        }

        public string QuerySource { get; }

        public string PropertyName { get; private set; }
    }
}