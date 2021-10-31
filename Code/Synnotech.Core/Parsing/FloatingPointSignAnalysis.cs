using System;

namespace Synnotech.Core.Parsing;

/// <summary>
/// Provides a member to analysis a string for points and commas.
/// </summary>
public static class FloatingPointSignAnalysis
{
    /// <summary>
    /// Counts the number of commas and points within the specified text
    /// and determines the last index for each of them. This can be used
    /// to 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static FloatingPointSignAnalysisResult AnalyseText(ReadOnlySpan<char> text)
    {
        var numberOfCommas = 0;
        var numberOfPoints = 0;
        var indexOfLastComma = -1;
        var indexOfLastPoint = -1;

        for (var i = 0; i < text.Length; i++)
        {
            var character = text[i];
            switch (character)
            {
                case ',':
                    numberOfCommas++;
                    indexOfLastComma = i;
                    break;
                case '.':
                    numberOfPoints++;
                    indexOfLastPoint = i;
                    break;
            }
        }

        return new FloatingPointSignAnalysisResult(numberOfCommas,
                                                   indexOfLastComma,
                                                   numberOfPoints,
                                                   indexOfLastPoint);
    }
}