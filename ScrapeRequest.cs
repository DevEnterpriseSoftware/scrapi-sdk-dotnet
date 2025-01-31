using System.ComponentModel;
using System.Runtime.Serialization;
using DevEnterprise.Scrapi.Sdk.BrowserCommands;
using Newtonsoft.Json;

namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// A web scrape request.
/// </summary>
[Serializable]
[DataContract]
public sealed record ScrapeRequest
{
  private Uri url = default!;

  /// <summary>
  /// Initializes a new instance of the <see cref="ScrapeRequest"/> class.
  /// </summary>
  /// <param name="url">The URL to scrape.</param>
  public ScrapeRequest(string url)
    : this(new Uri(url))
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ScrapeRequest"/> class.
  /// </summary>
  /// <param name="url">The URL to scrape.</param>
  public ScrapeRequest(Uri url)
  {
    Url = url;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ScrapeRequest"/> class.
  /// </summary>
  internal ScrapeRequest()
  {
  }

  /// <summary>
  /// Gets the URL to scrape for content.
  /// </summary>
  /// <value>
  /// The URL to scrape.
  /// </value>
  /// <example>https://deventerprise.com/</example>
  [DataMember]
  public Uri Url
  {
    get => url;
    init
    {
      if (value.IsAbsoluteUri)
      {
        url = value;
      }
      else
      {
        url = new Uri($"https://{value.OriginalString.TrimStart(':', '/')}");
        if (!url.IsAbsoluteUri)
        {
          throw new ArgumentException("The URL provided is not a valid absolute URL.", nameof(Url));
        }
      }
    }
  }

  /// <summary>
  /// Gets or sets the response format from the API call (JSON, HTML or Markdown).
  /// </summary>
  /// The expected response format.
  /// <example>0</example>
  [DataMember(EmitDefaultValue = false)]
  [DefaultValue(typeof(ResponseFormat), "Json")]
  public ResponseFormat ResponseFormat { get; set; } = ScrapeRequestDefaults.ResponseFormat;

  /// <summary>
  /// Gets or sets key/value pair list of cookies to include in the scrape request.
  /// </summary>
  /// <value>
  /// A dictionary container of key/value cookies.
  /// </value>
  [DataMember]
  public IDictionary<string, string> Cookies { get; set; } = new Dictionary<string, string>(ScrapeRequestDefaults.Cookies);

  /// <summary>
  /// Gets or sets key/value pair list of headers to include in the scrape request.
  /// </summary>
  /// <remarks>
  /// ScrAPI will generate certain headers automatically such as a random User Agent.
  /// Headers that are provided will override any headers automatically set by ScrAPI.
  /// </remarks>
  /// <value>
  /// A dictionary container of key/value headers.
  /// </value>
  [DataMember]
  public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(ScrapeRequestDefaults.Headers);

  /// <summary>
  /// Override the HTTP request method to use when making the request to the target website.
  /// </summary>
  /// <remarks>
  /// This option cannot be used when <see cref="UseBrowser"/> is set to <c>true</c>.
  /// </remarks>
  /// <value>
  /// The HTTP request method to use.
  /// </value>
  [DataMember(EmitDefaultValue = false)]
  [DefaultValue("GET")]
  public string RequestMethod { get; set; } = ScrapeRequestDefaults.RequestMethod;

  /// <summary>
  /// Base64 encoded binary data to send to the target website when making the request.
  /// </summary>
  /// <remarks>
  /// This option cannot be used when <see cref="UseBrowser"/> is set to <c>true</c>.
  /// You must provide the Content-Type header as well to indicate the type of data.
  /// </remarks>
  /// <value>
  /// Base64 encoded binary data.
  /// </value>
  [DataMember]
  public string? RequestBodyBase64 { get; set; }

  /// <summary>
  /// Gets or sets the type of the proxy to use for the scrape request.
  /// 0 = None, 1 = Free, 2 = Residential, 3 = DataCenter, 4 = Tor, 5 = Custom
  /// </summary>
  /// <remarks>
  /// The default is <see cref="ProxyType.None"/>.
  /// <seealso cref="Sdk.ProxyType"/>
  /// </remarks>
  /// <value>
  /// The type of the proxy to use.
  /// </value>
  /// <example>3</example>
  [DataMember(EmitDefaultValue = false)]
  [DefaultValue(typeof(ProxyType), "None")]
  public ProxyType ProxyType { get; set; } = ScrapeRequestDefaults.ProxyType;

  /// <summary>
  /// Gets or sets the proxy country to use if you require the scrape request to come from another geographic location.
  /// </summary>
  /// <remarks>
  /// This is useful for geographically locked websites so that the request can come from the correct location.
  /// Use /v1/countries/ to get a list of supported country codes to use.
  /// </remarks>
  /// <value>
  /// The proxy country.
  /// </value>
  /// <example>ZAF</example>
  [DataMember]
  public string? ProxyCountry { get; set; } = ScrapeRequestDefaults.ProxyCountry;

  /// <summary>
  /// Gets or sets the custom proxy URL to use.
  /// </summary>
  /// <remarks>
  /// A custom proxy URL using the format of `protocol://username:password@host:port`.
  /// Username and password is optional if using an unauthenticated proxy.
  /// </remarks>
  /// <value>
  /// The custom proxy URL.
  /// </value>
  /// <example>https://user:password@local.proxy:8080</example>
  [DataMember]
  public string? CustomProxyUrl { get; set; } = ScrapeRequestDefaults.CustomProxyUrl;

  /// <summary>
  /// Gets or sets the indicator of whether you want to use a full headless browser (will execute JavaScript)
  /// or just a regular web client call to retrieve the response (faster but no JavaScript).
  /// </summary>
  /// <remarks>
  /// By default a browser is not used (<c>false</c>).
  /// </remarks>
  /// <value>
  /// <c>true</c> to use a headless browser, <c>false</c> to use a normal HTTP client.
  /// </value>
  /// <example>true</example>
  [DataMember(EmitDefaultValue = false)]
  [DefaultValue(false)]
  public bool UseBrowser { get; set; } = ScrapeRequestDefaults.UseBrowser;

  /// <summary>
  /// Gets or sets the indicator of whether you want to automatically detect, solve and submit any captchas.
  /// </summary>
  /// <remarks>
  /// Solving captchas requires setting <see cref="UseBrowser"/> to <c>true</c>.
  /// Captchas can be largely avoided if using a <see cref="ProxyType"/>.
  /// </remarks>
  /// <value>
  /// <c>true</c> automatically detect, solve and submit and forms protected with captchas.
  /// </value>
  /// <example>false</example>
  [DataMember(EmitDefaultValue = false)]
  [DefaultValue(false)]
  public bool SolveCaptchas { get; set; } = ScrapeRequestDefaults.SolveCaptchas;

  /// <summary>
  /// Gets or sets the option to accept or reject popup dialogs when using browser commands.
  /// </summary>
  /// <remarks>
  /// By default all popup dialogs are cancelled, if that it not the expected behavior you can choose to accept any popup dialogs instead.
  /// </remarks>
  /// <value>
  /// Whether to accept or reject any popup dialogs.
  /// </value>
  [DataMember(EmitDefaultValue = false)]
  [DefaultValue(false)]
  public bool AcceptDialogs { get; set; } = ScrapeRequestDefaults.AcceptDialogs;

  /// <summary>
  /// Gets or sets a session identifier.
  /// </summary>
  /// <remarks>
  /// Using a session identifier will reuse the same contextual information for multiple requests with the same value.
  /// This means the same IP address, user agent and any cookies collected will apply on each request.
  /// Using sessions is useful to avoid having bypass multiple captchas because the request does not change and clearance cookies will remain.
  /// </remarks>
  /// <value>
  /// The session identifier.
  /// </value>
  [DataMember]
  public string? SessionId { get; set; } = ScrapeRequestDefaults.SessionId;

  /// <summary>
  /// Gets or sets the URL that will be called when the scraping operation is complete.
  /// </summary>
  /// <remarks>
  /// An HTTP POST will be performed and the body will contain a <see cref="ScrapeResponse"/> object serialized to JSON.
  /// </remarks>
  /// <value>
  /// The URL to POST response data to when the scraping operation completes.
  /// </value>
  [DataMember]
  public Uri? CallbackUrl { get; set; } = ScrapeRequestDefaults.CallbackUrl;

  /// <summary>
  /// List of browser commands to execute once the web page has loaded.
  /// </summary>
  /// <remarks>
  /// The commands will be executed in order.
  /// You can fluently configure these commands using extension methods defined in <see cref="BrowserCommandExtensions"/>.
  /// </remarks>
  /// <value>
  /// List of browser commands.
  /// </value>
  [DataMember]
  [JsonConverter(typeof(BrowserCommandJsonConverter))]
  public IList<IBrowserCommand> BrowserCommands { get; } = [];
}
