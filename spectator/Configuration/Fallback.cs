using System;
using log4net;
using spectator.Configuration.Overrides;

namespace spectator.Configuration
{
    public static class Fallback
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ISpectatorOverrideConfiguration On(Func<ISpectatorOverrideConfiguration> configFactory, Func<ISpectatorOverrideConfiguration> fallbackFactory)
        {
            try
            {
                return configFactory();
            }
            catch (Exception ex)
            {
                Log.WarnFormat("Couldn't obtain primary configuration ('{0}'), falling back to secondary configuration", ex.Message);

                return fallbackFactory();
            }
        }
    }
}