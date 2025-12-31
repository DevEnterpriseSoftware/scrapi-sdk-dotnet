namespace DevEnterprise.Scrapi.Sdk.BrowserCommands;

/// <summary>
/// Option list selection browser command.
/// </summary>
public sealed record SelectCommand : IBrowserCommand
{
  /// <inheritdoc/>
  public string CommandName => "select";

  /// <summary>
  /// The CSS/XPath to find the options target.
  /// </summary>
  public string TargetSelector { get; set; } = string.Empty;

  /// <summary>
  /// The option value to select on the target.
  /// </summary>
  public string SelectValue { get; set; } = string.Empty;
}
