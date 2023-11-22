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
    public void MatcherIsMultiWildcard()
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
}
