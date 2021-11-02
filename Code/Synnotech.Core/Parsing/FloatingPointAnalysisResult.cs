using System.Globalization;

namespace Synnotech.Core.Parsing;

/// <summary>
/// Represents the result of analysing a string for points and commas.
/// </summary>
/// <param name="NumberOfCommas">The total number of commas found in the string.</param>
/// <param name="IndexOfLastComma">The index of the last comma found in the string.</param>
/// <param name="NumberOfPoints">The total number of points found in the string.</param>
/// <param name="IndexOfLastPoint">The index of the last point found in the string.</param>
public readonly record struct FloatingPointAnalysisResult(int NumberOfCommas,
                                                          int IndexOfLastComma,
                                                          int NumberOfPoints,
                                                          int IndexOfLastPoint)
{
    /// <summary>
    /// Selects either the invariant or German culture for parsing the number.
    /// This is based on the number of points and commas and their position
    /// within the string.
    /// </summary>
    public CultureInfo ChooseCultureInfo()
    {
        if (NumberOfCommas == 0)
            return NumberOfPoints is 0 or 1 ? Cultures.InvariantCulture : Cultures.GermanCulture;
        if (NumberOfPoints == 0)
            return NumberOfCommas is 1 ? Cultures.GermanCulture : Cultures.InvariantCulture;
        return IndexOfLastComma > IndexOfLastPoint ? Cultures.GermanCulture : Cultures.InvariantCulture;
    }
}