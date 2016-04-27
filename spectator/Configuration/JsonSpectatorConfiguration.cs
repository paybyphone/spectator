using System;
using System.Collections.Generic;
using log4net;
using Newtonsoft.Json;
using spectator.Configuration.Overrides;
using spectator.Infrastructure;

namespace spectator.Configuration
{
    public class JsonSpectatorConfiguration : ISpectatorOverrideConfiguration, ISpectatorConfiguration
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string StatsdHost { get; set; }

        int ISpectatorConfiguration.StatsdPort => StatsdPort ?? 0;

        public int? StatsdPort { get; set; }

        public string MetricPrefix { get; set; }

        TimeSpan ISpectatorConfiguration.Interval => Interval ?? TimeSpan.Zero;

        public TimeSpan? Interval { get; set; }

        public IList<MetricConfiguration> Metrics { get; set; }

        public static ISpectatorConfiguration LoadConfigFrom(string path)
        {
            Log.Debug($"Loading JSON configuration from '{path}'");

            return LoadJsonConfig<ISpectatorConfiguration>(path, () => new EmptyConfiguration(), LoadConfigFromString);
        }

        private static T LoadJsonConfig<T>(string path, Func<T> emptyConfigurationFactory, Func<string, T> loaderMethod)
        {
            var fileAdapter = new FileAdapter();

            try
            {
                var configContents = fileAdapter.ReadAllText(path);

                return loaderMethod(configContents);
            }
            catch(Exception ex)
            {
                Log.Warn($"Exception occurred loading JSON configuration file from '{path}' ('{ex.Message}'), assuming an empty configuration.");

                return emptyConfigurationFactory();
            }
        }

        public static JsonSpectatorConfiguration LoadConfigFromString(string contents)
        {
            return JsonConvert.DeserializeObject<JsonSpectatorConfiguration>(contents);
        }

        public static ISpectatorOverrideConfiguration LoadOverrideFrom(string path)
        {
            Log.Debug($"Loading JSON override configuration from '{path}'");

            return LoadJsonConfig<ISpectatorOverrideConfiguration>(path, () => new EmptyOverrideConfiguration(), LoadConfigFromString);
        }
    }
}