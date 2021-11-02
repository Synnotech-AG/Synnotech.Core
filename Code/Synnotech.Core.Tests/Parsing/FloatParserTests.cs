using FluentAssertions;
using Synnotech.Core.Parsing;
using Xunit;

namespace Synnotech.Core.Tests.Parsing;

public static class FloatParserTests
{
    private const float Precision = 0.000001f;

    [Theory]
    [InlineData("0.13", 0.13f)]
    [InlineData("-3.41", -3.41f)]
    [InlineData("10050.9", 10050.9f)]
    [InlineData("-394.955", -394.955f)]
    [InlineData("15,019.33", 15_019.33f)]
    public static void ParseFloatingPointNumberWithDecimalPoint(string text, float expectedValue) =>
        CheckNumber(text, expectedValue);

    [Theory]
    [InlineData("000,7832", 0.7832f)]
    [InlineData("-0,499", -0.499f)]
    [InlineData("40593,84", 40593.84f)]
    [InlineData("1.943.100,84", 1_943_100.84f)]
    public static void ParseFloatingPointNumberWithDecimalComma(string text, float expectedValue) =>
        CheckNumber(text, expectedValue);

    [Theory]
    [InlineData("15", 15.0f)]
    [InlineData("-743923", -743923.0f)]
    [InlineData("482.392.923", 482_392_923.0f)]
    [InlineData("21,500,000", 21_500_000.0f)]
    public static void ParseInteger(string text, float expectedValue) =>
        CheckNumber(text, expectedValue);

    private static void CheckNumber(string text, float expectedValue)
    {
        var result = FloatParser.TryParse(text, out var parsedValue);

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
        var result = FloatParser.TryParse(text, out var actualValue);

        result.Should().BeFalse();
        actualValue.Should().Be(default);
    }
}