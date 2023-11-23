namespace FicktEuchAllee.Core.Tests;

public class MatcherTests
{
    [Fact]
    public void MatcherIsEqual()
    {
        var result = Matcher.Match("kekw/kekw", "kekw/kekw", out _);
        Assert.True(result);
    }

    [Theory]
    [InlineData("Haus/Erdgeschoss/Kueche/Temperatur", true, 1)]
    [InlineData("Haus/Erdgeschoss/Stauraum/Temperatur", true, 1)]
    [InlineData("Haus/Erdgeschoss/Terrasse/Temperatur", true, 1)]
    [InlineData("kekw/kekw", false, 0)]
    public void MatcherIsSingleWildcard(string value, bool expected, int expectedWildcards)
    {
        var result = Matcher.Match(value, "Haus/Erdgeschoss/+/Temperatur", out var wildcards);
        Assert.Equal(expected, result);
        Assert.Equal(expectedWildcards, wildcards.Count);
    }

    [Theory]
    [InlineData("Haus/Erdgeschoss/Kueche/Temperatur", true, 2)]
    [InlineData("Haus/Erdgeschoss/Kueche/Luftfeuchte", true, 2)]
    [InlineData("Haus/Erdgeschoss/Stauraum/Temperatur", true, 2)]
    [InlineData("Haus/Erdgeschoss/Terrasse/Temperatur", true, 2)]
    [InlineData("Haus/Erdgeschoss/Stauraum/Licht", true, 2)]
    [InlineData("kekw/kekw", false, 0)]
    public void MatcherIsMultiWildcard(string value, bool expected, int expectedWildcards)
    {
        var result = Matcher.Match(value, "Haus/Erdgeschoss/#", out var wildcards);
        Assert.Equal(expected, result);
        Assert.Equal(expectedWildcards, wildcards.Count);
    }
}
