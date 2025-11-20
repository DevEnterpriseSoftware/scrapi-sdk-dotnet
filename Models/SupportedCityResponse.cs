using System.Runtime.Serialization;

namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// Represents a supported city.
/// </summary>
[DataContract]
public sealed record SupportedCityResponse
{
  /// <summary>
  /// Gets the full name of the City that the key belongs to.
  /// </summary>
  /// <value>
  /// The city name.
  /// </value>
  [DataMember]
  public string Name { get; init; } = default!;

  /// <summary>
  /// Gets the city key that can be used for the ProxyCity parameter option.
  /// </summary>
  /// <value>
  /// The city key.
  /// </value>
  [DataMember]
  public string Key { get; init; } = default!;

  /// <summary>
  /// The number of proxies available in this city.
  /// </summary>
  /// <value>
  /// The proxy count.
  /// </value>
  [DataMember]
  public int ProxyCount { get; init; }
}
