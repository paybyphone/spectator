using System;
using System.Collections.Generic;
using System.Net;
using log4net;
using spectator.Infrastructure;

namespace spectator.Configuration
{
    public class WebSpectatorConfiguration : ISpectatorConfiguration
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISpectatorConfiguration _innerConfiguration;
        private readonly string _configContents;

        public WebSpectatorConfiguration(Uri configurationUrl)
        {
            using (var client = new WebClient())
            {
                _configContents = client.DownloadString(configurationUrl);

                Log.InfoFormat("Using configuration read from web address '{0}'", configurationUrl);

                _innerConfiguration = JsonSpectatorConfiguration.LoadFromString(_configContents);
            }
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

        public static ISpectatorConfiguration LoadFrom(Uri configurationUrl, string saveTo)
        {
            var config = new WebSpectatorConfiguration(configurationUrl);

            config.Save(saveTo);

            return config;
        }
    }
}