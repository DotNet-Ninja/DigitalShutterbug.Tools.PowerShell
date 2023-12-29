namespace DigitalShutterbug.Tools.PowerShell.Services;

public class FileSystem : IFileSystem
{
    public List<FileInfo> GetFiles(string path, string filter, bool recursive)
    {
        var root = new DirectoryInfo(path);
        var files = root.GetFiles(filter, (recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        return files.ToList();
    }

    public List<FileInfo> GetFiles(string path, IEnumerable<string> extensions, bool recursive)
    {
        var root = new DirectoryInfo(path);
        if (!root.Exists)
        {
            throw new DirectoryNotFoundException();
        }
        var files = root.GetFiles("*.*", (recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
            .Where(f => extensions.Contains(f.Extension, StringComparer.CurrentCultureIgnoreCase));
        return files.ToList();
    }

    public FileInfo GetFile(string path)
    {
        return new FileInfo(path);
    }

    public string ResolvePath(string path)
    {
        var result = path;
        if (result.StartsWith("~\\"))
        {
            result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), path.Substring(2));
        }

        result = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, result));

        return result;
    }
}