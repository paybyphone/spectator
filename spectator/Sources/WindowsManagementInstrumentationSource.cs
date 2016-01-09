using System.Linq;
using System.Management;

namespace spectator.Sources
{
    public class WindowsManagementInstrumentationSource : IQueryableSource
    {
        public int QueryValue(string path)
        {
            var definition = new WindowsManagementInstrumentationDefinition(path);

            var searcher = new ManagementObjectSearcher("select * from " + definition.QuerySource);

            foreach (ManagementObject managedObject in searcher.Get())
            {
                return GetInfo(managedObject, definition.PropertyName);
            }

            return 0;
        }

        private static int GetInfo(ManagementObject managedObject, string property)
        {
            object manageObjectProperty = managedObject[property];

            ulong value = (ulong)manageObjectProperty;
            return (int)value;
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