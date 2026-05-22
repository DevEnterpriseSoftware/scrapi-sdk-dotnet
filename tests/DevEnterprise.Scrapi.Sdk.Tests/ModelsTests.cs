using System.Security.Cryptography;
using System.Text;
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
}
