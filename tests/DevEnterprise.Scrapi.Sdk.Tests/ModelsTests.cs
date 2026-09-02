using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DevEnterprise.Scrapi.Sdk.Tests;

public class ModelsTests
{
  [Fact]
  public void ScrapeRequest_CopiesMutableDefaults()
  {
    var originalCookies = new Dictionary<string, string>(ScrapeRequestDefaults.Cookies);
    var originalHeaders = new Dictionary<string, string>(ScrapeRequestDefaults.Headers);

    try
    {
      ScrapeRequestDefaults.Cookies = new Dictionary<string, string> { ["cookie1"] = "value1" };
      ScrapeRequestDefaults.Headers = new Dictionary<string, string> { ["header1"] = "value1" };

      var request = new ScrapeRequest("https://deventerprise.com");

      Assert.Equal("value1", request.Cookies["cookie1"]);
      Assert.Equal("value1", request.Headers["header1"]);

      request.Cookies["cookie2"] = "value2";
      request.Headers["header2"] = "value2";

      Assert.False(ScrapeRequestDefaults.Cookies.ContainsKey("cookie2"));
      Assert.False(ScrapeRequestDefaults.Headers.ContainsKey("header2"));
    }
    finally
    {
      ScrapeRequestDefaults.Cookies = originalCookies;
      ScrapeRequestDefaults.Headers = originalHeaders;
    }
  }

  [Fact]
  public void ScrapeRequest_NormalizesRelativeUrl()
  {
    var request = new ScrapeRequest(new Uri("deventerprise.com", UriKind.RelativeOrAbsolute));

    Assert.Equal("https://deventerprise.com/", request.Url.ToString());
  }

  [Fact]
  public void ScrapeResponse_ContentHash_MatchesUtf16LeSha1()
  {
    var response = new ScrapeResponse
    {
      RequestUrl = new Uri("https://deventerprise.com"),
      Content = "Hello",
    };

    var bytes = Encoding.Unicode.GetBytes("Hello");
    var expected = Convert.ToHexString(SHA1.HashData(bytes));

    Assert.Equal(expected, response.ContentHash);
  }

  [Fact]
  public void ScrapeResponse_Html_IsLazyAndRefreshesAfterContentChange()
  {
    var response = new ScrapeResponse
    {
      RequestUrl = new Uri("https://deventerprise.com"),
      Content = "<html><body><p>A &amp; B</p></body></html>",
    };

    var html1 = response.Html;
    Assert.NotNull(html1);
    Assert.Equal("A & B", html1!.SelectSingleNode("//p")!.InnerText);

    response.Content = "<html><body><p>Changed</p></body></html>";

    var html2 = response.Html;
    Assert.NotNull(html2);
    Assert.NotSame(html1, html2);
    Assert.Equal("Changed", html2!.SelectSingleNode("//p")!.InnerText);
  }

  [Fact]
  public void ScrapeRequest_Serialize_IncludesGloballyConfiguredDefaults()
  {
    var originalCookies = new Dictionary<string, string>(ScrapeRequestDefaults.Cookies);
    var originalHeaders = new Dictionary<string, string>(ScrapeRequestDefaults.Headers);
    var originalResponseSelector = ScrapeRequestDefaults.ResponseSelector;
    var originalProxyCountry = ScrapeRequestDefaults.ProxyCountry;
    var originalProxyType = ScrapeRequestDefaults.ProxyType;
    var originalCustomProxyUrl = ScrapeRequestDefaults.CustomProxyUrl;
    var originalUseBrowser = ScrapeRequestDefaults.UseBrowser;
    var originalSessionId = ScrapeRequestDefaults.SessionId;
    var originalCallbackUrl = ScrapeRequestDefaults.CallbackUrl;

    try
    {
      // Simulate an application-wide default configured once at startup, not per-request.
      ScrapeRequestDefaults.Cookies = new Dictionary<string, string> { ["cookie1"] = "value1" };
      ScrapeRequestDefaults.Headers = new Dictionary<string, string> { ["header1"] = "value1" };
      ScrapeRequestDefaults.ResponseSelector = ".article";
      ScrapeRequestDefaults.ProxyCountry = "ZAF";
      ScrapeRequestDefaults.ProxyType = ProxyType.Residential;
      ScrapeRequestDefaults.CustomProxyUrl = "https://user:pass@local.proxy:8080";
      ScrapeRequestDefaults.UseBrowser = true;
      ScrapeRequestDefaults.SessionId = "default-session";
      ScrapeRequestDefaults.CallbackUrl = new Uri("https://deventerprise.com/callback");

      // Nothing set explicitly on the request itself — every non-default value below comes from ScrapeRequestDefaults.
      var request = new ScrapeRequest("https://deventerprise.com");

      var json = JObject.Parse(JsonConvert.SerializeObject(request));

      Assert.Equal("value1", json["Cookies"]!["cookie1"]!.Value<string>());
      Assert.Equal("value1", json["Headers"]!["header1"]!.Value<string>());
      Assert.Equal(".article", json["ResponseSelector"]!.Value<string>());
      Assert.Equal("ZAF", json["ProxyCountry"]!.Value<string>());
      Assert.Equal("Residential", json["ProxyType"]!.Value<string>());
      Assert.Equal("https://user:pass@local.proxy:8080", json["CustomProxyUrl"]!.Value<string>());
      Assert.True(json["UseBrowser"]!.Value<bool>());
      Assert.Equal("default-session", json["SessionId"]!.Value<string>());
      Assert.Equal("https://deventerprise.com/callback", json["CallbackUrl"]!.Value<string>());
    }
    finally
    {
      ScrapeRequestDefaults.Cookies = originalCookies;
      ScrapeRequestDefaults.Headers = originalHeaders;
      ScrapeRequestDefaults.ResponseSelector = originalResponseSelector;
      ScrapeRequestDefaults.ProxyCountry = originalProxyCountry;
      ScrapeRequestDefaults.ProxyType = originalProxyType;
      ScrapeRequestDefaults.CustomProxyUrl = originalCustomProxyUrl;
      ScrapeRequestDefaults.UseBrowser = originalUseBrowser;
      ScrapeRequestDefaults.SessionId = originalSessionId;
      ScrapeRequestDefaults.CallbackUrl = originalCallbackUrl;
    }
  }

