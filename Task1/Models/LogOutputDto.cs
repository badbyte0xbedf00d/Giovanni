using System;

namespace Giovanni.Task1.Models;

public record LogOutputDto
{
    public DateTime DateTimeUtc { get; init; }
    public string Status { get; init; }
    public string BlobName { get; init; }
    public int StatusCode { get; init; }
}