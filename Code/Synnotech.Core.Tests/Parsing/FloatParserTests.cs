using System;
using FluentAssertions;
using Synnotech.Core.Parsing;
using Xunit;

namespace Synnotech.Core.Tests.Parsing;

public static class FloatParserTests
{
    private const float Precision = 0.000001f;

    [Theory]
    [MemberData(nameof(NumbersWithDecimalPoint))]
    public static void ParseFloatingPointNumberWithDecimalPoint(string text, float expectedValue) =>
        CheckNumber(text, expectedValue);

    [Theory]
    [MemberData(nameof(NumbersWithDecimalPoint))]
    public static void ParseFloatingPointNumberWithDecimalPointAsSpan(string text, float expectedValue) =>
        CheckNumberAsSpan(text, expectedValue);

    public static readonly TheoryData<string, float> NumbersWithDecimalPoint =
        new ()
        {
            { "0.13", 0.13f },
            { "-3.41", -3.41f },
            { "10050.9", 10050.9f },
            { "-394.955", -394.955f },
            { "15,019.33", 15_019.33f }
        };

    [Theory]
    [MemberData(nameof(NumbersWithDecimalComma))]
    public static void ParseFloatingPointNumberWithDecimalComma(string text, float expectedValue) =>
        CheckNumber(text, expectedValue);

    [Theory]
    [MemberData(nameof(NumbersWithDecimalComma))]
    public static void ParseFloatingPointNumberWithDecimalCommaAsSpan(string text, float expectedValue) =>
        CheckNumberAsSpan(text, expectedValue);

    public static readonly TheoryData<string, float> NumbersWithDecimalComma =
        new ()
        {
            { "000,7832", 0.7832f },
            { "-0,499", -0.499f },
            { "40593,84", 40593.84f },
            { "1.943.100,84", 1_943_100.84f }
        };

    [Theory]
    [MemberData(nameof(IntegerNumbers))]
    public static void ParseInteger(string text, float expectedValue) =>
        CheckNumber(text, expectedValue);

    [Theory]
    [MemberData(nameof(IntegerNumbers))]
    public static void ParseIntegerAsSpan(string text, float expectedValue) =>
        CheckNumberAsSpan(text, expectedValue);

    public static readonly TheoryData<string, float> IntegerNumbers =
        new ()
        {
            { "15", 15.0f },
            { "-743923", -743923.0f },
            { "482.392.923", 482_392_923.0f },
            { "21,500,000", 21_500_000.0f }
        };

    private static void CheckNumber(string text, float expectedValue)
    {
        var result = FloatParser.TryParse(text, out var parsedValue);

        result.Should().BeTrue();
        parsedValue.Should().BeApproximately(expectedValue, Precision);
    }

    private static void CheckNumberAsSpan(ReadOnlySpan<char> text, float expectedValue)
    {
        var result = FloatParser.TryParse(text, out var parsedValue);

        result.Should().BeTrue();
        parsedValue.Should().BeApproximately(expectedValue, Precision);
    }

    [Theory]
    [MemberData(nameof(InvalidNumbers))]
    public static void InvalidNumber(string text)
    {
        var result = FloatParser.TryParse(text, out var actualValue);

        result.Should().BeFalse();
        actualValue.Should().Be(default);
    }

    [Theory]
    [MemberData(nameof(InvalidNumbers))]
    public static void InvalidNumberAsSpan(string text)
    {
        var result = FloatParser.TryParse(text.AsSpan(), out var actualValue);

        result.Should().BeFalse();
        actualValue.Should().Be(default);
    }

    public static readonly TheoryData<string?> InvalidNumbers =
        new ()
        {
            "Foo",
            "Bar",
            "",
            null,
            "9392gk381",
        };
}