using System;
using System.Globalization;
using Light.GuardClauses;

namespace Synnotech.Core.Parsing;

/// <summary>
/// Provides methods for parsing <see cref="decimal" /> values.
/// </summary>
public static class DecimalParser
{
    /// <summary>
    /// <para>
    /// Tries to parse the specified text to a decimal value. This method will
    /// check if the text uses the point character '.' or the comma character ','
    /// as the decimal sign and will parse the text either with the invariant culture
    /// or the German culture. <see cref="NumberStyles.Number" /> is used when parsing
    /// the text.
    /// </para>
    /// <para>
    /// BE CAREFUL: if you have a number with only a single thousand-delimiter sign (i.e. no
    /// decimal sign), this number will not be parsed correctly. The thousand-delimiter
    /// sign will be interpreted as the decimal sign by this method. We recognize that
    /// this scenario is rare, as especially human input will most likely never use
    /// the thousand-delimiter sign, but if you happen to have this scenario, please
    /// use <see cref="decimal.TryParse(string, NumberStyles, IFormatProvider, out decimal)" />
    /// instead and specify the corresponding culture info.
    /// </para>
    /// </summary>
    /// <param name="text">The text to be parsed.</param>
    /// <param name="value">The parsed decimal value.</param>
    /// <returns>True if parsing was successful, else false.</returns>
    public static bool TryParse(string text, out decimal value) =>
        TryParse(text, NumberStyles.Number, out value);

    /// <summary>
    /// <para>
    /// Tries to parse the specified text to a decimal value. This method will
    /// check if the text uses the point character '.' or the comma character ','
    /// as the decimal sign and will parse the text either with the invariant culture
    /// or the German culture.
    /// </para>
    /// <para>
    /// BE CAREFUL: if you have a number with only a single thousand-delimiter sign (i.e. no
    /// decimal sign), this number will not be parsed correctly. The thousand-delimiter
    /// sign will be interpreted as the decimal sign by this method. We recognize that
    /// this scenario is rare, as especially human input will most likely never use
    /// the thousand-delimiter sign, but if you happen to have this scenario, please
    /// use <see cref="decimal.TryParse(string, NumberStyles, IFormatProvider, out decimal)" />
    /// instead and specify the corresponding culture info.
    /// </para>
    /// </summary>
    /// <param name="text">The text to be parsed.</param>
    /// <param name="style">The number style that describes how the parser will handle the text.</param>
    /// <param name="value">The parsed decimal value.</param>
    /// <returns>True if parsing was successful, else false.</returns>
    public static bool TryParse(string text, NumberStyles style, out decimal value)
    {
        if (text.IsNullOrWhiteSpace())
        {
            value = default;
            return false;
        }

        var result = FloatingPointAnalysis.AnalyseText(text.AsSpan());
        var cultureInfo = result.ChooseCultureInfo();
        return decimal.TryParse(text, style, cultureInfo, out value);
    }

#if NETSTANDARD2_1
    /// <summary>
    /// <para>
    /// Tries to parse the specified text to a decimal value. This method will
    /// check if the text uses the point character '.' or the comma character ','
    /// as the decimal sign and will parse the text either with the invariant culture
    /// or the German culture. <see cref="NumberStyles.Number" /> is used when parsing
    /// the text.
    /// </para>
    /// <para>
    /// BE CAREFUL: if you have a number with only a single thousand-delimiter sign (i.e. no
    /// decimal sign), this number will not be parsed correctly. The thousand-delimiter
    /// sign will be interpreted as the decimal sign by this method. We recognize that
    /// this scenario is rare, as especially human input will most likely never use
    /// the thousand-delimiter sign, but if you happen to have this scenario, please
    /// use <see cref="decimal.TryParse(string, NumberStyles, IFormatProvider, out decimal)" />
    /// instead and specify the corresponding culture info.
    /// </para>
    /// </summary>
    /// <param name="text">The text to be parsed.</param>
    /// <param name="value">The parsed decimal value.</param>
    /// <returns>True if parsing was successful, else false.</returns>
    public static bool TryParse(ReadOnlySpan<char> text, out decimal value) =>
        TryParse(text, NumberStyles.Number, out value);

    /// <summary>
    /// <para>
    /// Tries to parse the specified text to a decimal value. This method will
    /// check if the text uses the point character '.' or the comma character ','
    /// as the decimal sign and will parse the text either with the invariant culture
    /// or the German culture.
    /// </para>
    /// <para>
    /// BE CAREFUL: if you have a number with only a single thousand-delimiter sign (i.e. no
    /// decimal sign), this number will not be parsed correctly. The thousand-delimiter
    /// sign will be interpreted as the decimal sign by this method. We recognize that
    /// this scenario is rare, as especially human input will most likely never use
    /// the thousand-delimiter sign, but if you happen to have this scenario, please
    /// use <see cref="decimal.TryParse(string, NumberStyles, IFormatProvider, out decimal)" />
    /// instead and specify the corresponding culture info.
    /// </para>
    /// </summary>
    /// <param name="text">The text to be parsed.</param>
    /// <param name="style">The number style that describes how the parser will handle the text.</param>
    /// <param name="value">The parsed decimal value.</param>
    /// <returns>True if parsing was successful, else false.</returns>
    public static bool TryParse(ReadOnlySpan<char> text, NumberStyles style, out decimal value)
    {
        if (text.IsWhiteSpace())
        {
            value = default;
            return false;
        }

        var result = FloatingPointAnalysis.AnalyseText(text);
        var cultureInfo = result.ChooseCultureInfo();
        return decimal.TryParse(text, style, cultureInfo, out value);
    }
#endif
}