using Microsoft.EntityFrameworkCore;

namespace ZME.API.Shared.Models;

[Index(nameof(Title))]
[Index(nameof(Created))]
[Index(nameof(Updated))]
public class News
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int? EditorId { get; set; }
    public User? Editor { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTimeOffset Created { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
}
