namespace DevEnterprise.Scrapi.Sdk.BrowserCommands;

/// <summary>
/// Evaluate JavaScript browser command.
/// </summary>
public sealed record JavaScriptCommand : IBrowserCommand
{
  /// <inheritdoc/>
  public string CommandName => "javascript";

  /// <summary>
  /// The JavaScript snippet to execute (5 second execution limit).
  /// </summary>
  public string Script { get; set; } = string.Empty;
}
