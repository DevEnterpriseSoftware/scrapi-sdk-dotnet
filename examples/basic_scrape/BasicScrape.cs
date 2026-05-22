//#:package ScrAPI
#:project ../../DevEnterprise.Scrapi.Sdk.csproj

using DevEnterprise.Scrapi.Sdk;

const string Url = "https://deventerprise.com";
var apiKey = Environment.GetEnvironmentVariable("SCRAPI_API_KEY") ?? string.Empty;

var request = new ScrapeRequest(Url)
{
  UseBrowser = true,
  IncludeScreenshot = true,
  IncludeVideo = true,
  IncludePdf = true,
};

using var client = new ScrapiClient(apiKey);
Console.WriteLine($"Scraping {Url}");
Console.WriteLine();

var response = await client.ScrapeAsync(request);
if (response is null)
{
  Console.WriteLine("No response was returned by ScrAPI.");
  return;
}

var preview = (response.Content ?? string.Empty).Trim().Replace(Environment.NewLine, " ");
if (preview.Length > 200)
{
  preview = preview[..200] + "...";
}

Console.WriteLine($"Request URL    : {response.RequestUrl}");
Console.WriteLine($"Response URL   : {response.ResponseUrl}");
Console.WriteLine($"Status Code    : {response.StatusCode}");
Console.WriteLine($"Duration       : {response.Duration}");
Console.WriteLine($"Credits Used   : {response.CreditsUsed}");
Console.WriteLine($"Screenshot URL : {response.ScreenshotUrl}");
Console.WriteLine($"Video URL      : {response.VideoUrl}");
Console.WriteLine($"PDF URL        : {response.PdfUrl}");
Console.WriteLine($"Content Hash   : {response.ContentHash}");
Console.WriteLine($"Preview        : {preview}");
Console.WriteLine();
