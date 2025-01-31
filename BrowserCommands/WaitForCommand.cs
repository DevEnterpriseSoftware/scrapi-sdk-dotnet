namespace DevEnterprise.Scrapi.Sdk.BrowserCommands;

/// <summary>
/// Wait for target element browser command.
/// </summary>
public sealed record WaitForCommand : IBrowserCommand
{
  /// <inheritdoc/>
  public string CommandName => "wait_for";

  /// <summary>
  /// The CSS/XPath used to find the target.
  /// </summary>
  public string TargetSelector { get; set; } = string.Empty;
}
