namespace DevEnterprise.Scrapi.Sdk.BrowserCommands;

/// <summary>
/// Waiting browser command.
/// </summary>
public sealed record WaitCommand : IBrowserCommand
{
  /// <inheritdoc/>
  public string CommandName => "wait";

  /// <summary>
  /// The number of milliseconds to wait (maximum of 15000).
  /// </summary>
  public int Milliseconds { get; set; }
}
