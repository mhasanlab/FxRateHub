namespace FxRateHub.Infrastructure.Configuration;

/// <summary>
/// Configuration options for the CurrencyFreaks API.
/// </summary>
public class CurrencyFreaksOptions
{
    /// <summary>
    /// The API key for authenticating with the CurrencyFreaks API.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
