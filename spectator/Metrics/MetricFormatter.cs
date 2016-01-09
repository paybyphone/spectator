using System;
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

        public string Format(string metricPrefix, string instance, string template)
        {
            var formatted = metricPrefix + "." + template;

            foreach (var replacement in _registeredReplacements)
            {
                formatted = formatted.Replace("{" + replacement.Key + "}", replacement.Value.ToLowerInvariant());
            }

            if (!string.IsNullOrEmpty(instance))
            {
                formatted = formatted.Replace("{instance}", instance.ToLowerInvariant());
            }

            formatted = Sanitise(formatted);

            return formatted;
        }

        private string Sanitise(string formatted)
        {
            char[] charArray = formatted.ToCharArray();

            charArray = Array.FindAll(charArray, c => char.IsLetterOrDigit(c)
                                                      || char.IsWhiteSpace(c)
                                                      || c == '-'
                                                      || c == '_'
                                                      || c == '.');
            return new string(charArray);
        }
    }
}