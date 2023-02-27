﻿namespace ZME.API.Shared.Models;

public class Comment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int SubmitterId { get; set; }
    public User? Submitter { get; set; }
    public bool Confidential { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}
