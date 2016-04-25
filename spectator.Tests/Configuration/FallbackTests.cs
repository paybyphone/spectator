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
        public void when_primary_override_config_factory_fails_then_the_fallback_config_factory_is_used()
        {
            var fallbackCalled = false;
            Func<ISpectatorOverrideConfiguration> failingConfigFactory = () => { throw new Exception(); };
            Func<ISpectatorOverrideConfiguration> workingConfigFactory = () =>
                                                                       {
                                                                           fallbackCalled = true;
                                                                           return new Mock<ISpectatorOverrideConfiguration>().Object;
                                                                       };

            Fallback.On(failingConfigFactory, workingConfigFactory);

            Assert.That(fallbackCalled, Is.True);
        }

        [Test]
        public void when_primary_override_config_factory_works_then_the_fallback_config_factory_is_not_used()
        {
            var fallbackCalled = false;
            Func<ISpectatorOverrideConfiguration> workingConfigFactory = () => new Mock<ISpectatorOverrideConfiguration>().Object;
            Func<ISpectatorOverrideConfiguration> fallbackConfigFactory = () =>
                                                                       {
                                                                           fallbackCalled = true;
                                                                           return new Mock<ISpectatorOverrideConfiguration>().Object;
                                                                       };

            Fallback.On(workingConfigFactory, fallbackConfigFactory);

            Assert.That(fallbackCalled, Is.False);
        }
    }
}