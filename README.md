<div align="center">

![ScrAPI logo](icon_small.png)
	
# ScrAPI SDK for .NET

</div>

<div align="center">

[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)
![Nuget](https://img.shields.io/nuget/dt/ScrAPI)

</div>

This is the official .NET SDK for the ScrAPI web scraping service.
- [Website](https://scrapi.tech)
- [Documentation](https://scrapi.tech/docs)

## Quick Start

Add the [ScrAPI](https://www.nuget.org/packages/ScrAPI) package to your project by running the following command:

```sh
dotnet add package ScrAPI
```

Then you can start scraping websites with as little as three lines of code:

```csharp
var client = new ScrapiClient("YOUR_API_KEY");  // "" for limited free mode.
var request = new ScrapeRequest("https://deventerprise.com");
var result = await client.ScrapeAsync(request);

// The result will contain the content and other information about the operation.
Console.WriteLine(result?.Content);
```