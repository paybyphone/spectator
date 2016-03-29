using System.IO;
using System.Text;
using NUnit.Framework;
using spectator.Configuration;

namespace spectator.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationResolverTests
    {
        [SetUp]
        public void SetUp()
        {
            ResetBaseConfiguration();
        }

        private static void ResetBaseConfiguration()
        {
            if (File.Exists(ConfigurationResolver.BaseSpectatorConfigFile))
            {
                File.Delete(ConfigurationResolver.BaseSpectatorConfigFile);
            }
        }

        [Test]
        public void when_there_is_no_base_configuration_then_the_resulting_config_is_not_combined()
        {
            var resolver = new ConfigurationResolver();
            var configuration = resolver.Resolve();

            Assert.That(configuration, Is.Not.TypeOf<CombinedSpectatorConfiguration>());
        }

        [Test]
        public void when_there_is_an_existing_base_configuration_then_the_resulting_config_is_combined()
        {
            File.WriteAllText(ConfigurationResolver.BaseSpectatorConfigFile, @"{
                      ""statsdHost"": ""basehost"",
                      ""statsdPort"": 8125,
                      ""metricPrefix"": ""base.prefix"",
                      ""interval"":  ""00:00:01"",
                      ""metrics"": [
                        {
                          ""source"": ""performanceCounter"",
                          ""path"": ""\\Processor(_Total)\\Interrupts/sec"",
                          ""type"": ""gauge"",
                          ""template"": ""base.performance.counter""
                        },
                        {
                          ""source"": ""performanceCounter"",
                          ""path"": ""\\Processor(_Total)\\Interrupts/sec"",
                          ""type"": ""gauge"",
                          ""template"": ""overriden.performance.counter""
                        }
                      ]
                    }", Encoding.UTF8);

            var resolver = new ConfigurationResolver();
            var configuration = resolver.Resolve();

            Assert.That(configuration, Is.TypeOf<ExpiringConfigurationDecorator>());

            var expiringConfig = (ExpiringConfigurationDecorator)configuration;
            var innerConfiguration = expiringConfig.InnerConfiguration;
            Assert.That(innerConfiguration, Is.TypeOf<CombinedSpectatorConfiguration>());
        }
    }
}