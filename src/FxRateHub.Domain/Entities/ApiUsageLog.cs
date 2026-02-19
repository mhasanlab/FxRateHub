using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FxRateHub.Domain.Common;

namespace FxRateHub.Domain.Entities;

/// <summary>
/// Represents an API usage log entry for tracking API request details.
/// </summary>
public class ApiUsageLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; private set; }

    [Required]
    public Guid ApiKeyId { get; private set; }

    [Required]
    [MaxLength(200)]
    [Column(TypeName = "varchar(200)")]
    public string Endpoint { get; private set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public string HttpMethod { get; private set; } = string.Empty;

    [Required]
    public int ResponseStatusCode { get; private set; }

    [Required]
    public DateTime RequestedAt { get; private set; }

    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string? IpAddress { get; private set; }

    [Required]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Navigation property to the associated API key.
    /// </summary>
    public ApiKey ApiKey { get; private set; } = null!;

    private ApiUsageLog() { }

    /// <summary>
    /// Creates a new API usage log entry.
    /// </summary>
    /// <param name="apiKeyId">The ID of the API key used.</param>
    /// <param name="endpoint">The API endpoint that was called.</param>
    /// <param name="httpMethod">The HTTP method used (GET, POST, etc.).</param>
    /// <param name="statusCode">The HTTP response status code.</param>
    /// <param name="ipAddress">The IP address of the requester.</param>
    /// <returns>A new ApiUsageLog instance configured with the provided values.</returns>
    public static ApiUsageLog Create(Guid apiKeyId, string endpoint, string httpMethod, int statusCode, string? ipAddress)
    {
        return new ApiUsageLog
        {
            ApiKeyId = apiKeyId,
            Endpoint = endpoint,
            HttpMethod = httpMethod.ToUpperInvariant(),
            ResponseStatusCode = statusCode,
            RequestedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };
    }
}
