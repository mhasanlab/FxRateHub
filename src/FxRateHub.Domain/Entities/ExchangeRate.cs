using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FxRateHub.Domain.Entities;

/// <summary>
/// Represents the latest exchange rate between two currencies
/// </summary>
public class ExchangeRate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private set; }

    [Required]
    [MaxLength(3)]
    [Column(TypeName = "nvarchar(3)")]
    public string BaseCurrency { get; private set; } = "USD";

    [Required]
    [MaxLength(3)]
    [Column(TypeName = "nvarchar(3)")]
    public string TargetCurrency { get; private set; } = string.Empty;

    [Column(TypeName = "decimal(18,8)")]
    public decimal Rate { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private ExchangeRate() { }

    /// <summary>
    /// Factory method to create a new ExchangeRate
    /// </summary>
    public static ExchangeRate Create(string baseCurrency, string targetCurrency, decimal rate)
    {
        var exchangeRate = new ExchangeRate
        {
            BaseCurrency = baseCurrency.ToUpperInvariant(),
            TargetCurrency = targetCurrency.ToUpperInvariant(),
            Rate = rate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        return exchangeRate;
    }

    /// <summary>
    /// Converts this rate to a history record
    /// </summary>
    public ExchangeRateHistory ToHistory()
    {
        return ExchangeRateHistory.Create(
            BaseCurrency,
            TargetCurrency,
            Rate,
            UpdatedAt
        );
    }

    /// <summary>
    /// Updates the exchange rate
    /// </summary>
    public void Update(decimal newRate)
    {
        Rate = newRate;
        UpdatedAt = DateTime.UtcNow;
    }
}
