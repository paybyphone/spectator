using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using spectator.Sources;

namespace spectator.Tests.Sources
{
    [TestFixture]
    public class PerformanceCounterRegistryTests
    {
        private IDictionary<Tuple<string, string, string>, IPerformanceCounter> _registryDictionary;

        [SetUp]
        public void SetUp()
        {
            _registryDictionary = new Dictionary<Tuple<string, string, string>, IPerformanceCounter>();
        }

        [Test]
        public void when_reading_a_performance_counter_for_the_first_time_then_it_is_added_to_the_registry()
        {
            var mockPerformanceCounter = new Mock<IPerformanceCounter>();
            var registry = new PerformanceCounterRegistryTestHarness(_registryDictionary, (a, b, c) => mockPerformanceCounter.Object);

            registry.Read("category", "counter", "instance");

            Assert.That(_registryDictionary.Count, Is.EqualTo(1));
            mockPerformanceCounter.Verify(m => m.NextValue(), Times.Once);
        }

        [Test]
        public void when_reading_a_performance_counter_for_the_second_time_then_it_is_read_from_the_registered_performance_counter()
        {
            var mockPerformanceCounter = new Mock<IPerformanceCounter>();
            var registry = new PerformanceCounterRegistryTestHarness(_registryDictionary, (a, b, c) => mockPerformanceCounter.Object);

            registry.Read("category", "counter", "instance");
            registry.Read("category", "counter", "instance");

            Assert.That(_registryDictionary.Count, Is.EqualTo(1));
            mockPerformanceCounter.Verify(m => m.NextValue(), Times.Exactly(2));
        }
    }
}