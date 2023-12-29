using System.Management.Automation;
using DigitalShutterbug.Tools.PowerShell.Constants;
using DigitalShutterbug.Tools.PowerShell.Services;

namespace DigitalShutterbug.Tools.PowerShell.CmdLets;

[Cmdlet(VerbsCommon.Get, Nouns.PhotoFile)]
[OutputType(typeof(FileInfo))]
public class GetPhotoFilesCommand: Cmdlet
{
    private readonly IFileSystem _fileSystem;

    public GetPhotoFilesCommand():this(new FileSystem())
    {
    }

    public GetPhotoFilesCommand(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }


    [Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0)]
    public string Path { get; set; } = Environment.CurrentDirectory;

    [Parameter(Mandatory = false)]
    public SwitchParameter Recursive { get; set; } = false;

    public string ResolvedPath =>
        System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, Path));

    protected override void ProcessRecord()
    {
        var files = _fileSystem.GetFiles(ResolvedPath, PhotoFiles.RawExtensions, Recursive);
        files.ForEach(WriteObject);
    }
}
