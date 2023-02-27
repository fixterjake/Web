﻿namespace ZME.API.Shared.Models;

public class Faq
{
    public int Id { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public int Order { get; set; }
    public DateTimeOffset Created { get; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
}
