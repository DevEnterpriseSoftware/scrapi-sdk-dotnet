using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using HtmlAgilityPack;

namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// A web scrape response.
/// </summary>
[Serializable]
[DataContract]
public sealed record ScrapeResponse
{
  private string? content;
  private HtmlNode? html;

  /// <summary>
  /// The parsed HTML document node that can be used to query data from the content.
  /// </summary>
  /// <value>
  /// The HTML document node.
  /// </value>
  public HtmlNode? Html
  {
    get
    {
      if (html is null && !string.IsNullOrEmpty(content))
      {
        var doc = new HtmlDocument();
        doc.LoadHtml(WebUtility.HtmlDecode(content));
        html = doc.DocumentNode;
      }

      return html;
    }
  }

  /// <summary>
  /// The URL that the requested.
  /// </summary>
  /// <value>
  /// The original URL.
  /// </value>
  /// <example>https://deventerprise.com/</example>
  [DataMember]
  public required Uri RequestUrl { get; init; }

  /// <summary>
  /// The URL that the scraped content comes from.
  /// </summary>
  /// <value>
  /// The final URL.
  /// </value>
  /// <example>https://deventerprise.com/</example>
  [DataMember]
  public Uri? ResponseUrl { get; set; }

  /// <summary>
  /// Gets the duration of the scrape operation.
  /// </summary>
  /// <value>
  /// The amount of time the scrape operation took.
  /// </value>
  /// <example>00:00:15.0000</example>
  [DataMember]
  public TimeSpan Duration { get; set; }

  /// <summary>
  /// The number of attempts made to scrape the page (retries) before responding.
  /// </summary>
  /// <value>
  /// The number of attempts to scrape the page.
  /// </value>
  /// <example>2</example>
  [DataMember]
  public int Attempts { get; set; }

  /// <summary>
  /// The type and number of captchas solved.
  /// </summary>
  /// <remarks>
  /// Some pages use multiple captchas often of the same type.
  /// </remarks>
  /// <value>
  /// The type of captcha that was detected and how many times it was solved successfully.
  /// </value>
  [DataMember]
  public IDictionary<string, short> CaptchasSolved { get; } = new Dictionary<string, short>();

  /// <summary>
  /// The number of credits used for this scrape operation.
  /// </summary>
  /// <value>
  /// The credits used.
  /// </value>
  /// <example>3</example>
  [DataMember]
  public int CreditsUsed { get; set; }

  /// <summary>
  /// Any error message that may have been reported as a result of failing to scrape the content.
  /// </summary>
  /// <value>
  /// A message describing the error.
  /// </value>
  /// <example>Only if an error has occurred. Message will contain details.</example>
  [DataMember(EmitDefaultValue = false)]
  public IEnumerable<string>? ErrorMessages { get; set; }

  /// <summary>
  /// The final HTTP status code of the scrape operation.
  /// </summary>
  /// <value>
  /// The HTTP status code.
  /// </value>
  [DataMember(EmitDefaultValue = false)]
  public int StatusCode { get; set; }

  /// <summary>
  /// Key/value pairs of the cookie data that resulted from the request.
  /// </summary>
  /// <remarks>
  /// You can reuse this in subsequent requests to mimic a "session" on the target site.
  /// </remarks>
  /// <value>
  /// The cookies returned in the response.
  /// </value>
  [DataMember]
  public IDictionary<string, string> Cookies { get; } = new Dictionary<string, string>();

  /// <summary>
  /// Key/value pairs of the header data that resulted from the request.
  /// </summary>
  /// <remarks>
  /// You can reuse this in subsequent requests.
  /// </remarks>
  /// <value>
  /// The headers returned in the response.
  /// </value>
  [DataMember]
  public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

  /// <summary>
  /// The HTML/JSON content of the URL that was scraped.
  /// </summary>
  /// <value>
  /// The HTML/JSON content.
  /// </value>
  [DataMember]
  public string? Content
  {
    get
    {
      return content;
    }

    set
    {
      content = value;
      html = null;
    }
  }

  /// <summary>
  /// A SHA1 hash of the HTML content which can be useful to compare against previous responses.
  /// </summary>
  /// <value>
  /// A SHA1 hash of the HTML content.
  /// </value>
  public string ContentHash
  {
    get
    {
      if (!string.IsNullOrEmpty(content))
      {
        var bytes = new byte[content!.Length * sizeof(char)];
        Buffer.BlockCopy(content.ToCharArray(), 0, bytes, 0, bytes.Length);

        using var sha1 = SHA1.Create();
        var encoded = sha1.ComputeHash(bytes);

        var sb = new StringBuilder();
        for (int i = 0; i < encoded.Length; i++)
        {
          sb.Append(encoded[i].ToString("X2"));
        }

        return sb.ToString();
      }

      return string.Empty;
    }
  }
}
