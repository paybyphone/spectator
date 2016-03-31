using System;
using NUnit.Framework;
using spectator.Sources;
using spectator.Sources.WMI;

namespace spectator.Tests.Sources
{
    [TestFixture]
    public class WindowsManagementInstrumentationDefinitionTests
    {
        [Test]
        public void windows_management_instrumentation_definition_cannot_be_created_from_nothing()
        {
            Assert.Throws<ArgumentNullException>(() => new WindowsManagementInstrumentationDefinition(null));
        }

        [Test]
        public void windows_management_instrumentation_definition_must_start_with_slash()
        {
            Assert.Throws<ArgumentException>(() => new WindowsManagementInstrumentationDefinition("querysource"));
        }

        [Test]
        public void windows_management_instrumentation_definition_must_contain_a_querysource_and_property()
        {
            Assert.Throws<ArgumentException>(() => new WindowsManagementInstrumentationDefinition(""));
            Assert.Throws<ArgumentException>(() =>  new WindowsManagementInstrumentationDefinition("\\querysource"));
        }

        [Test]
        public void a_windows_management_instrumentation_can_be_defined_from_a_querysource_and_a_property()
        {
            var counter = new WindowsManagementInstrumentationDefinition("\\querysource\\property");

            Assert.That(counter.QuerySource, Is.EqualTo("querysource"));
            Assert.That(counter.PropertyName, Is.EqualTo("property"));
        }

        [TestCase("\\query%^@")]
        [TestCase("\\query source")]
        [TestCase("\\!*@(#&;")]
        public void querysource_cannot_contain_characters_other_than_alphanumerics_or_underscore(string querySource)
        {
            Assert.Throws<ArgumentException>(() => new WindowsManagementInstrumentationDefinition(querySource));
        }
    }
}
