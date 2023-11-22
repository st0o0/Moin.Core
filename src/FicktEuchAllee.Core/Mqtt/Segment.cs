namespace FicktEuchAllee.Core;

public readonly struct Segment(string segment) : IEquatable<Segment?>
{
    public string Content { get; } = segment;
    public bool IsSingleWildcard => !string.IsNullOrWhiteSpace(Content) && Content.StartsWith('+') && Content.EndsWith('+');
    public bool IsMultiWildcard => !string.IsNullOrWhiteSpace(Content) && Content.StartsWith('#') && Content.EndsWith('#');
    public bool IsSYS => !string.IsNullOrWhiteSpace(Content) && Content.StartsWith('$');

    public bool Equals(Segment? other) => Content == other?.Content;
    public static bool operator ==(Segment left, Segment right) => left.Equals(right);
    public static bool operator !=(Segment left, Segment right) => !(left == right);
    public override bool Equals(object obj) => obj is Segment segment1 && Equals(segment1);
    public override int GetHashCode() => Content.GetHashCode();
}
