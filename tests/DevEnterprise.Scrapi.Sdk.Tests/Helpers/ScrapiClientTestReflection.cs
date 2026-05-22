using System.Reflection;

namespace DevEnterprise.Scrapi.Sdk.Tests.Helpers;

internal static class ScrapiClientTestReflection
{
  private static readonly FieldInfo HttpClientField =
    typeof(ScrapiClient).GetField("httpClient", BindingFlags.Instance | BindingFlags.NonPublic) ??
    throw new InvalidOperationException("Could not locate ScrapiClient.httpClient field.");

  public static HttpClient GetHttpClient(ScrapiClient client)
  {
    return (HttpClient)(HttpClientField.GetValue(client) ?? throw new InvalidOperationException("HttpClient field was null."));
  }
}
