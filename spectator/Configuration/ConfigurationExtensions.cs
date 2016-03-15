using Newtonsoft.Json;

namespace spectator.Configuration
{
    public static class ConfigurationExtensions
    {
        public static string PrettyPrint(this ISpectatorConfiguration configuration)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(configuration, Formatting.None, jsonSerializerSettings);
        }
    }
}