﻿using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Types;
public class RequestInfo
{
    public Guid ApiId { get; set; }
    public Guid EndpointId { get; set; }
    public JsonObject? Definitions { get; set; }
    public Dictionary<string, object>? RouteParameters { get; set; }
    public Dictionary<string, object>? QueryParameters { get; set; }
    public Dictionary<string, object>? Headers { get; set; }
    public JsonNode? Body { get; set; }
    public IEnumerable<Action>? Actions { get; set; }
    public Dictionary<string, Response>? Response { get; set; }
}
