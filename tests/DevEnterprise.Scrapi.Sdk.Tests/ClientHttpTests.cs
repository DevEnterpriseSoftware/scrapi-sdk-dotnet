using System.Net;
using DevEnterprise.Scrapi.Sdk.Tests.Helpers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DevEnterprise.Scrapi.Sdk.Tests;

public class ClientHttpTests
{
  [Fact]
  public async Task ScrapeAsync_MapsSuccessfulResponse()
  {
    string payload = string.Empty;

    using var server = new LocalHttpServer(request =>
    {
      if (request.Path == "/v1/scrape")
      {
        payload = request.Body;

        return Task.FromResult(
          new LocalHttpServer.StubResponse(
            (int)HttpStatusCode.OK,
            """
            {
              "RequestUrl": "https://deventerprise.com",
              "ResponseUrl": "https://deventerprise.com/",
              "Duration": "00:00:01.000",
              "Attempts": 1,
              "CaptchasSolved": { "hcaptcha": 1 },
              "CreditsUsed": 2,
              "StatusCode": 200,
              "Headers": { "content-type": "text/html" },
              "Cookies": { "a": "b" },
              "Content": "<html>Hello</html>"
            }
            """));
      }

      return Task.FromResult(new LocalHttpServer.StubResponse((int)HttpStatusCode.NotFound));
    });

    using var client = new ScrapiClient("api-key");
    ConfigureClient(client, server.BaseAddress);

    var response = await client.ScrapeAsync(new ScrapeRequest("https://deventerprise.com"));

    Assert.NotNull(response);
    Assert.Equal("https://deventerprise.com/", response!.RequestUrl.ToString());
    Assert.Equal(2, response.CreditsUsed);

    var requestPayload = JObject.Parse(payload);
    Assert.Equal("https://deventerprise.com", requestPayload["Url"]?.ToString());
  }

  [Fact]
  public async Task LookupMethods_MapSuccessfulResponses()
  {
    using var server = new LocalHttpServer(request =>
    {
      return Task.FromResult(request.Path switch
      {
        "/v1/countries" => new LocalHttpServer.StubResponse(
          (int)HttpStatusCode.OK,
          """
          [
            { "Name": "United States", "Key": "USA", "ProxyCount": 10 }
          ]
          """),
        "/v1/countries/USA/cities" => new LocalHttpServer.StubResponse(
          (int)HttpStatusCode.OK,
          """
          [
            { "Name": "New York", "Key": "NewYork", "ProxyCount": 5 }
          ]
          """),
        "/v1/balance" => new LocalHttpServer.StubResponse(
          (int)HttpStatusCode.OK,
          """
          { "Credits": 123 }
          """),
        _ => new LocalHttpServer.StubResponse((int)HttpStatusCode.NotFound),
      });
    });

    using var client = new ScrapiClient("api-key");
    ConfigureClient(client, server.BaseAddress);

    var countries = (await client.GetSupportedCountriesAsync()).ToList();
    Assert.Single(countries);
    Assert.Equal("USA", countries[0].Key);

    var cities = (await client.GetSupportedCitiesAsync("USA")).ToList();
    Assert.Single(cities);
    Assert.Equal("NewYork", cities[0].Key);

    var balance = await client.GetCreditBalanceAsync();
    Assert.Equal(123, balance);
  }

  [Fact]
  public async Task NotFoundResponses_ReturnFallbacks()
  {
    using var server = new LocalHttpServer(_ =>
      Task.FromResult(new LocalHttpServer.StubResponse((int)HttpStatusCode.NotFound)));

    using var client = new ScrapiClient("api-key");
    ConfigureClient(client, server.BaseAddress);

    Assert.Null(await client.ScrapeAsync("https://deventerprise.com"));
    Assert.Empty(await client.GetSupportedCountriesAsync());
    Assert.Empty(await client.GetSupportedCitiesAsync("USA"));
    Assert.Equal(0, await client.GetCreditBalanceAsync());
  }

  [Fact]
  public async Task InvalidJson_ThrowsScrapiException()
  {
    using var server = new LocalHttpServer(_ =>
      Task.FromResult(new LocalHttpServer.StubResponse((int)HttpStatusCode.OK, "not-json")));

    using var client = new ScrapiClient("api-key");
    ConfigureClient(client, server.BaseAddress);

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync("https://deventerprise.com"));

    Assert.Equal("Invalid JSON response.", ex.Message);
  }

  [Fact]
  public async Task HttpError_ThrowsScrapiExceptionWithStatusCode()
  {
    using var server = new LocalHttpServer(_ =>
      Task.FromResult(new LocalHttpServer.StubResponse((int)HttpStatusCode.InternalServerError, "{}")));

    using var client = new ScrapiClient("api-key");
    ConfigureClient(client, server.BaseAddress);

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync("https://deventerprise.com"));

    Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
  }

  [Fact]
  public async Task Timeout_ThrowsRequestTimeout()
  {
    using var server = new LocalHttpServer(_ =>
      Task.FromResult(new LocalHttpServer.StubResponse(
        (int)HttpStatusCode.OK,
        "{}",
        DelayMilliseconds: 2000)));

    using var client = new ScrapiClient("api-key");
    ConfigureClient(client, server.BaseAddress, TimeSpan.FromMilliseconds(200));

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync("https://deventerprise.com"));

    Assert.Equal(HttpStatusCode.RequestTimeout, ex.StatusCode);
  }

  private static void ConfigureClient(ScrapiClient client, Uri baseAddress, TimeSpan? timeout = null)
  {
    var httpClient = ScrapiClientTestReflection.GetHttpClient(client);
    httpClient.BaseAddress = baseAddress;

    if (timeout is not null)
    {
      httpClient.Timeout = timeout.Value;
    }
  }
}
