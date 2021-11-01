using FluentAssertions;
using Synnotech.Core.Parsing;
using Xunit;

namespace Synnotech.Core.Tests.Parsing;

public static class DoubleParserTests
{
    private const double Precision = 0.0000001;

    [Theory]
    [InlineData("0.74", 0.74)]
    [InlineData("1.34", 1.34)]
    [InlineData("391202.9", 391202.9)]
    [InlineData("-20.816", -20.816)]
    [InlineData("15,019.33", 15_019.33)]
    public static void ParseFloatingPointNumberWithDecimalPoint(string text, double expectedValue) =>
        CheckNumber(text, expectedValue);

    [Theory]
    [InlineData("000,7832", 0.7832)]
    [InlineData("-0,499", -0.499)]
    [InlineData("40593,84", 40593.84)]
    [InlineData("1.943.100,84", 1_943_100.84)]
    public static void ParseFloatingPointNumberWithDecimalComma(string text, double expectedValue) =>
        CheckNumber(text, expectedValue);

    [Theory]
    [InlineData("15", 15.0)]
    [InlineData("-743923", -743923.0)]
    [InlineData("239.482.392.923", 239_482_392_923.0)]
    [InlineData("21,500,000", 21_500_000.0)]
    public static void ParseInteger(string text, double expectedValue) =>
        CheckNumber(text, expectedValue);

    private static void CheckNumber(string text, double expectedValue)
    {
        var result = DoubleParser.TryParse(text, out var parsedValue);

        result.Should().BeTrue();
        parsedValue.Should().BeApproximately(expectedValue, Precision);
    }

    [Theory]
    [InlineData("Foo")]
    [InlineData("Bar")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("9392gk381")]
    public static void InvalidNumber(string text)
    {
        var result = DoubleParser.TryParse(text, out var actualValue);

        result.Should().BeFalse();
        actualValue.Should().Be(default);
    }
}