using ZME.API.Shared.Enums;

namespace ZME.API.Shared.Models;

public class Feedback
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public required string UserCallsign { get; set; }
    public required string Description { get; set; }
    public FeedbackStatus Status { get; set; }
    public DateTimeOffset Created { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
}
