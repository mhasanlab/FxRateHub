using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FxRateHub.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for foreign exchange rate provider services.
/// </summary>
public interface IFxRateProvider
{
    /// <summary>
    /// Gets the latest exchange rates for the specified base currency.
    /// </summary>
    /// <param name="baseCurrency">The base currency to get rates for (defaults to USD).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A dictionary containing currency codes and their corresponding exchange rates.</returns>
    Task<Dictionary<string, decimal>> GetLatestRatesAsync(string baseCurrency = "USD", CancellationToken cancellationToken = default);
}
