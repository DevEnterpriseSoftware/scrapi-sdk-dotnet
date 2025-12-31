namespace DevEnterprise.Scrapi.Sdk.BrowserCommands;

/// <summary>
/// Scroll page browser command.
/// </summary>
public sealed record ScrollCommand : IBrowserCommand
{
  /// <inheritdoc/>
  public string CommandName => "scroll";

  /// <summary>
  /// The number of pixels to scroll. Use negative values to scroll up.
  /// </summary>
  public int Pixels { get; set; } = 1000;
}
