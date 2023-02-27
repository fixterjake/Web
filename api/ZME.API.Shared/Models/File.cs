using ZME.API.Shared.Enums;

namespace ZME.API.Shared.Models;

public class File
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Version { get; set; }
    public string? FileUrl { get; set; }
    public FileType Type { get; set; }
    public DateTimeOffset Created { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
}
