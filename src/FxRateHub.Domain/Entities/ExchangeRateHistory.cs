using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FxRateHub.Domain.Entities;

/// <summary>
/// Represents historical exchange rate records for tracking rate changes over time
/// </summary>
public class ExchangeRateHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; private set; }

    [Required]
    [MaxLength(3)]
    [Column(TypeName = "nvarchar(3)")]
    public string BaseCurrency { get; private set; } = "USD";

    [Required]
    [MaxLength(10)]
    [Column(TypeName = "nvarchar(10)")]
    public string TargetCurrency { get; private set; } = string.Empty;

    [Column(TypeName = "decimal(28,8)")]
    public decimal Rate { get; private set; }

    /// <summary>
    /// When this rate was recorded (from UpdatedAt of original ExchangeRate)
    /// </summary>
    public DateTime RecordedAt { get; private set; }

    /// <summary>
    /// When this history record was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    private ExchangeRateHistory() { }

    /// <summary>
    /// Factory method to create a new ExchangeRateHistory
    /// </summary>
    public static ExchangeRateHistory Create(string baseCurrency, string targetCurrency, decimal rate, DateTime recordedAt)
    {
        var history = new ExchangeRateHistory
        {
            BaseCurrency = baseCurrency.ToUpperInvariant(),
            TargetCurrency = targetCurrency.ToUpperInvariant(),
            Rate = rate,
            RecordedAt = recordedAt,
            CreatedAt = DateTime.UtcNow
        };
        return history;
    }
}
