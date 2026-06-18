using Spectre.Console;

namespace DotBahn.Samples.StationBrowser.Additional;

/// <summary>
/// Convenience wrappers around common Spectre.Console patterns used across samples.
/// </summary>
public static class ConsoleExtensions {
    /// <summary>
    /// Shows a Gruvbox-styled status spinner with the given message while executing an action.
    /// </summary>
    public static Task StatusAsync(string message, Func<StatusContext, Task> action) =>
        AnsiConsole.Status().Spinner(Spinner.Known.Dots).StartAsync(message, action);

    /// <summary>
    /// Creates a centered title Rule in Gruvbox blue.
    /// </summary>
    public static Rule TitleRule(string formattedTitle) => new($"[bold {Gruvbox.Blue}]{formattedTitle}[/]") {
        Justification = Justify.Center,
        Style = Style.Parse(Gruvbox.Blue)
    };

    /// <summary>
    /// Creates a separator Rule in Gruvbox gray.
    /// </summary>
    public static Rule SeparatorRule() => new() {
        Style = Style.Parse(Gruvbox.Gray)
    };

    /// <summary>
    /// Pre-built Gruvbox gray style.
    /// </summary>
    public static Style GrayStyle => Style.Parse(Gruvbox.Gray);
}
