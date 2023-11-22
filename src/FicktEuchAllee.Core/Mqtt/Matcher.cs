namespace FicktEuchAllee.Core;

internal record MergeSegment(Segment? ATS, Segment? TS);

public static class Matcher
{
    public const char SegmentSeperator = '/';

    public static bool Match(string topic, string allowedTopic, out List<string> wildcards)
    {
        wildcards = [];
        return Match(topic.ToSegments(), allowedTopic.ToSegments(), out wildcards);
    }

    public static bool Match(List<Segment> topicSegments, List<Segment> allowedTopicSegments, out List<string> wildcards)
    {
        wildcards = [];
        if (topicSegments.Count == 0 || allowedTopicSegments.Count == 0) return false;

        var mergeSegments = Enumerable
        .Range(0, Math.Max(allowedTopicSegments.Count, topicSegments.Count))
        .Select(n => new MergeSegment(allowedTopicSegments.ElementAtOrDefault(n), topicSegments.ElementAtOrDefault(n)))
        .ToList();

        var mergeSegmentsLength = mergeSegments.Count;
        if ((allowedTopicSegments[0].IsSYS && !topicSegments[0].IsSYS) || (topicSegments[0].IsSYS && !allowedTopicSegments[0].IsSYS))
        {
            return true;
        }

        var isMultiWildcard = false;
        foreach (var item in mergeSegments)
        {
            (Segment? ATS, Segment? TS) = item;

            if (!ATS.Equals(TS)) // || mergeSegments.IndexOf >= topicLength
            {
                // Check for wildcard matches
                if (ATS is Segment ats0 && TS is Segment ts0 && ats0.IsSingleWildcard)
                {
                    // Check for bad "+foo" or "a/+foo" subscription
                    if (ats0.Content.Length > 0 && ats0.Content.Contains('/'))
                    {
                        return false;
                    }

                    wildcards.Add(ts0.Content);
                }
                else if (ATS is Segment ats1 && TS is Segment ts1 && ats1.IsMultiWildcard)
                {
                    // Check for bad "foo#" subscription
                    if (ats1.Content.Length > 0 && ats1.Content.Contains('#'))
                    {
                        return false;
                    }

                    isMultiWildcard = true;
                    wildcards.Add(ts1.Content);
                }
                else
                {
                    // Check for e.g. foo/bar matching foo/+/#
                    if (isMultiWildcard && TS is Segment ts2)
                    {
                        wildcards.Add(ts2.Content);
                    }

                    // Valid input, but no match
                    return false;
                }
            }
        }

        if (topicSegments.Count != allowedTopicSegments.Count)
        {
            return false;
        }

        return true;
    }
}