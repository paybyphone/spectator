using System;
using System.Collections.Generic;
using System.Management;

namespace spectator.Sources.WMI
{
    public class WindowsManagementInstrumentationSource : IQueryableSource
    {
        public IEnumerable<Sample> QueryValue(string path)
        {
            var definition = new WindowsManagementInstrumentationDefinition(path);

            using (var searcher = new ManagementObjectSearcher("select * from " + definition.QuerySource))
            {
                foreach (var foundObject in searcher.Get())
                {
                    var managedObject = (ManagementObject) foundObject;
                    var value = GetInfo(managedObject, definition.PropertyName);

                    managedObject.Dispose();

                    return new[] {new Sample(string.Empty, value)};
                }

                throw new NotSupportedException($"Could not find property '{definition.PropertyName}' on any managed object in query source '{definition.QuerySource}'");
            }
        }

        private static double GetInfo(ManagementBaseObject managedObject, string property)
        {
            var manageObjectProperty = managedObject[property];

            double value = (ulong)manageObjectProperty;

            return value;
        }
    }
}