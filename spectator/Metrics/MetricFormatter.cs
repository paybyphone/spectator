using System.Collections.Generic;
using System.Net;

namespace spectator.Metrics
{
    public class MetricFormatter : IMetricFormatter
    {
        private readonly IDictionary<string, string> _registeredReplacements = new Dictionary<string, string>
            {
                { "machine", Dns.GetHostName() }
            };

        public string Format(string metricPrefix, string template)
        {
            var formatted = metricPrefix + "." + template;

            foreach (var replacement in _registeredReplacements)
            {
                formatted = formatted.Replace("{" + replacement.Key + "}", replacement.Value.ToLowerInvariant());
            }

            return formatted;
        }
    }
}