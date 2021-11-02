using FluentAssertions;
using Synnotech.Core.Parsing;
using Xunit;

namespace Synnotech.Core.Tests.Parsing;

public static class DecimalParserTests
{
    [Theory]
    [MemberData(nameof(DecimalPointData))]
    public static void ParseFloatingPointNumberWithDecimalPoint(string text, decimal expectedValue) =>
        CheckNumber(text, expectedValue);

    public static readonly TheoryData<string, decimal> DecimalPointData =
        new ()
        {
            { "0.74", 0.74m },
            { "1.34", 1.34m },
            { "391202.9", 391202.9m },
            { "-20.816", -20.816m },
            { "15,019.33", 15_019.33m }
        };

    [Theory]
    [MemberData(nameof(DecimalCommaData))]
    public static void ParseFloatingPointNumberWithDecimalComma(string text, decimal expectedValue) =>
        CheckNumber(text, expectedValue);

    public static readonly TheoryData<string, decimal> DecimalCommaData =
        new ()
        {
            { "000,7832", 0.7832m },
            { "-0,499", -0.499m },
            { "40593,84", 40593.84m },
            { "1.943.100,84", 1_943_100.84m }
        };

    [Theory]
    [MemberData(nameof(IntegerData))]
    public static void ParseInteger(string text, decimal expectedValue) =>
        CheckNumber(text, expectedValue);

    public static readonly TheoryData<string, decimal> IntegerData =
        new ()
        {
            { "15", 15.0m },
            { "-743923", -743923.0m },
            { "239.482.392.923", 239_482_392_923.0m },
            { "21,500,000", 21_500_000.0m }
        };

    private static void CheckNumber(string text, decimal expectedValue)
    {
        var result = DecimalParser.TryParse(text, out var parsedValue);

        result.Should().BeTrue();
        parsedValue.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("Foo")]
    [InlineData("Bar")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("9392gk381")]
    public static void InvalidNumber(string text)
    {
        var result = DecimalParser.TryParse(text, out var actualValue);

        result.Should().BeFalse();
        actualValue.Should().Be(default);
    }
}