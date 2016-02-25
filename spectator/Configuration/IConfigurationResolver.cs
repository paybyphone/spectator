namespace spectator.Configuration
{
    public interface IConfigurationResolver
    {
        ISpectatorConfiguration Resolve();
    }
}