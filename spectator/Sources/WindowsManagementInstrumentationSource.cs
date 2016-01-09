using System.Linq;
using System.Management;

namespace spectator.Sources
{
    public class WindowsManagementInstrumentationSource : IQueryableSource
    {
        public Number QueryValue(string key)
        {
            var definition = new WindowsManagementInstrumentationDefinition(key);

            var searcher = new ManagementObjectSearcher("select * from " + definition.QuerySource);

            foreach (ManagementObject managedObject in searcher.Get())
            {
                return GetInfo(managedObject, definition.PropertyName);
            }

            return null;
        }

        private static ulong GetInfo(ManagementObject managedObject, string property)
        {
            object manageObjectProperty = managedObject[property];

            ulong value = (ulong)manageObjectProperty;
            return value;
        }
    }

    internal class WindowsManagementInstrumentationDefinition
    {
        private const char KeySeparator = '\\';

        public WindowsManagementInstrumentationDefinition(string key)
        {
            var keyTokens = key.Split(KeySeparator);

            QuerySource = keyTokens[0];
            PropertyName = keyTokens[1];
        }

        public string QuerySource { get; private set; }

        public string PropertyName { get; private set; }
    }
}