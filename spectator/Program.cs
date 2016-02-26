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
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            var configurationResolver = new ConfigurationResolver();

            Log.Info("Starting spectator topshelf host");

            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<SpectatorService>(serviceConfigurator =>
                    {
                        var configuration = configurationResolver.Resolve();
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
    }
}
