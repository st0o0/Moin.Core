namespace FicktEuchAllee.Core;

/// <summary>
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    public static List<Segment> ToSegments(this string topic) => topic.Split(Matcher.SegmentSeperator).Select(x => new Segment(x)).ToList();
}
