using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Consul;
using log4net;
using Newtonsoft.Json;

namespace spectator.Configuration
{
    public class ConsulConfiguration : ISpectatorConfiguration
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly JsonConfiguration _innerConfiguration;
        private string _configContents;

        public ConsulConfiguration(string host, string key)
        {
            var client = new Client(new ConsulClientConfiguration { Address = host });

            var getPair = client.KV.Get(key);
            _configContents = Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);

            Log.InfoFormat("Using configuration read from consul host '{0}', in key '{1}'", host, key);

            _innerConfiguration = JsonConvert.DeserializeObject<JsonConfiguration>(_configContents);
        }

        public void Save(string destination)
        {
            File.WriteAllText(destination, _configContents, Encoding.UTF8);
        }

        public string StatsdHost { get { return _innerConfiguration.StatsdHost; } }

        public int StatsdPort { get { return _innerConfiguration.StatsdPort; } }

        public string MetricPrefix { get { return _innerConfiguration.MetricPrefix; } }

        public TimeSpan Interval { get { return _innerConfiguration.Interval; } }

        public IList<MetricConfiguration> Metrics { get { return _innerConfiguration.Metrics; } }
    }
}