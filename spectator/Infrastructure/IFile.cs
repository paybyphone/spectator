namespace spectator.Infrastructure
{
    public interface IFile
    {
        void WriteAllText(string path, string text);

        string ReadAllText(string path);
    }
}