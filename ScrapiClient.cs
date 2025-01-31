using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace DevEnterprise.Scrapi.Sdk;

/// <inheritdoc/>
public sealed class ScrapiClient : IScrapiClient
{
#pragma warning disable S1075 // URIs should not be hardcoded
  /// <summary>
  /// The API URL.
  /// </summary>
  public static readonly string ApiUrl = "https://api.scrapi.tech";

  /// <summary>
  /// The API healtch check URL.
  /// </summary>
  public static readonly string ApiHealthUrl = "https://api.scrapi.tech/health";
#pragma warning restore S1075 // URIs should not be hardcoded

  private static readonly Encoding StreamEncoding = new UTF8Encoding(false);

  private readonly HttpClient httpClient;

  /// <summary>
  /// Initializes a new instance of the <see cref="ScrapiClient"/> class.
  /// </summary>
  /// <param name="apiKey">The API key that was issued to you when registering on <![CDATA[<a href="https://scrapi.tech">https://scrapi.tech</a>]]></param>
  public ScrapiClient(string apiKey)
  {
    var clientHandler = new HttpClientHandler
    {
#if NETSTANDARD2_0 || NETSTANDARD2_1
      AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
#else
      AutomaticDecompression = DecompressionMethods.All,
#endif
    };

    httpClient = new HttpClient(clientHandler, true)
    {
      BaseAddress = new(ApiUrl),
      Timeout = TimeSpan.FromSeconds(300),
    };

#if NETSTANDARD2_0 || NETSTANDARD2_1
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip,deflate");
#else
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip,deflate,br");
#endif

    httpClient.DefaultRequestHeaders.Remove("User-Agent");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ScrAPI .NET SDK - " + typeof(ScrapiClient).Assembly.GetName().Version);
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-API-KEY", apiKey);
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    httpClient.Dispose();
  }

  /// <inheritdoc/>
  public Task<ScrapeResponse?> ScrapeAsync(string url, CancellationToken cancellationToken = default)
    => ScrapeAsync(new ScrapeRequest(url), cancellationToken);

  /// <inheritdoc/>
  public Task<ScrapeResponse?> ScrapeAsync(Uri uri, CancellationToken cancellationToken = default)
    => ScrapeAsync(new ScrapeRequest(uri), cancellationToken);

  /// <inheritdoc/>
  public Task<ScrapeResponse?> ScrapeAsync(ScrapeRequest request, CancellationToken cancellationToken = default)
  {
    if (request is null)
    {
      throw new ArgumentNullException(nameof(request));
    }

    var requestUrl = request.Url.ToString();
    if (string.IsNullOrEmpty(requestUrl))
    {
      throw new ScrapiException("URL cannot be null/blank.");
    }

    if (!requestUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
    {
      throw new ScrapiException("Invalid URL protocol.");
    }

    if (request.ProxyCountry?.Length == 3 &&
       (request.ProxyType == ProxyType.None || request.ProxyType == ProxyType.Tor))
    {
      throw new ScrapiException("Cannot specify a proxy country when not using proxy (Residential or DataCenter) or when using Tor.");
    }

    if (request.BrowserCommands.Count > 0 && !request.UseBrowser)
    {
      throw new ScrapiException("Cannot use browser commands unless you are using a browser. Set UseBrowser = true.");
    }

    if (request.SolveCaptchas && !request.UseBrowser)
    {
      throw new ScrapiException("Cannot solve captchas unless you are using a browser. Set UseBrowser = true.");
    }

    if (request.ResponseFormat != ResponseFormat.Json)
    {
      throw new ScrapiException("The client only supports the JSON response format.");
    }

    return MakeApiCallAsync<ScrapeRequest, ScrapeResponse>(HttpMethod.Post, "v1/scrape", request, cancellationToken);
  }

  /// <inheritdoc/>
  public Task<SupportedCountryResponse?> GetSupportedCountriesAsync(CancellationToken cancellationToken = default)
    => MakeApiCallAsync<object, SupportedCountryResponse>(HttpMethod.Get, "v1/countries", null, cancellationToken);

  /// <inheritdoc/>
  public async Task<int> GetCreditBalanceAsync(CancellationToken cancellationToken = default)
  {
    var result = await MakeApiCallAsync<object, BalanceResponse>(HttpMethod.Get, "v1/balance", null, cancellationToken);

    return result?.Credits ?? 0;
  }

  private async Task<TResponse?> MakeApiCallAsync<TRequest, TResponse>(HttpMethod method, string url, TRequest? data, CancellationToken cancellationToken)
  {
    //// var debugResponse = await httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"), cancellationToken);
    //// var debugJson = await debugResponse.Content.ReadAsStringAsync(cancellationToken);
    //// var debugResult = JsonConvert.DeserializeObject<ScrapiResponse<TResponse>>(debugJson);
    //// return debugResult?.Result;

    using var request = new HttpRequestMessage(method, url);

    // Enable HTTP/3 when the server can be upgraded to use it.
    //// #if !NETSTANDARD
    ////     request.Version = HttpVersion.Version30;
    ////     request.VersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
    //// #endif

    HttpResponseMessage? response = null;

    try
    {
      if (data is not null)
      {
        using var ms = new MemoryStream();
        using var sw = new StreamWriter(ms, StreamEncoding, 1024, true);
        using var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None };
        var js = new JsonSerializer();
        js.Serialize(jtw, data);
        await jtw.FlushAsync();

        ms.Seek(0, SeekOrigin.Begin);
        var httpContent = new StreamContent(ms);
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        request.Content = httpContent;

        response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
      }
      else
      {
        response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
      }

      var stream = await response.Content.ReadAsStreamAsync();
      if (stream?.CanRead != true)
      {
        return default;
      }

      using var sr = new StreamReader(stream);
      using var jtr = new JsonTextReader(sr);
      var jr = new JsonSerializer();

      if (!response.IsSuccessStatusCode)
      {
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
          return default;
        }

        response.EnsureSuccessStatusCode();
      }

      var result = jr.Deserialize<TResponse>(jtr);

      return result is null ? default : result;
    }
    catch (TaskCanceledException ex)
    {
      throw new ScrapiException(HttpStatusCode.RequestTimeout, ex.Message, ex);
    }
    catch (HttpRequestException ex)
    {
      throw new ScrapiException(response?.StatusCode, ex.Message, ex);
    }
    catch (JsonReaderException ex)
    {
      throw new ScrapiException(response?.StatusCode, "Invalid JSON response.", ex);
    }
  }
}
