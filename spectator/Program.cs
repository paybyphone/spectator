using System;
using System.Configuration;
using System.IO;
using log4net;
using Newtonsoft.Json;
using spectator.Configuration;
using spectator.Metrics;
using spectator.Sources;
using StatsdClient;
using Topshelf;

namespace spectator
{
    public class Program
    {
        private const string SpectatorConfigFile = @"spectator-config.json";
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<SpectatorService>(serviceConfigurator =>
                    {
                        var configuration = LoadJsonConfiguration();
                        serviceConfigurator.ConstructUsing(() => new SpectatorService(configuration, new QueryableSourceFactory(),
                            new StatsdPublisher(new Statsd(new StatsdUDP(configuration.StatsdHost, configuration.StatsdPort))), new MetricFormatter()));
                        serviceConfigurator.WhenStarted(myService => myService.Start());
                        serviceConfigurator.WhenStopped(myService => myService.Stop());
                    });

                hostConfigurator.RunAsLocalSystem();

                hostConfigurator.SetDisplayName("Spectator Service");
                hostConfigurator.SetDescription("Periodically monitors metrics/statistics and records them to a statsd server.");
                hostConfigurator.SetServiceName("Spectator");
            });
        }

        private static ISpectatorConfiguration LoadJsonConfiguration()
        {
            var consulHost = ConfigurationManager.AppSettings["Spectator.ConsulHost"];

            if (!string.IsNullOrEmpty(consulHost))
            {
                var consulKey = ConfigurationManager.AppSettings["Spectator.ConsulKey"];

                try
                {
                    var config = new ConsulConfiguration(consulHost, consulKey);

                    config.Save(SpectatorConfigFile);

                    return config;
                }
                catch (Exception ex)
                {
                    Log.Warn(string.Format("Exception occurred while obtaining configuration from consul at '{0}' (key: '{1}'). Will fallback to using locally stored JSON configuration.", consulHost, consulKey), ex);
                }
            }

            var configContents = File.ReadAllText(SpectatorConfigFile);

            return JsonConvert.DeserializeObject<JsonConfiguration>(configContents);
        }
    }
}
