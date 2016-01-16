using System.IO;
using NUnit.Framework;
using spectator.Infrastructure;

namespace spectator.Tests.Infrastructure
{
    [TestFixture]
    public class FileAdapterTests
    {
        private FileAdapter _fileAdapter;

        [SetUp]
        public void SetUp()
        {
            _fileAdapter = new FileAdapter();
        }

        [Test]
        public void file_adapter_can_read_and_write_lines_to_file()
        {
            var temporaryFilePath = Path.GetTempFileName();
            var originalText = "this is some temporary\n text to write\n to files\n";

            _fileAdapter.WriteAllText(temporaryFilePath, originalText);

            var writtenText = _fileAdapter.ReadAllText(temporaryFilePath);

            Assert.That(writtenText, Is.EqualTo(originalText));
        }
    }
}
