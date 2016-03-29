using System.Linq;
using System.Text;

namespace spectator.Configuration
{
    public class ConfigurationDifferenceSummary
    {
        private readonly ISpectatorConfiguration _first;
        private readonly ISpectatorConfiguration _second;
        private string _summary;

        public ConfigurationDifferenceSummary(ISpectatorConfiguration first, ISpectatorConfiguration second)
        {
            _first = first;
            _second = second;

            CreateSummary();
        }

        private void CreateSummary()
        {
            var summaryStringBuilder = new StringBuilder();

            if (_first.StatsdHost != _second.StatsdHost)
            {
                summaryStringBuilder.AppendFormat("StatsdHost: '{0}' → '{1}'\r\n", _first.StatsdHost, _second.StatsdHost);
            }

            if (_first.StatsdPort != _second.StatsdPort)
            {
                summaryStringBuilder.AppendFormat("StatsdPort: '{0}' → '{1}'\r\n", _first.StatsdPort, _second.StatsdPort);
            }

            if (_first.MetricPrefix != _second.MetricPrefix)
            {
                summaryStringBuilder.AppendFormat("MetricPrefix: '{0}' → '{1}'\r\n", _first.MetricPrefix, _second.MetricPrefix);
            }

            if (_first.Interval != _second.Interval)
            {
                summaryStringBuilder.AppendFormat("Interval: '{0}' → '{1}'\r\n", _first.Interval, _second.Interval);
            }

            var removed = _first.Metrics.Where(m => _second.Metrics.All(o => o.Template != m.Template || o.Path != m.Path)).ToList();
            var added = _second.Metrics.Where(m => _first.Metrics.All(o => o.Template != m.Template || o.Path != m.Path)).ToList();

            if (removed.Any() || added.Any())
            {
                summaryStringBuilder.AppendLine("Metrics:");

                foreach (var a in added)
                {
                    var matchingRemoved = removed.FirstOrDefault(r => r.Template == a.Template);

                    if (matchingRemoved != null)
                    {
                        summaryStringBuilder.AppendFormat("    Replaced '{0}' → '{2}' with '{1}' → '{2}'\r\n", matchingRemoved.Path, a.Path, matchingRemoved.Template);
                        removed.Remove(matchingRemoved);
                    }
                    else
                    {
                        summaryStringBuilder.AppendFormat("    Added '{0}' → '{1}'\r\n", a.Path, a.Template);
                    }
                }

                foreach (var r in removed)
                {
                    summaryStringBuilder.AppendFormat("    Removed '{0}' → '{1}'\r\n", r.Path, r.Template);
                }
            }

            _summary = summaryStringBuilder.ToString();
        }

        public override string ToString()
        {
            return _summary;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(_summary);
        }
    }
}