using System;
using log4net;

namespace spectator.Configuration
{
    public static class Fallback
    {
        public static ISpectatorConfiguration On(Func<ISpectatorConfiguration> configFactory, Func<ISpectatorConfiguration> fallbackFactory)
        {
            try
            {
                return configFactory();
            }
            catch (Exception)
            {
                return fallbackFactory();
            }
        }

    }
}