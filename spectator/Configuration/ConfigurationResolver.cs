using System;
using System.Configuration;
using log4net;

namespace spectator.Configuration
{
    public class ConfigurationResolver : IConfigurationResolver
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const string DefaultSpectatorConfigFile = @"spectator-config.json";
        public const string BaseSpectatorConfigFile = @"base-spectator-config.json";

        public ISpectatorConfiguration Resolve()
        {
            var configRefresh = TimeSpan.Parse(ConfigurationManager.AppSettings[@"Spectator.ConfigurationRefresh"] ?? @"00:00:00");

            var consulHost = ConfigurationManager.AppSettings[@"Spectator.ConsulHost"];
            var consulKey = ConfigurationManager.AppSettings[@"Spectator.ConsulKey"];

            var jsonConfigFile = ConfigurationManager.AppSettings[@"Spectator.JsonConfigFile"] ?? DefaultSpectatorConfigFile;

            var baseJsonConfigFile = JsonSpectatorConfiguration.LoadFrom(BaseSpectatorConfigFile);

            ISpectatorConfiguration config =
                new ExpiringConfigurationDecorator(() =>
                    Fallback.On(() => ConsulSpectatorConfiguration.LoadFrom(consulHost, consulKey, saveTo: jsonConfigFile),
                                () => JsonSpectatorConfiguration.LoadFrom(jsonConfigFile)), configRefresh);

            if (!(baseJsonConfigFile is EmptyConfiguration))
            {
                Log.InfoFormat("Using combined configuration using base config file '{0}' ({1} metrics) and overriding with loaded {3} metrics", BaseSpectatorConfigFile, baseJsonConfigFile.Metrics.Count, config.Metrics);

                return new CombinedSpectatorConfiguration(baseJsonConfigFile, config);
            }

            return config;
        }
    }
}