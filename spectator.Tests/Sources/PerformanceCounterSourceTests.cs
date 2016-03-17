using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using spectator.Sources.PerformanceCounters;

namespace spectator.Tests.Sources
{
    [TestFixture]
    public class PerformanceCounterSourceTests
    {
        private IDictionary<Tuple<string, string, string>, IPerformanceCounter> _registryDictionary;

        [SetUp]
        public void SetUp()
        {
            _registryDictionary = new Dictionary<Tuple<string, string, string>, IPerformanceCounter>();
        }

        [Test]
        public void when_querying_for_a_metric_across_all_instances_then_the_instances_are_read_from_the_category_repository()
        {
            var mockPerformanceCounter = new Mock<IPerformanceCounter>();
            var mockPerformanceCounterCategoryRepository = new Mock<IPerformanceCounterCategoryRepository>();
            mockPerformanceCounterCategoryRepository.Setup(m => m.GetInstances(It.IsAny<string>())).Returns(new[] { "foo" });

            var source = new PerformanceCounterSource(new PerformanceCounterRegistryTestHarness(_registryDictionary, (a, b, c) => mockPerformanceCounter.Object), mockPerformanceCounterCategoryRepository.Object);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            source.QueryValue("\\category(*)\\counter").ToList();

            mockPerformanceCounterCategoryRepository.Verify(m => m.GetInstances(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void when_querying_for_a_metric_in_a_particular_instances_then_the_instances_do_not_need_to_be_read()
        {
            var mockPerformanceCounter = new Mock<IPerformanceCounter>();
            var mockPerformanceCounterCategoryRepository = new Mock<IPerformanceCounterCategoryRepository>();
            mockPerformanceCounterCategoryRepository.Setup(m => m.GetInstances(It.IsAny<string>())).Returns(new[] { "foo" });

            var source = new PerformanceCounterSource(new PerformanceCounterRegistryTestHarness(_registryDictionary, (a, b, c) => mockPerformanceCounter.Object), mockPerformanceCounterCategoryRepository.Object);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            source.QueryValue("\\category(instance)\\counter").ToList();

            mockPerformanceCounterCategoryRepository.Verify(m => m.GetInstances(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void if_a_metric_is_being_queried_for_the_first_time_it_returns_no_value()
        {
            var mockPerformanceCounter = new Mock<IPerformanceCounter>();
            mockPerformanceCounter.Setup(m => m.NextValue()).Returns(5.0f);

            var mockPerformanceCounterCategoryRepository = new Mock<IPerformanceCounterCategoryRepository>();
            mockPerformanceCounterCategoryRepository.Setup(m => m.GetInstances(It.IsAny<string>())).Returns(new[] { "foo" });

            var source = new PerformanceCounterSource(new PerformanceCounterRegistryTestHarness(_registryDictionary, (a, b, c) => mockPerformanceCounter.Object), mockPerformanceCounterCategoryRepository.Object);

            var samples = source.QueryValue("\\category(*)\\counter").ToList();

            Assert.That(samples.Count, Is.EqualTo(0));
        }

        [Test]
        public void if_a_metric_is_being_queried_for_the_second_time_it_returns_a_measured_value()
        {
            var mockPerformanceCounter = new Mock<IPerformanceCounter>();
            mockPerformanceCounter.Setup(m => m.NextValue()).Returns(5.0f);

            var mockPerformanceCounterCategoryRepository = new Mock<IPerformanceCounterCategoryRepository>();
            mockPerformanceCounterCategoryRepository.Setup(m => m.GetInstances(It.IsAny<string>())).Returns(new[] { "foo" });

            var source = new PerformanceCounterSource(new PerformanceCounterRegistryTestHarness(_registryDictionary, (a, b, c) => mockPerformanceCounter.Object), mockPerformanceCounterCategoryRepository.Object);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            source.QueryValue("\\category(*)\\counter").ToList();
            var samples = source.QueryValue("\\category(*)\\counter").ToList();

            Assert.That(samples.Count, Is.EqualTo(1));
            Assert.That(samples.First().Value, Is.EqualTo(5.0f));
        }
    }
}