using DevEnterprise.Scrapi.Sdk.BrowserCommands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DevEnterprise.Scrapi.Sdk.Tests;

public class BrowserCommandsTests
{
  [Fact]
  public void BrowserCommandList_SerializesToExpectedWireShape()
  {
    var request = new ScrapeRequest("https://deventerprise.com")
    {
      UseBrowser = true,
    };

    request.BrowserCommands
      .Input("input[name='name']", "Werner")
      .Select("select[name='type']", "Discover")
      .Wait(1000)
      .WaitFor("button[type='submit']")
      .Click("button[type='submit']")
      .Scroll(500)
      .Evaluate("console.log('done')");

    var json = JsonConvert.SerializeObject(request);
    var browserCommandsJson = JObject.Parse(json)["BrowserCommands"]?.ToString(Formatting.None);

    var commands = JArray.Parse(browserCommandsJson ?? "[]");
    Assert.Equal(7, commands.Count);
    Assert.Equal("input", ((JProperty)((JObject)commands[0]).First!).Name);
    Assert.Equal("select", ((JProperty)((JObject)commands[1]).First!).Name);
    Assert.Equal("wait", ((JProperty)((JObject)commands[2]).First!).Name);
    Assert.Equal("wait_for", ((JProperty)((JObject)commands[3]).First!).Name);
    Assert.Equal("click", ((JProperty)((JObject)commands[4]).First!).Name);
    Assert.Equal("scroll", ((JProperty)((JObject)commands[5]).First!).Name);
    Assert.Equal("javascript", ((JProperty)((JObject)commands[6]).First!).Name);
  }

  [Fact]
  public void Wait_ThrowsWhenAboveMaximum()
  {
    IList<IBrowserCommand> commands = [];

    var ex = Assert.Throws<ArgumentException>(() => commands.Wait(TimeSpan.FromSeconds(16)));

    Assert.Equal("The maximum wait time is 15 seconds.", ex.Message);
  }
}
