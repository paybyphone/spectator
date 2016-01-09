using System;
using System.Management;

namespace spectator.Sources
{
    public class WindowsManagementInstrumentationSource : IQueryableSource
    {
        public double QueryValue(string path)
        {
            var definition = new WindowsManagementInstrumentationDefinition(path);

            var searcher = new ManagementObjectSearcher("select * from " + definition.QuerySource);

            foreach (ManagementObject managedObject in searcher.Get())
            {
                return GetInfo(managedObject, definition.PropertyName);
            }

            throw new NotSupportedException();
        }

        private static double GetInfo(ManagementObject managedObject, string property)
        {
            object manageObjectProperty = managedObject[property];

            double value = (ulong)manageObjectProperty;

            return value;
        }
    }
}