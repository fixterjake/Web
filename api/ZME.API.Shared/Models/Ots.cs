using Microsoft.EntityFrameworkCore;
using ZME.API.Shared.Enums;

namespace ZME.API.Shared.Models;

[Index(nameof(Position))]
[Index(nameof(Status))]
public class Ots
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int SubmitterId { get; set; }
    public required User Submitter { get; set; }
    public int? InstructorId { get; set; }
    public User? Instructor { get; set; }
    public int? TrainingRequestId { get; set; }
    public TrainingRequest? TrainingRequest { get; set; }
    public TrainingTicketPosition Position { get; set; }
    public required string Facility { get; set; }
    public OtsStatus Status { get; set; }
    public DateTimeOffset Created { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
}
