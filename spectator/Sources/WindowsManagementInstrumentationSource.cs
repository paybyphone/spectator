using System;
using System.Collections.Generic;
using System.Management;

namespace spectator.Sources
{
    public class WindowsManagementInstrumentationSource : IQueryableSource
    {
        public IEnumerable<Sample> QueryValue(string path)
        {
            var definition = new WindowsManagementInstrumentationDefinition(path);

            using (var searcher = new ManagementObjectSearcher("select * from " + definition.QuerySource))
            {
                foreach (ManagementObject managedObject in searcher.Get())
                {
                    var value = GetInfo(managedObject, definition.PropertyName);

                    managedObject.Dispose();

                    return new[] {new Sample(string.Empty, value)};
                }

                throw new NotSupportedException(string.Format("Could not find property '{0}' on any managed object in query source '{1}'", definition.PropertyName, definition.QuerySource));
            }
        }

        private static double GetInfo(ManagementObject managedObject, string property)
        {
            object manageObjectProperty = managedObject[property];

            double value = (ulong)manageObjectProperty;

            return value;
        }
    }
}