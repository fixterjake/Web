using System.Text.Json.Serialization;
using ZME.API.Shared.Enums;

namespace ZME.API.Shared.Models;

public class EventPosition
{
    public int Id { get; set; }
    public int EventId { get; set; }

    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Event? Event { get; set; }
    public required string Name { get; set; }
    public Rating MinRating { get; set; }
    public bool Available { get; set; }
}
