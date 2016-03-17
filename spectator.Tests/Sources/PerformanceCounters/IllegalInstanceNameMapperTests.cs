using NUnit.Framework;
using spectator.Sources.PerformanceCounters;

namespace spectator.Tests.Sources.PerformanceCounters
{
    [TestFixture]
    public class IllegalInstanceNameMapperTests
    {
        [TestCase("", "")]
        [TestCase("foo", "foo")]
        [TestCase("!@$%^&*_+=-[]{};:,.><?`~", "!@$%^&*_+=-[]{};:,.><?`~")]
        public void instance_name_with_no_illegal_characters_is_left_alone(string instanceName, string mappedName)
        {
            var mapper = new IllegalInstanceNameMapper();
            var result = mapper.Map(instanceName);

            Assert.That(result, Is.EqualTo(mappedName));
        }

        [TestCase("foo()#\\/", "foo[]___")]
        public void instance_name_with_illegal_characters_is_correctly_mapped(string instanceName, string mappedName)
        {
            var mapper = new IllegalInstanceNameMapper();
            var result = mapper.Map(instanceName);

            Assert.That(result, Is.EqualTo(mappedName));
        }
    }
}