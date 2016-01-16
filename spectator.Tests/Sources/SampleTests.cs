using NUnit.Framework;
using spectator.Sources;

namespace spectator.Tests.Sources
{
    [TestFixture]
    public class SampleTests
    {
        [Test]
        public void a_sample_can_have_positive_or_negative_values()
        {
            Assert.DoesNotThrow(() => new Sample(Some.String(), -1));
            Assert.DoesNotThrow(() => new Sample(Some.String(), 1));
        }

        [Test]
        public void a_sample_does_not_require_an_associated_instance()
        {
            Assert.DoesNotThrow(() => new Sample(string.Empty, 1));
            Assert.DoesNotThrow(() => new Sample(null, 1));
        }
    }
}
