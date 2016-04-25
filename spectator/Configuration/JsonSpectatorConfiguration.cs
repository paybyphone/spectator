using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using spectator.Infrastructure;

namespace spectator.Configuration
{
    public class JsonSpectatorConfiguration : ISpectatorOverrideConfiguration, ISpectatorConfiguration
    {
        public string StatsdHost { get; set; }

        int ISpectatorConfiguration.StatsdPort => StatsdPort ?? 0;

        public int? StatsdPort { get; set; }

        public string MetricPrefix { get; set; }

        TimeSpan ISpectatorConfiguration.Interval => Interval ?? TimeSpan.Zero;

        public TimeSpan? Interval { get; set; }

        public IList<MetricConfiguration> Metrics { get; set; }

        public static ISpectatorConfiguration LoadConfigFrom(string path)
        {
            var fileAdapter = new FileAdapter();

            try
            {
                var configContents = fileAdapter.ReadAllText(path);

                return LoadConfigFromString(configContents);
            }
            catch
            {
                return new EmptyConfiguration();
            }
        }

        public static JsonSpectatorConfiguration LoadConfigFromString(string contents)
        {
            return JsonConvert.DeserializeObject<JsonSpectatorConfiguration>(contents);
        }

        public static ISpectatorOverrideConfiguration LoadOverrideFrom(string path)
        {
            var fileAdapter = new FileAdapter();

            try
            {
                var configContents = fileAdapter.ReadAllText(path);

                return LoadConfigFromString(configContents);
            }
            catch
            {
                return new EmptyOverrideConfiguration();
            }
        }
    }
}