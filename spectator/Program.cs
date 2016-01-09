using System.Configuration;
using System.IO;
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

                return new ConsulConfiguration(consulHost, consulKey);
            }

            var configContents = File.ReadAllText("spectator-config.json");

            return JsonConvert.DeserializeObject<JsonConfiguration>(configContents);
        }
    }
}
