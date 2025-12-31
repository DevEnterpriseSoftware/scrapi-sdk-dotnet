namespace DevEnterprise.Scrapi.Sdk.BrowserCommands;

/// <summary>
/// Text input browser command.
/// </summary>
public sealed record InputCommand : IBrowserCommand
{
  /// <inheritdoc/>
  public string CommandName => "input";

  /// <summary>
  /// The CSS/XPath to find the input target.
  /// </summary>
  public string TargetSelector { get; set; } = string.Empty;

  /// <summary>
  /// The text value to enter on the target.
  /// </summary>
  public string InputValue { get; set; } = string.Empty;
}
