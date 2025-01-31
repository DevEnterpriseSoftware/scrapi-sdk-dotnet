namespace DevEnterprise.Scrapi.Sdk.BrowserCommands;

/// <summary>
/// Click browser command.
/// </summary>
public sealed record ClickCommand : IBrowserCommand
{
  /// <inheritdoc/>
  public string CommandName => "click";

  /// <summary>
  /// The CSS/XPath used to find the target to click.
  /// </summary>
  public string TargetSelector { get; set; } = string.Empty;
}
