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
                        serviceConfigurator.ConstructUsing(() => new SpectatorService());
                        serviceConfigurator.WhenStarted(myService => myService.Start());
                        serviceConfigurator.WhenStopped(myService => myService.Stop());
                    });

                hostConfigurator.RunAsLocalSystem();

                hostConfigurator.SetDisplayName("Spectator Service");
                hostConfigurator.SetDescription("Periodically monitors metrics/statistics and records them to a statsd server.");
                hostConfigurator.SetServiceName("Spectator");
            });
        }
    }
}
