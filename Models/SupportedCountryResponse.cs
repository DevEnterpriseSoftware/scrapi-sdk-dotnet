using System.Runtime.Serialization;

namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// Represents a supported country.
/// </summary>
[DataContract]
public sealed record SupportedCountryResponse
{
  /// <summary>
  /// Gets the full name of the country that the key belongs to.
  /// </summary>
  /// <value>
  /// The country name.
  /// </value>
  [DataMember]
  public string Name { get; init; } = default!;

  /// <summary>
  /// Gets the country key that can be used for the  ProxyCountry parameter option.
  /// </summary>
  /// <value>
  /// The country key.
  /// </value>
  [DataMember]
  public string Key { get; init; } = default!;
}
