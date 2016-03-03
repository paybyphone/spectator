using System;
using Moq;
using NUnit.Framework;
using spectator.Configuration;

namespace spectator.Tests.Configuration
{
    [TestFixture]
    public class FallbackTests
    {
        [Test]
        public void when_primary_config_factory_fails_then_the_fallback_config_factory_is_used()
        {
            var fallbackCalled = false;
            Func<ISpectatorConfiguration> failingConfigFactory = () => { throw new Exception(); };
            Func<ISpectatorConfiguration> workingConfigFactory = () =>
                                                                       {
                                                                           fallbackCalled = true;
                                                                           return new Mock<ISpectatorConfiguration>().Object;
                                                                       };

            Fallback.On(failingConfigFactory, workingConfigFactory);

            Assert.That(fallbackCalled, Is.True);
        }

        [Test]
        public void when_primary_config_factory_works_then_the_fallback_config_factory_is_not_used()
        {
            var fallbackCalled = false;
            Func<ISpectatorConfiguration> workingConfigFactory = () => new Mock<ISpectatorConfiguration>().Object;
            Func<ISpectatorConfiguration> fallbackConfigFactory = () =>
                                                                       {
                                                                           fallbackCalled = true;
                                                                           return new Mock<ISpectatorConfiguration>().Object;
                                                                       };

            Fallback.On(workingConfigFactory, fallbackConfigFactory);

            Assert.That(fallbackCalled, Is.False);
        }
    }
}