using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using spectator.Infrastructure;

namespace spectator.Metrics
{
    public sealed class MetricFormatter : IMetricFormatter
    {
        private readonly IDictionary<string, string> _registeredReplacements = new Dictionary<string, string>
            {
                { "machine", Dns.GetHostName() }
            };

        public string Format(string metricPrefix, string instance, string template)
        {
            var formatted = (!metricPrefix.IsNullOrEmpty() ? metricPrefix + "." : "") + template;

            formatted = FormatTemplatedValues(formatted);
            formatted = FormatInstanceName(instance, formatted);
            formatted = Sanitise(formatted);

            return formatted;
        }

        private string FormatTemplatedValues(string formatted)
        {
            return _registeredReplacements.Aggregate(formatted, (current, replacement) => current.Replace("{" + replacement.Key + "}", replacement.Value.ToLowerInvariant()));
        }

        private static string FormatInstanceName(string instance, string formatted)
        {
            if (!string.IsNullOrEmpty(instance))
            {
                formatted = formatted.Replace("{instance}", instance.ToLowerInvariant());
            }

            return formatted;
        }

        private static string Sanitise(string formatted)
        {
            Must.NotBeNull(() => formatted);

            var charArray = formatted.ToCharArray();

            charArray = Array.FindAll(charArray, c => char.IsLetterOrDigit(c)
                                                   || c == '-'
                                                   || c == '_'
                                                   || c == '.');
            return new string(charArray);
        }
    }
}