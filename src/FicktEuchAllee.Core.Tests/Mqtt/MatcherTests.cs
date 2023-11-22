namespace FicktEuchAllee.Core.Tests;

public class MatcherTests
{
    [Fact]
    public void MatcherIsEqual()
    {
        var result = Matcher.Match("kekw/kekw", "kekw/kekw", out _);
        Assert.True(result);
    }

    [Fact]
    public void MatcherIsSingleWildcard()
    {
        List<string> list = [
            "Haus/Erdgeschoss/Kueche/Temperatur",
            "Haus/Erdgeschoss/Stauraum/Temperatur",
            "Haus/Erdgeschoss/Terrasse/Temperatur"];

        foreach (var item in list)
        {
            var result = Matcher.Match(item, "Haus/Erdgeschoss/+/Temperatur", out var wildcards);
            Assert.True(result);
            Assert.Single(wildcards);
        }
    }

    [Fact]
    public void MatcherIsMultiWildcard()
    {
        List<string> list = [
            "Haus/Erdgeschoss/Kueche/Temperatur",
            "Haus/Erdgeschoss/Kueche/Luftfeuchte",
            "Haus/Erdgeschoss/Stauraum/Temperatur",
            "Haus/Erdgeschoss/Stauraum/Licht",
            "Haus/Erdgeschoss/Terrasse/Temperatur"];

        foreach (var item in list)
        {
            var result = Matcher.Match(item, "Haus/Erdgeschoss/#", out var wildcards);
            Assert.True(result);
            Assert.Equal(2, wildcards.Count);
        }
    }
}
