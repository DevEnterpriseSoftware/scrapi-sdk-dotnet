![ScrAPI logo](https://raw.githubusercontent.com/DevEnterpriseSoftware/scrapi-sdk-dotnet/master/icon_small.png)
	
# ScrAPI SDK for .NET

[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)
![Nuget](https://img.shields.io/nuget/dt/ScrAPI)

This is the official .NET SDK for the ScrAPI web scraping service.
- [Website](https://scrapi.tech)
- [Documentation](https://scrapi.tech/docs)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Scrape Request Options](#scrape-request-options)
    - [Browser Commands](#browser-commands)
- [Scrape Response Data](#scrape-response-data)
    - [Extensions](#extensions)
- [Scrape Request Defaults](#scrape-request-defaults)
- [Lookups](#lookups)
    - [Balance Check](#balance-check)
    - [Supported Countries](#supported-countries)
- [Exceptions](#exceptions)
- [Dependency Injection](#dependency-injection)

## Installation

ScrAPI can be [found on NuGet](https://www.nuget.org/packages/ScrAPI) and can be installed by copying and pasting the following command into your Package Manager Console within Visual Studio (Tools > NuGet Package Manager > Package Manager Console).

```sh
Install-Package ScrAPI
```

Alternatively if you're using .NET Core then you can install ScrAPI via the command line interface with the following command:

```sh
dotnet add package ScrAPI
```

## Quick Start

You can start scraping websites with as little as three lines of code:

```csharp
var client = new ScrapiClient("YOUR_API_KEY");  // "" for limited free mode.
var request = new ScrapeRequest("https://deventerprise.com");
var response = await client.ScrapeAsync(request);

// The result will contain the content and other information about the operation.
Console.WriteLine(response?.Content);
```

## Dependency Injection

The API client implements the interface `IScrapiClient` which can be use with dependency injection and assist with mocking for unit tests.

```csharp
// Add singleton to IServiceCollection
services.AddSingleton<IScrapiClient>(_ => new ScrapiClient("YOUR_API_KEY"));
```

## Scrape Request Options

The API provides a number of options to assist with scraping a target website.

```csharp
var request = new ScrapeRequest("https://deventerprise.com")
{
  Cookies = new Dictionary<string, string>
  {
    { "cookie1", "value1" },
    { "cookie2", "value2" },
  },
  Headers = new Dictionary<string, string>
  {
    { "header1", "value1" },
    { "header2", "value2" },
  },
  ProxyCountry = "USA",
  ProxyType = ProxyType.Residential,
  UseBrowser = true,
  SolveCaptchas = true,
  RequestMethod = "GET",
  ResponseFormat = ResponseFormat.Html,
  CustomProxyUrl = "https://user:password@local.proxy:8080",
  SessionId = Guid.NewGuid().ToString(),
  CallbackUrl = new Uri("https://webhook.site/"),
};
```

For more detailed information on these options please refer to the [documentation](https://scrapi.tech/docs/api_details/v1_scrape).

### Browser Commands

When the `UseBrowser` request option is used, you can supply any number of browser commands to control the browser before the resulting page state is captured.

```csharp
var request = new ScrapeRequest("https://www.roboform.com/filling-test-all-fields")
{
  UseBrowser = true,
  AcceptDialogs = true
};

// Example of chaining commands to control the website.
request.BrowserCommands
  .Input("input[name='01___title']", "Mr")
  .Input("input[name='02frstname']", "Werner")
  .Input("input[name='04lastname']", "van Deventer")
  .Select("select[name='40cc__type']", "Discover")
  .Wait(TimeSpan.FromSeconds(3))
  .WaitFor("input[type='reset']")
  .Click("input[type='reset']")
  .Wait(TimeSpan.FromSeconds(1))
  .Evaluate("console.log('any valid code...')");
```

## Scrape Response data

The response data contains all the result information about your request including the HTML data, headers and any cookies.

```csharp
var response = await client.ScrapeAsync(request);

Console.WriteLine(response.RequestUrl);  // The requested URL.
Console.WriteLine(response.ResponseUrl); // The final URL of the page.
Console.WriteLine(response.Duration);    // The amount of time the operation took.
Console.WriteLine(response.Attempts);    // The number of attempts to scrape the page.
Console.WriteLine(response.CreditsUsed); // The number of credits used for this request.
Console.WriteLine(response.StatusCode);  // The response status code from the request.
Console.WriteLine(response.Content);     // The final page content.
Console.WriteLine(response.ContentHash); // SHA1 hash of the content.
Console.WriteLine(response.Html);        // Html Agility Pack parsed HTML content.

foreach (var captchaSolved in response.CaptchasSolved)
{
  Console.WriteLine($"{captchaSolved.Value} occurrences of {captchaSolved.Key} solved");
}

foreach (var header in response.Headers)
{
  Console.WriteLine($"{header.Key}: {header.Value}");
}

foreach (var cookie in response.Cookies)
{
  Console.WriteLine($"{cookie.Key}: {cookie.Value}");
}

foreach (var errorMessage in response.ErrorMessages ?? [])
{
  Console.WriteLine(errorMessage);  // Any errors that occurred during the request.
}
```

### Extensions

This SDK also provides a number of convenient [extensions](https://github.com/DevEnterpriseSoftware/scrapi-sdk-dotnet/blob/master/Extensions/ScrapiExtensions.cs) to assist in parsing and checking the data once retrieved.

- Extract numbers only
- Strip script tags from HTML
- Safe query selector that does not throw
- Next/adjacent element finder
- Comprehensive check of element visibility.
- Style parsing.

[Html Agility Pack](https://github.com/zzzprojects/html-agility-pack) is included as well as [Hazz](https://github.com/atifaziz/Hazz) for HTML parsing.

## Scrape Request Defaults

The SDK provides a static class to define the defaults that will be applied to every `ScrapeRequest` object.
This can greatly reduce the amount of code required to create new requests if all/most of your requests need to use the same values.

```csharp
// Set default that will apply to all new `ScrapeRequest` object (unless overridden).
ScrapeRequestDefaults.ProxyType = ProxyType.Residential;
ScrapeRequestDefaults.UseBrowser = true;
ScrapeRequestDefaults.SolveCaptchas = true;
ScrapeRequestDefaults.Headers.Add("Sample", "Custom-Value");

// Any new request will have the corresponding values automatically applied.
var request = new ScrapeRequest("https://deventerprise.com") { ProxyType = ProxyType.Tor };
Debug.Assert(request.ProxyType == ProxyType.Tor);  // Overridden
Debug.Assert(request.UseBrowser);
Debug.Assert(request.SolveCaptchas);
Debug.Assert(request.Headers.ContainsKey("Sample"));
```

## Lookups

The SDK provides wrappers for basic lookups such as the credit balance of an API key and a list of supported country codes to use with the `ProxyCountry` request option.

### Balance Check

Easily check the remaining credit balance for your API key.

```csharp
var balance = await client.GetCreditBalanceAsync();
```

### Supported Countries

```csharp
var supportedCountries = await client.GetSupportedCountriesAsync();

// Use the Key value in the ProxyCountry request property.
foreach (var country in supportedCountries)
{
  Console.WriteLine($"{country.Key}: {country.Name}");
}
```

## Exceptions

Any errors using the API will always result in a `ScrapiException`.
This exception also contains a property for the HTTP status that caused the exception to assist with retry logic.

```csharp
var client = new ScrapiClient("YOUR_API_KEY");  // "" for limited free mode.
var request = new ScrapeRequest("https://deventerprise.com");

try
{
  var result = await client.ScrapeAsync(request);
  Console.WriteLine(result?.Content);
}
catch (ScrapiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
{
  // Error messages from the server aim to be as helpful as possible.
  Console.WriteLine(ex.Message);
  throw;
}

// The result will contain the content and other information about the operation.
Console.WriteLine(result?.Content);
```
