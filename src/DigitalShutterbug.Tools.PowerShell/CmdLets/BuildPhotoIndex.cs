using System.Management.Automation;
using System.Text.Json;
using DigitalShutterbug.Tools.PowerShell.Constants;
using DigitalShutterbug.Tools.PowerShell.Model;
using DigitalShutterbug.Tools.PowerShell.Services;

namespace DigitalShutterbug.Tools.PowerShell.CmdLets;

[Cmdlet(VerbsLifecycle.Build, Nouns.PhotoIndex)]
public class BuildPhotoIndex: Cmdlet
{
    private readonly IFileSystem _fileSystem;

    public BuildPhotoIndex():this(new FileSystem())
    {}

    public BuildPhotoIndex(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
    public string Directory { get; set; } = string.Empty;

    public string ResolvedRoot => _fileSystem.ResolvePath(Directory); 

    public DirectoryInfo Root { get; set; }

    protected override void BeginProcessing()
    {
        Root = new DirectoryInfo(ResolvedRoot);
        base.BeginProcessing();
    }

    protected override void ProcessRecord()
    {
        var indexCache = new List<PhotoFileMetadata>();
        var indexFile = _fileSystem.GetFile(Path.Combine(Root.FullName, ".index.json"));
        if (indexFile.Exists)
        {
            using var reader = indexFile.OpenText();
            var json = reader.ReadToEnd();
            if (json.Length > 0)
            {
                indexCache = JsonSerializer.Deserialize<List<PhotoFileMetadata>>(json);
            }
        }
        var files = _fileSystem.GetFiles(Root.FullName, PhotoFiles.RawExtensions, true);
        var count = files.Count;
        var processed = 0;
        var progress = new ProgressRecord(1, "Building Index", "Percent Completed 0%");
        foreach (var file in files)
        {
            var tmp = new PhotoFileMetadata(file);
            var existing = indexCache.SingleOrDefault(c => c.Path == file.FullName);
            if (existing is null)
            {
                tmp.ComputeHash(file);
                indexCache.Add(tmp);
            }
            else if (existing.HasChanged(file))
            {
                existing.ComputeHash(file);
                existing.LastModified = file.LastWriteTime;
                existing.Length=file.Length;
            }
            processed = processed + 1;
            var percent = (processed * 100) / count;
            progress.PercentComplete = percent;
            progress.StatusDescription = $"Percent Completed {percent}%";
            WriteProgress(progress);
        }
        var indexJson = JsonSerializer.Serialize(indexCache, Defaults.JsonOptions);
        using var writer = indexFile.CreateText();
        writer.Write(indexJson);
        base.ProcessRecord();
    }
}