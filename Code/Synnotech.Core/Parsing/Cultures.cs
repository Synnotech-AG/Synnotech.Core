using System.Globalization;

namespace Synnotech.Core.Parsing;

/// <summary>
/// Caches the invariant and German culture.
/// </summary>
public static class Cultures
{
    /// <summary>
    /// Gets the invariant culture.
    /// </summary>
    public static CultureInfo InvariantCulture => CultureInfo.InvariantCulture;

    /// <summary>
    /// Gets the German culture.
    /// </summary>
    public static CultureInfo GermanCulture { get; } = new ("de-DE");
}