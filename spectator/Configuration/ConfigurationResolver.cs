using System;
using System.Configuration;
using log4net;
using spectator.Configuration.Overrides;

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

            ISpectatorConfiguration config = new ExpiringConfigurationDecorator(InnerResolve, configRefresh);

            return config;
        }

        private ISpectatorConfiguration InnerResolve()
        {
            var consulHost = ConfigurationManager.AppSettings[@"Spectator.ConsulHost"];
            var consulKey = ConfigurationManager.AppSettings[@"Spectator.ConsulKey"];
            var jsonConfigFile = ConfigurationManager.AppSettings[@"Spectator.JsonConfigFile"] ?? DefaultSpectatorConfigFile;

            var baseJsonConfig = JsonSpectatorConfiguration.LoadConfigFrom(BaseSpectatorConfigFile);
            var overrideJsonConfig = Fallback.On(() => ConsulSpectatorOverrideConfiguration.LoadFrom(consulHost, consulKey, saveTo: jsonConfigFile),
                                                 () => JsonSpectatorConfiguration.LoadOverrideFrom(jsonConfigFile));

            Log.InfoFormat("Using combined configuration using base config file '{0}' ({1} metrics) and overriding with loaded {2} metrics", BaseSpectatorConfigFile, baseJsonConfig.Metrics.Count, overrideJsonConfig.Metrics.Count);

            return new CombinedSpectatorConfiguration(baseJsonConfig, overrideJsonConfig);
        }
    }
}