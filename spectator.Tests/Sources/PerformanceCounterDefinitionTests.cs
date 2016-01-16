using System;
using NUnit.Framework;
using spectator.Sources;

namespace spectator.Tests.Sources
{
    [TestFixture]
    public class PerformanceCounterDefinitionTests
    {
        [Test]
        public void performance_counter_definition_cannot_be_created_from_nothing()
        {
            Assert.Throws<ArgumentNullException>(() => new PerformanceCounterDefinition(null));
        }

        [Test]
        public void performance_counter_definition_must_start_with_slash()
        {
            Assert.Throws<ArgumentException>(() => new PerformanceCounterDefinition("category"));
        }

        [Test]
        public void performance_counter_definition_must_contain_a_category_and_counter()
        {
            Assert.Throws<ArgumentException>(() => new PerformanceCounterDefinition(""));
            Assert.Throws<ArgumentException>(() =>  new PerformanceCounterDefinition("\\category"));
        }

        [Test]
        public void a_performance_counter_can_be_defined_from_a_category_and_a_counter()
        {
            var counter = new PerformanceCounterDefinition("\\category\\counter");

            Assert.That(counter.CategoryName, Is.EqualTo("category"));
            Assert.That(counter.CounterName, Is.EqualTo("counter"));
            Assert.That(counter.InstanceName, Is.Null);
        }

        [Test]
        public void a_performance_counter_can_be_defined_from_a_category_with_all_instances_and_a_counter()
        {
            var counter = new PerformanceCounterDefinition("\\category(*)\\counter");

            Assert.That(counter.CategoryName, Is.EqualTo("category"));
            Assert.That(counter.CounterName, Is.EqualTo("counter"));
            Assert.That(counter.InstanceName, Is.EqualTo("*"));
            Assert.That(counter.AllInstances, Is.True);
        }

        [Test]
        public void a_performance_counter_can_be_defined_from_a_category_with_a_particular_instance_and_a_counter()
        {
            var counter = new PerformanceCounterDefinition("\\category(instance)\\counter");

            Assert.That(counter.CategoryName, Is.EqualTo("category"));
            Assert.That(counter.CounterName, Is.EqualTo("counter"));
            Assert.That(counter.InstanceName, Is.EqualTo("instance"));
            Assert.That(counter.AllInstances, Is.False);
        }
    }
}
