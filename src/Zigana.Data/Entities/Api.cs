﻿using Zigana.Data.Abstractions;

namespace Zigana.Data.Entities;
public class Api : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Path { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Defs { get; set; }
    public bool IsActive { get; set; }
}
