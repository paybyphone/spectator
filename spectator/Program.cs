using System;
using System.Configuration;
using log4net;
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
                        var configuration = LoadConfiguration();
                        serviceConfigurator.ConstructUsing(() =>
                            new SpectatorService(
                                configuration,
                                new QueryableSourceFactory(),
                                new StatsdPublisher(
                                        new Statsd(
                                                new StatsdUDP(
                                                    configuration.StatsdHost,
                                                    configuration.StatsdPort
                                                )
                                        )
                                ),
                                new MetricFormatter()
                            )
                        );
                        serviceConfigurator.WhenStarted(myService => myService.Start());
                        serviceConfigurator.WhenStopped(myService => myService.Stop());
                    });

                hostConfigurator.RunAsLocalSystem();

                hostConfigurator.SetDisplayName(@"Spectator Agent");
                hostConfigurator.SetDescription(@"Monitors system metrics and sends them to a statsd-compatible server.");
                hostConfigurator.SetServiceName(@"Spectator");
            });
        }

        private static ISpectatorConfiguration LoadConfiguration()
        {
            var consulHost = ConfigurationManager.AppSettings[@"Spectator.ConsulHost"];
            var configRefresh = TimeSpan.Parse(ConfigurationManager.AppSettings[@"Spectator.ConfigurationRefresh"]);

            if (!string.IsNullOrEmpty(consulHost))
            {
                var consulKey = ConfigurationManager.AppSettings[@"Spectator.ConsulKey"];

                try
                {
                    return new ExpiringConfigurationDecorator(() => ConsulSpectatorConfiguration.LoadFrom(consulHost, consulKey, saveTo: SpectatorConfigFile), configRefresh);
                }
                catch (Exception ex)
                {
                    Log.Warn(string.Format(@"Exception occurred while obtaining configuration from consul at '{0}' (key: '{1}'). Will fallback to using locally stored JSON configuration.", consulHost, consulKey), ex);
                }
            }

            return new ExpiringConfigurationDecorator(() => JsonSpectatorConfiguration.LoadFrom(SpectatorConfigFile), configRefresh);
        }
    }
}
