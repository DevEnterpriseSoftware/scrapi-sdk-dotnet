using System.Runtime.Serialization;

namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// Represents your credit balance.
/// </summary>
[DataContract]
public sealed record BalanceResponse
{
  /// <summary>
  /// Gets the number of credits available for the API key.
  /// </summary>
  /// <value>
  /// The credit balance.
  /// </value>
  [DataMember]
  public int Credits { get; init; } = default!;
}
