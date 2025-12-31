using DevEnterprise.Scrapi.Sdk.BrowserCommands;

namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// Extensions methods to fluently setup the browser commands to execute in the browser.
/// </summary>
public static class BrowserCommandExtensions
{
  /// <summary>
  /// Perform a mouse click on a target element.
  /// </summary>
  /// <param name="commands">List of browser commands to extend.</param>
  /// <param name="targetSelector">The CSS/XPath used to find the target to click.</param>
  /// <returns>
  /// The same list of browser commands that got extended.
  /// </returns>
  public static IList<IBrowserCommand> Click(this IList<IBrowserCommand> commands, string targetSelector)
  {
    commands.Add(new ClickCommand { TargetSelector = targetSelector });
    return commands;
  }

  /// <summary>
  /// Scroll the web page by a given number of pixels before continuing.
  /// </summary>
  /// <param name="commands">List of browser commands to extend.</param>
  /// <param name="pixels">The number of pixels to scroll, use negative values to scroll up.</param>
  /// <returns>
  /// The same list of browser commands that got extended.
  /// </returns>
  public static IList<IBrowserCommand> Scroll(this IList<IBrowserCommand> commands, int pixels = 1000)
  {
    commands.Add(new ScrollCommand { Pixels = pixels });
    return commands;
  }

  /// <summary>
  /// Wait for a given number of milliseconds before continuing.
  /// </summary>
  /// <param name="commands">List of browser commands to extend.</param>
  /// <param name="milliseconds">The number of milliseconds to wait (maximum of 15000).</param>
  /// <returns>
  /// The same list of browser commands that got extended.
  /// </returns>
  public static IList<IBrowserCommand> Wait(this IList<IBrowserCommand> commands, int milliseconds)
  {
    return commands.Wait(TimeSpan.FromMilliseconds(milliseconds));
  }

  /// <summary>
  /// Wait for a given amount of time before continuing.
  /// </summary>
  /// <param name="commands">List of browser commands to extend.</param>
  /// <param name="timeSpan">The amount of time to wait (maximum of 15 seconds).</param>
  /// <returns>
  /// The same list of browser commands that got extended.
  /// </returns>
  /// <exception cref="ArgumentException">The maximum wait time is 15 seconds.</exception>
  public static IList<IBrowserCommand> Wait(this IList<IBrowserCommand> commands, TimeSpan timeSpan)
  {
    if (timeSpan.TotalMilliseconds > 15000)
    {
      throw new ArgumentException("The maximum wait time is 15 seconds.");
    }

    commands.Add(new WaitCommand { Milliseconds = Convert.ToInt32(timeSpan.TotalMilliseconds) });
    return commands;
  }

  /// <summary>
  /// Wait for a element for be available in the DOM.
  /// </summary>
  /// <param name="commands">List of browser commands to extend.</param>
  /// <param name="targetSelector">The CSS/XPath used to find the target.</param>
  /// <returns>
  /// The same list of browser commands that got extended.
  /// </returns>
  public static IList<IBrowserCommand> WaitFor(this IList<IBrowserCommand> commands, string targetSelector)
  {
    commands.Add(new WaitForCommand { TargetSelector = targetSelector });
    return commands;
  }

  /// <summary>
  /// Enter a text value into an input, text area or any editable content on the web page.
  /// </summary>
  /// <param name="commands">List of browser commands to extend.</param>
  /// <param name="targetSelector">The CSS/XPath to find the input target.</param>
  /// <param name="inputValue">The text value to enter on the target.</param>
  /// <returns>
  /// The same list of browser commands that got extended.
  /// </returns>
  public static IList<IBrowserCommand> Input(this IList<IBrowserCommand> commands, string targetSelector, string inputValue)
  {
    commands.Add(new InputCommand { TargetSelector = targetSelector, InputValue = inputValue });
    return commands;
  }

  /// <summary>
  /// Select an option from a drop-down list.
  /// </summary>
  /// <param name="commands">List of browser commands to extend.</param>
  /// <param name="targetSelector">The CSS/XPath to find the select target.</param>
  /// <param name="selectValue">The value to select on the target.</param>
  /// <remarks>
  /// The selection value can be the option value or the label/text.
  /// </remarks>
  /// <returns>
  /// The same list of browser commands that got extended.
  /// </returns>
  public static IList<IBrowserCommand> Select(this IList<IBrowserCommand> commands, string targetSelector, string selectValue)
  {
    commands.Add(new SelectCommand { TargetSelector = targetSelector, SelectValue = selectValue });
    return commands;
  }

  /// <summary>
  /// JavaScript snippet run on the web page once it has completed loading.
  /// </summary>
  /// <param name="commands">List of browser commands to extend.</param>
  /// <param name="javaScript">The JavaScript snippet to execute (5 second execution limit).</param>
  /// <returns>
  /// The same list of browser commands that got extended.
  /// </returns>
  public static IList<IBrowserCommand> Evaluate(this IList<IBrowserCommand> commands, string javaScript)
  {
    commands.Add(new JavaScriptCommand { Script = javaScript });
    return commands;
  }
}
