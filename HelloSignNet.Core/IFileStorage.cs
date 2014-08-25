using System.IO;

namespace HelloSignNet.Core
{
    public interface IFileStorage
    {
        void SaveFileAsync(Stream stream, string filepath, string filename);
        string LoadFileAsync(string filename);
    }
}