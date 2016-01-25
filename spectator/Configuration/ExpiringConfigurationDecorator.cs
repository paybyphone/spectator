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

                _configurationInstance = _spectatorConfigurationFactory();

                _expiration = now + _expirationInterval;
            }

            return configValue(_configurationInstance);
        }
    }
}
