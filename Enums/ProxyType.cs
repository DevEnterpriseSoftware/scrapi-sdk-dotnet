using NetEscapades.EnumGenerators;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// Proxy type options to use when requesting a scrape operation.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
[EnumExtensions]
public enum ProxyType
{
  /// <summary>
  /// No proxy will be used (default).
  /// </summary>
  /// <remarks>
  /// This is the fastest option but can easily be blocked by sites that throttle traffic.
  /// </remarks>
  None = 0,

  /// <summary>
  /// Rotating public/free proxy will be used.
  /// </summary>
  /// <remarks>
  /// This option uses free anonymous proxies available on the internet.
  /// These are generally slow and unreliable and should not be used for production workloads.
  /// </remarks>
  Free = 1,

  /// <summary>
  /// Rotating residential proxy will be used.
  /// </summary>
  /// <remarks>
  /// This option is slower but more reliable for sites that actively try to block scraping activity.
  /// </remarks>
  Residential = 2,

  /// <summary>
  /// Rotating data center proxy will be used.
  /// </summary>
  /// <remarks>
  /// This option is faster but can be less reliable on sites that  try to block scraping activity.
  /// </remarks>
  DataCenter = 3,

  /// <summary>
  /// A Tor proxy will be used.
  /// </summary>
  /// <remarks>
  /// This option is ideal for scraping Onion websites.
  /// </remarks>
  Tor = 4,

  /// <summary>
  /// A Tor proxy will be used.
  /// </summary>
  /// <remarks>
  /// This option is ideal for scraping Onion websites.
  /// </remarks>
  Custom = 5,
}
