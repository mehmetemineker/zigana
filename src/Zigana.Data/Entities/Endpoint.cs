﻿using Zigana.Data.Abstractions;

namespace Zigana.Data.Entities;
public class Endpoint : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApiId { get; set; }
    public required string Path { get; set; }
    public required string Method { get; set; }
    public string? Actions { get; set; }
    public string? Response { get; set; }
    public bool IsActive { get; set; }

    // Api entity reference
    public string? Defs { get; set; }
}

