using System.Security.Cryptography;

namespace DigitalShutterbug.Tools.PowerShell.Model;

public class PhotoFileMetadata
{
    public PhotoFileMetadata()
    {}

    public PhotoFileMetadata(FileInfo file)
    {
        Name = file.Name;
        Path = file.FullName;
        LastModified = file.LastWriteTime;
        Length = file.Length;

        var number = file.Name.Substring(file.Name.Length-4 - file.Extension.Length);
        number = number.Substring(0, 4);
        if (int.TryParse(number, out var _))
        {
            PhotoNumber = number;
        }
    }
    
    public int MetadataVersion { get; set; } = 1;
    public string PhotoNumber { get; set; } = "0000";
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public DateTime LastModified { get; set; } = DateTime.MinValue;
    public long Length { get; set; } = 0;

    public bool HasChanged(FileInfo file)
    {
        if (!file.FullName.Equals(Path))
        {
            throw new InvalidOperationException("Source file and metadata are not the same.");
        }
        return LastModified!=file.LastWriteTime || Length !=file.Length;
    }

    public void ComputeHash(FileInfo file)
    {
        using var md5 = MD5.Create();
        using var stream = file.OpenRead();
        Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
    }
}