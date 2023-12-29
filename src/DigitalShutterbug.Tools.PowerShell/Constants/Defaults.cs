using System.Text.Json;

namespace DigitalShutterbug.Tools.PowerShell.Constants;

public class Defaults
{
    public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };
}