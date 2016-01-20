using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using spectator.Infrastructure;

namespace spectator.Configuration
{
    public class JsonSpectatorConfiguration : ISpectatorConfiguration
    {
        public string StatsdHost { get; set; }

        public int StatsdPort { get; set; }

        public string MetricPrefix { get; set; }

        public TimeSpan Interval { get; set; }

        public IList<MetricConfiguration> Metrics { get; set; }

        public static ISpectatorConfiguration LoadFrom(string path)
        {
            var fileAdapter = new FileAdapter();
            var configContents = fileAdapter.ReadAllText(path);

            return LoadFromString(configContents);
        }
        public static ISpectatorConfiguration LoadFromString(string contents)
        {
            return JsonConvert.DeserializeObject<JsonSpectatorConfiguration>(contents);
        }
    }
}