  [Fact]
  public void ScrapeRequest_Serialize_OmitsEmptyDefaults()
  {
    var request = new ScrapeRequest("https://deventerprise.com");

    var json = JObject.Parse(JsonConvert.SerializeObject(request));

    Assert.False(json.ContainsKey("Cookies"));
    Assert.False(json.ContainsKey("Headers"));
    Assert.False(json.ContainsKey("BrowserCommands"));
    Assert.False(json.ContainsKey("RequestBodyBase64"));
    Assert.False(json.ContainsKey("CustomProxyUrl"));
    Assert.False(json.ContainsKey("SessionId"));
    Assert.False(json.ContainsKey("CallbackUrl"));
  }

  [Fact]
  public void ScrapeRequest_Serialize_IncludesPopulatedValues()
  {
    var request = new ScrapeRequest("https://deventerprise.com")
    {
      RequestBodyBase64 = "abc123",
      CustomProxyUrl = "https://user:pass@local.proxy:8080",
      SessionId = "session-1",
      CallbackUrl = new Uri("https://deventerprise.com/callback"),
    };
    request.Cookies["cookie1"] = "value1";
    request.Headers["header1"] = "value1";
    request.BrowserCommands.Add(new BrowserCommands.ClickCommand { TargetSelector = "#button" });

    var json = JObject.Parse(JsonConvert.SerializeObject(request));

    Assert.Equal("value1", json["Cookies"]!["cookie1"]!.Value<string>());
    Assert.Equal("value1", json["Headers"]!["header1"]!.Value<string>());
    Assert.Single(json["BrowserCommands"]!);
    Assert.Equal("abc123", json["RequestBodyBase64"]!.Value<string>());
    Assert.Equal("https://user:pass@local.proxy:8080", json["CustomProxyUrl"]!.Value<string>());
    Assert.Equal("session-1", json["SessionId"]!.Value<string>());
    Assert.Equal("https://deventerprise.com/callback", json["CallbackUrl"]!.Value<string>());
  }

  [Fact]
  public void ScrapeRequest_Deserialize_MissingPropertiesGetNonNullDefaults()
  {
    var request = JsonConvert.DeserializeObject<ScrapeRequest>("""{"Url":"https://deventerprise.com"}""");

    Assert.NotNull(request);
    Assert.NotNull(request!.Cookies);
    Assert.Empty(request.Cookies);
    Assert.NotNull(request.Headers);
    Assert.Empty(request.Headers);
    Assert.NotNull(request.BrowserCommands);
    Assert.Empty(request.BrowserCommands);
  }

  [Fact]
  public void ScrapeResponse_Serialize_OmitsEmptyDefaults()
  {
    var response = new ScrapeResponse { RequestUrl = new Uri("https://deventerprise.com") };

    var json = JObject.Parse(JsonConvert.SerializeObject(response));

    Assert.False(json.ContainsKey("CaptchasSolved"));
    Assert.False(json.ContainsKey("Cookies"));
    Assert.False(json.ContainsKey("Headers"));
    Assert.False(json.ContainsKey("ResponseUrl"));
    Assert.False(json.ContainsKey("ScreenshotUrl"));
    Assert.False(json.ContainsKey("PdfUrl"));
    Assert.False(json.ContainsKey("VideoUrl"));
    Assert.False(json.ContainsKey("Content"));
  }

  [Fact]
  public void ScrapeResponse_Serialize_IncludesPopulatedValues()
  {
    var response = new ScrapeResponse { RequestUrl = new Uri("https://deventerprise.com") };
    response.CaptchasSolved["recaptcha"] = 1;
    response.Cookies["cookie1"] = "value1";
    response.Headers["header1"] = "value1";

    var json = JObject.Parse(JsonConvert.SerializeObject(response));

    Assert.Equal(1, json["CaptchasSolved"]!["recaptcha"]!.Value<short>());
    Assert.Equal("value1", json["Cookies"]!["cookie1"]!.Value<string>());
    Assert.Equal("value1", json["Headers"]!["header1"]!.Value<string>());
  }

  [Fact]
  public void ScrapeResponse_Deserialize_MissingPropertiesGetNonNullDefaults()
  {
    var response = JsonConvert.DeserializeObject<ScrapeResponse>("""{"RequestUrl":"https://deventerprise.com"}""");

    Assert.NotNull(response);
    Assert.NotNull(response!.CaptchasSolved);
    Assert.Empty(response.CaptchasSolved);
    Assert.NotNull(response.Cookies);
    Assert.Empty(response.Cookies);
    Assert.NotNull(response.Headers);
    Assert.Empty(response.Headers);
  }
}
