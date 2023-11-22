namespace FicktEuchAllee.Core;

public static class StringExtensions
{
    public static List<Segment> ToSegments(this string topic) => topic.Split(Matcher.SegmentSeperator).Select(x => new Segment(x)).ToList();
}
