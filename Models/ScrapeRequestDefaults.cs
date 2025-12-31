namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// Static settings that can be applied to all new <see cref="ScrapeRequest"/> objects.
/// </summary>
public static class ScrapeRequestDefaults
{
  /// <summary>
  /// Gets or sets the default response format from the API call (JSON or HTML).
  /// </summary>
  /// <value>
  /// The expected response format.
  /// </value>
  public static ResponseFormat ResponseFormat { get; set; } = ResponseFormat.Json;

  /// <summary>
  /// Gets or sets the default response selector (CSS or XPath) to use when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <value>
  /// The default response selector (CSS or XPath).
  /// </value>
  public static string? ResponseSelector { get; set; }

  /// <summary>
  /// Gets or sets the default key/value pair list of cookies to include when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <value>
  /// A dictionary container of key/value cookies.
  /// </value>
  public static IDictionary<string, string> Cookies { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

  /// <summary>
  /// Gets or sets the default key/value pair list of headers to include when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <value>
  /// A dictionary container of key/value headers.
  /// </value>
  public static IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

  /// <summary>
  /// Gets or sets the default HTTP request method when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <value>
  /// The HTTP request method to use.
  /// </value>
  public static string RequestMethod { get; set; } = "GET";

  /// <summary>
  /// Gets or sets the default type of the proxy to use when instantiating a new <see cref="ScrapeRequest"/>.
  /// 0 = None, 1 = Free, 2= Residential, 3 = DataCenter, 4 = Tor, 5 = Custom
  /// </summary>
  /// <remarks>
  /// The default is <see cref="ProxyType.None"/>.
  /// <seealso cref="Sdk.ProxyType"/>
  /// </remarks>
  /// <value>
  /// The type of the proxy to use.
  /// </value>
  public static ProxyType ProxyType { get; set; } = ProxyType.None;

  /// <summary>
  /// Gets or sets the default proxy country to use when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <remarks>
  /// This is useful for geographically locked websites so that the request can come from the correct location.
  /// Use /v1/countries/ to get a list of supported country codes to use.
  /// </remarks>
  /// <value>
  /// The proxy country.
  /// </value>
  public static string? ProxyCountry { get; set; }

  /// <summary>
  /// Gets or sets the default proxy city to use when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <remarks>
  /// This is useful for geographically locked websites so that the request can come from the correct location.
  /// Use /v1/countries/{countryKey}/cities/ to get a list of supported cities names to use.
  /// </remarks>
  /// <value>
  /// The proxy city.
  /// </value>
  public static string? ProxyCity { get; set; }

  /// <summary>
  /// Gets or sets the default custom proxy to use when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <remarks>
  /// A custom proxy URL using the format of `protocol://username:password@host:port`.
  /// Username and password is optional if using an unauthenticated proxy.
  /// </remarks>
  /// <value>
  /// The custom proxy URL.
  /// </value>
  public static string? CustomProxyUrl { get; set; }

  /// <summary>
  /// Gets or sets the default indicator of whether you want to use a full headless browser when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <remarks>
  /// By default a browser is not used (<c>false</c>) and JavaScript will not be executed.
  /// </remarks>
  /// <value>
  /// <c>true</c> to use a headless browser, <c>false</c> to use a normal HTTP client.
  /// </value>
  public static bool UseBrowser { get; set; }

  /// <summary>
  /// Gets or sets the default indicator of whether you want to automatically detect, solve and submit any captchas.
  /// </summary>
  /// <remarks>
  /// Solving captchas requires setting <see cref="UseBrowser"/> to <c>true</c>.
  /// Captchas can be largely avoided if using a <see cref="ProxyType"/>.
  /// </remarks>
  /// <value>
  /// <c>true</c> automatically detect, solve and submit and forms protected with captchas.
  /// </value>
  public static bool SolveCaptchas { get; set; }

  /// <summary>
  /// Gets or sets the default indicator of whether a screenshot of the page should be included in the response.
  /// </summary>
  /// <remarks>
  /// Taking screenshots requires setting <see cref="UseBrowser"/> to <c>true</c>.
  /// A link to the screenshot image will be included in the <see cref="ScrapeResponse"/> object.
  /// </remarks>
  /// <value>
  /// <c>true</c> to include a screenshot, <c>false</c> to exclude it.
  /// </value>
  public static bool IncludeScreenshot { get; set; }

  /// <summary>
  /// Gets or sets the default indicator of whether a PDF should be included in the response.
  /// </summary>
  /// <remarks>
  /// Taking screenshots requires setting <see cref="UseBrowser"/> to <c>true</c>.
  /// A link to the PDF file will be included in the <see cref="ScrapeResponse"/> object.
  /// </remarks>
  /// <value>
  /// <c>true</c> to include a PDF, <c>false</c> to exclude it.
  /// </value>
  public static bool IncludePdf { get; set; }

  /// <summary>
  /// Gets or sets the default indicator of whether a video should be included in the response.
  /// </summary>
  /// <remarks>
  /// Taking screenshots requires setting <see cref="UseBrowser"/> to <c>true</c>.
  /// A link to the video recording file will be included in the <see cref="ScrapeResponse"/> object.
  /// </remarks>
  /// <value>
  /// <c>true</c> to include a video recording, <c>false</c> to exclude it.
  /// </value>
  public static bool IncludeVideo { get; set; }

  /// <summary>
  /// Gets or sets the default option to accept or reject popup dialogs when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <remarks>
  /// By default all popup dialogs are cancelled, if that it not the expected behavior you can choose to accept any popup dialogs instead.
  /// </remarks>
  /// <value>
  /// Whether to accept or reject any popup dialogs.
  /// </value>
  public static bool AcceptDialogs { get; set; }

  /// <summary>
  /// Gets or sets a default session identifier when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <remarks>
  /// Using a session identifier will reuse the same contextual information for multiple requests with the same value.
  /// This means the same IP address, user agent and any cookies collected will apply on each request.
  /// Using sessions is useful to avoid having bypass multiple captchas because the request does not change and clearance cookies will remain.
  /// </remarks>
  /// <value>
  /// The session identifier.
  /// </value>
  public static string? SessionId { get; set; }

  /// <summary>
  /// Gets or sets the default URL to use when instantiating a new <see cref="ScrapeRequest"/>.
  /// </summary>
  /// <remarks>
  /// An HTTP POST will be performed and the body will contain a <see cref="ScrapeResponse"/> object serialized to JSON.
  /// </remarks>
  /// <value>
  /// The URL to POST response data to when the scraping operation completes.
  /// </value>
  public static Uri? CallbackUrl { get; set; }
}
