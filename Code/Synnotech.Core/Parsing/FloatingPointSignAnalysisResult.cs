namespace Synnotech.Core.Parsing;

/// <summary>
/// Represents the result of analysing a string for points and commas.
/// </summary>
/// <param name="NumberOfCommas">The total number of commas found in the string.</param>
/// <param name="IndexOfLastComma">The index of the last comma found in the string.</param>
/// <param name="NumberOfPoints">The total number of points found in the string.</param>
/// <param name="IndexOfLastPoint">The index of the last point found in the string.</param>
public record struct FloatingPointSignAnalysisResult(int NumberOfCommas,
                                                     int IndexOfLastComma,
                                                     int NumberOfPoints,
                                                     int IndexOfLastPoint);