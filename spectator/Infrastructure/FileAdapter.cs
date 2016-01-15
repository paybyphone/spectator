using System.IO;
using System.Text;

namespace spectator.Infrastructure
{
    public class FileAdapter : IFile
    {
        public void WriteAllText(string path, string text)
        {
            File.WriteAllText(path, text, Encoding.UTF8);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}