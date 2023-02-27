using System.Text.Json.Serialization;
using ZME.API.Shared.Enums;

namespace ZME.API.Shared.Models;

public class EventRegistration
{
    public int Id { get; set; }
    public int UserId { get; set; }

    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public User? User { get; set; }
    public int EventId { get; set; }

    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Event? Event { get; set; }
    public int EventPositionId { get; set; }
    public EventPosition? EventPosition { get; set; }
    public EventRegistrationStatus Status { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public DateTimeOffset Created { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
}
