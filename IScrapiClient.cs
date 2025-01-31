namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// The official ScrAPI client to make web scraping calls using the ScrAPI service.
/// </summary>
/// <seealso cref="IDisposable"/>
public interface IScrapiClient : IDisposable
{
  /// <summary>
  /// Perform a web scraping operation using the provided URL.
  /// </summary>
  /// <param name="request">The scrape request options.</param>
  /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
  /// <exception cref="ArgumentNullException">Thrown when the request object is <c>null</c>.</exception>
  /// <exception cref="ScrapiException">Can be thrown when there are problems with the scrape operation.</exception>
  /// <returns>
  /// The scrape content response or an error if scraping failed.
  /// </returns>
  Task<ScrapeResponse?> ScrapeAsync(ScrapeRequest request, CancellationToken cancellationToken = default);

  /// <summary>
  /// Perform a web scraping operation using the provided URL.
  /// </summary>
  /// <param name="url">The URL to scrape.</param>
  /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
  /// <exception cref="ArgumentNullException">Thrown when the request object is <c>null</c>.</exception>
  /// <exception cref="ScrapiException">Can be thrown when there are problems with the scrape operation.</exception>
  /// <returns>
  /// The scrape content response or an error if scraping failed.
  /// </returns>
  Task<ScrapeResponse?> ScrapeAsync(string url, CancellationToken cancellationToken = default);

  /// <summary>
  /// Perform a web scraping operation using the provided URL.
  /// </summary>
  /// <param name="uri">The URL to scrape.</param>
  /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
  /// <exception cref="ArgumentNullException">Thrown when the request object is <c>null</c>.</exception>
  /// <exception cref="ScrapiException">Can be thrown when there are problems with the scrape operation.</exception>
  /// <returns>
  /// The scrape content response or an error if scraping failed.
  /// </returns>
  Task<ScrapeResponse?> ScrapeAsync(Uri uri, CancellationToken cancellationToken = default);

  /// <summary>
  /// Get a list of supported countries that can be used as the ProxyCountry value in a scrape request.
  /// </summary>
  /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
  /// <exception cref="ScrapiException">Can be thrown when there are problems with fetching the country list.</exception>
  /// <remarks>
  /// The list usually updates every 30 minutes.
  /// </remarks>
  /// <returns>
  /// A list of supported countries.
  /// </returns>
  Task<SupportedCountryResponse?> GetSupportedCountriesAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Get the current credit balance for your API key.
  /// </summary>
  /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
  /// <remarks>
  /// The credit balance could be a negative value as concurrent requests complete.
  /// </remarks>
  /// <returns>
  /// The current credit balance.
  /// </returns>
  Task<int> GetCreditBalanceAsync(CancellationToken cancellationToken = default);
}
