using System;
using System.Collections.Generic;
using log4net;

namespace spectator.Configuration
{
    public class ExpiringConfigurationDecorator : ISpectatorConfiguration
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Func<ISpectatorConfiguration> _spectatorConfigurationFactory;
        private readonly TimeSpan _expirationInterval;
        private ISpectatorConfiguration _configurationInstance;
        private DateTimeOffset _expiration;

        public ExpiringConfigurationDecorator(Func<ISpectatorConfiguration> spectatorConfigurationFactory, TimeSpan expirationInterval)
        {
            _spectatorConfigurationFactory = spectatorConfigurationFactory;
            _expirationInterval = expirationInterval;
            _expiration = DateTimeOffset.MinValue;
        }

        internal ISpectatorConfiguration InnerConfiguration { get { return Get(c => c); } }

        public string StatsdHost { get { return Get(c => c.StatsdHost); } }

        public int StatsdPort { get { return Get(c => c.StatsdPort); } }

        public string MetricPrefix { get { return Get(c => c.MetricPrefix); } }

        public TimeSpan Interval { get { return Get(c => c.Interval); } }

        public IList<MetricConfiguration> Metrics { get { return Get(c => c.Metrics); } }

        private T Get<T>(Func<ISpectatorConfiguration, T> configValue)
        {
            var now = DateTimeOffset.UtcNow;
            if (now > _expiration)
            {
                Log.Info("Configuration has expired, refreshing from source");

                var newConfig = _spectatorConfigurationFactory();

                _expiration = now + _expirationInterval;

                Log.Info("Configuration loaded.");

                if (_configurationInstance != null)
                {
                    var diff = new ConfigurationDifferenceSummary(_configurationInstance, newConfig);

                    if (!diff.IsEmpty())
                    {
                        Log.Info("Configuration changed: \n" + diff);
                    }
                }

                _configurationInstance = newConfig;
                Log.InfoFormat("Expires next at: {0}", _expiration.ToString("o"));
            }

            return configValue(_configurationInstance);
        }
    }
}
