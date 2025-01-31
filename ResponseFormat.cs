using NetEscapades.EnumGenerators;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// Proxy type options to use when requesting a scrape operation.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
[EnumExtensions]
public enum ResponseFormat
{
  /// <summary>
  /// The response will be in the standard JSON format representing a <see cref="ScrapeResponse"/> object.
  /// </summary>
  /// <remarks>
  /// The content type will be application/json.
  /// </remarks>
  Json = 0,

  /// <summary>
  /// The response will be in HTML and other information will be return as headers.
  /// </summary>
  /// <remarks>
  /// The content type will be text/html.
  /// </remarks>
  Html = 1,

  /// <summary>
  /// The response will be in Markdown and other information will be return as headers.
  /// </summary>
  /// <remarks>
  /// The content type will be text/markdown.
  /// </remarks>
  Markdown = 2,
}
