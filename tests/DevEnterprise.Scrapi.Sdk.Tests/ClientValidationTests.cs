using Xunit;

namespace DevEnterprise.Scrapi.Sdk.Tests;

public class ClientValidationTests
{
  [Fact]
  public async Task ScrapeAsync_ValidatesUrlProtocol()
  {
    using var client = new ScrapiClient(string.Empty);
    var request = new ScrapeRequest("https://deventerprise.com") { Url = new Uri("ftp://deventerprise.com", UriKind.Absolute) };

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync(request));

    Assert.Equal("Invalid URL protocol.", ex.Message);
  }

  [Fact]
  public async Task ScrapeAsync_ValidatesProxyCountryLength()
  {
    using var client = new ScrapiClient(string.Empty);
    var request = new ScrapeRequest("https://deventerprise.com") { ProxyCountry = "US" };

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync(request));

    Assert.Equal("Proxy country must be exactly 3 characters long (e.g., 'USA', 'GBR', 'ZAF').", ex.Message);
  }

  [Fact]
  public async Task ScrapeAsync_ValidatesProxyCityRequiresCountry()
  {
    using var client = new ScrapiClient(string.Empty);
    var request = new ScrapeRequest("https://deventerprise.com") { ProxyCity = "NewYork" };

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync(request));

    Assert.Equal("Proxy country must be specified when proxy city is provided.", ex.Message);
  }

  [Theory]
  [InlineData(ProxyType.None)]
  [InlineData(ProxyType.Tor)]
  public async Task ScrapeAsync_ValidatesProxyCountryAndProxyType(ProxyType proxyType)
  {
    using var client = new ScrapiClient(string.Empty);
    var request = new ScrapeRequest("https://deventerprise.com")
    {
      ProxyType = proxyType,
      ProxyCountry = "USA",
    };

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync(request));

    Assert.Equal("Cannot specify a proxy country when not using a proxy (Residential or DataCenter) or when using Tor.", ex.Message);
  }

  [Fact]
  public async Task ScrapeAsync_ValidatesBrowserCommandsRequireBrowser()
  {
    using var client = new ScrapiClient(string.Empty);
    var request = new ScrapeRequest("https://deventerprise.com");
    request.BrowserCommands.Click("#submit");

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync(request));

    Assert.Equal("Cannot use browser commands unless you are using a browser. Set UseBrowser = true.", ex.Message);
  }

  [Fact]
  public async Task ScrapeAsync_ValidatesSolveCaptchasRequireBrowser()
  {
    using var client = new ScrapiClient(string.Empty);
    var request = new ScrapeRequest("https://deventerprise.com") { SolveCaptchas = true };

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync(request));

    Assert.Equal("Cannot solve captchas unless you are using a browser. Set UseBrowser = true.", ex.Message);
  }

  [Fact]
  public async Task ScrapeAsync_ValidatesScreenshotRequiresBrowser()
  {
    using var client = new ScrapiClient(string.Empty);
    var request = new ScrapeRequest("https://deventerprise.com") { IncludeScreenshot = true };

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync(request));

    Assert.Equal("Cannot include a screenshot unless you are using a browser. Set UseBrowser = true.", ex.Message);
  }

  [Fact]
  public async Task ScrapeAsync_ValidatesPdfRequiresBrowser()
  {
    using var client = new ScrapiClient(string.Empty);
    var request = new ScrapeRequest("https://deventerprise.com") { IncludePdf = true };

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync(request));

    Assert.Equal("Cannot include a PDF unless you are using a browser. Set UseBrowser = true.", ex.Message);
  }

  [Fact]
  public async Task ScrapeAsync_ValidatesVideoRequiresBrowser()
  {
    using var client = new ScrapiClient(string.Empty);
    var request = new ScrapeRequest("https://deventerprise.com") { IncludeVideo = true };

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync(request));

    Assert.Equal("Cannot include a video unless you are using a browser. Set UseBrowser = true.", ex.Message);
  }

  [Fact]
  public async Task ScrapeAsync_ValidatesResponseFormat()
  {
    using var client = new ScrapiClient(string.Empty);
    var request = new ScrapeRequest("https://deventerprise.com") { ResponseFormat = ResponseFormat.Html };

    var ex = await Assert.ThrowsAsync<ScrapiException>(() => client.ScrapeAsync(request));

    Assert.Equal("The client only supports the JSON response format.", ex.Message);
  }
}
