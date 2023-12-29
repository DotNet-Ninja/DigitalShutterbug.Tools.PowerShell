namespace DigitalShutterbug.Tools.PowerShell.Services;

public interface IFileSystem
{
    List<FileInfo> GetFiles(string path, string filter, bool recursive);
    List<FileInfo> GetFiles(string path, IEnumerable<string> extensions, bool recursive);
    FileInfo GetFile(string path);
    string ResolvePath(string path);
}