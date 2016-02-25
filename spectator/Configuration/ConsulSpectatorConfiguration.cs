using System;
using System.Collections.Generic;
using System.Text;
using Consul;
using log4net;
using spectator.Infrastructure;

namespace spectator.Configuration
{
    public class ConsulSpectatorConfiguration : ISpectatorConfiguration
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISpectatorConfiguration _innerConfiguration;
        private readonly string _configContents;

        public ConsulSpectatorConfiguration(string host, string key)
        {
            var client = string.IsNullOrEmpty(host) ? new Client() : new Client(new ConsulClientConfiguration { Address = host });

            var getPair = client.KV.Get(key);
            _configContents = Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);

            Log.InfoFormat("Using configuration read from consul host '{0}', in key '{1}'", host, key);

            _innerConfiguration = JsonSpectatorConfiguration.LoadFromString(_configContents);
        }

        public string StatsdHost { get { return _innerConfiguration.StatsdHost; } }

        public int StatsdPort { get { return _innerConfiguration.StatsdPort; } }

        public string MetricPrefix { get { return _innerConfiguration.MetricPrefix; } }

        public TimeSpan Interval { get { return _innerConfiguration.Interval; } }

        public IList<MetricConfiguration> Metrics { get { return _innerConfiguration.Metrics; } }

        private void Save(string destinationPath)
        {
            var fileAdapter = new FileAdapter();
            fileAdapter.WriteAllText(destinationPath, _configContents);
        }

        public static ISpectatorConfiguration LoadFrom(string host, string key, string saveTo)
        {
            if (string.IsNullOrEmpty(host) && string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("A consul host or key must be provided to use a consul configuration.");
            }

            var config = new ConsulSpectatorConfiguration(host, key);

            config.Save(saveTo);

            return config;
        }
    }
}