namespace DevEnterprise.Scrapi.Sdk.BrowserCommands;

/// <summary>
/// Defines the contract for browser commands.
/// </summary>
public interface IBrowserCommand
{
  /// <summary>
  /// The command name to identify the type of browser command.
  /// </summary>
  string CommandName { get; }
}
