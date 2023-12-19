namespace Moin.Core.Tests;

public class SegmentTests

{
    [Fact]
    public void SegmentIsSYS()
    {
        var segment = new Segment("$SYS");
        Assert.True(segment.IsSYS);
    }

    [Fact]
    public void SegmentIsSingleWildcard()
    {
        var segment = new Segment("+");
        Assert.True(segment.IsSingleWildcard);
    }

    [Fact]
    public void SegmentIsMultiWildcard()
    {
        var segment = new Segment("#");
        Assert.True(segment.IsMultiWildcard);
    }
}
