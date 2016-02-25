using System;
using log4net;

namespace spectator.Configuration
{
    public static class Fallback
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ISpectatorConfiguration On(Func<ISpectatorConfiguration> configFactory, Func<ISpectatorConfiguration> fallbackFactory)
        {
            try
            {
                return configFactory();
            }
            catch (Exception ex)
            {
                Log.Warn("Couldn't obtain primary configuration, falling back to secondary configuration", ex);

                return fallbackFactory();
            }
        }
    }
}