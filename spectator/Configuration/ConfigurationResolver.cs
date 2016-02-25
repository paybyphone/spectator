using System;
using System.Configuration;
using log4net;

namespace spectator.Configuration
{
    public class ConfigurationResolver : IConfigurationResolver
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string DefaultSpectatorConfigFile = @"spectator-config.json";
        private const string BaseSpectatorConfigFile = @"base-spectator-config";

        public ISpectatorConfiguration Resolve()
        {
            var configRefresh = TimeSpan.Parse(ConfigurationManager.AppSettings[@"Spectator.ConfigurationRefresh"]);

            var consulHost = ConfigurationManager.AppSettings[@"Spectator.ConsulHost"];
            var consulKey = ConfigurationManager.AppSettings[@"Spectator.ConsulKey"];

            var jsonConfigFile = ConfigurationManager.AppSettings[@"Spectator.JsonConfigFile"] ?? DefaultSpectatorConfigFile;

            var baseJsonConfigFile = JsonSpectatorConfiguration.LoadFrom(BaseSpectatorConfigFile);

            ISpectatorConfiguration config;

            try
            {
                config =  new ExpiringConfigurationDecorator(() => ConsulSpectatorConfiguration.LoadFrom(consulHost, consulKey, saveTo: jsonConfigFile), configRefresh);
            }
            catch (Exception ex)
            {
                Log.Warn(string.Format(@"Exception occurred while obtaining configuration from consul at '{0}' (key: '{1}'). Will fallback to using locally stored JSON configuration.", consulHost, consulKey), ex);

                config = new ExpiringConfigurationDecorator(() => JsonSpectatorConfiguration.LoadFrom(jsonConfigFile), configRefresh);
            }

            if (!(baseJsonConfigFile is EmptyConfiguration))
            {
                Log.InfoFormat("Using combined configuration based on '{0}'", BaseSpectatorConfigFile);

                return new CombinedSpectatorConfiguration(baseJsonConfigFile, config);
            }

            return config;
        }
    }
}