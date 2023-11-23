namespace FicktEuchAllee.Core;

public struct Segment(string segment) : IEquatable<Segment>
{
    public readonly string Content { get; } = segment;
    public readonly bool IsSingleWildcard => !string.IsNullOrWhiteSpace(Content) && Content.StartsWith('+') && Content.EndsWith('+');
    public readonly bool IsMultiWildcard => !string.IsNullOrWhiteSpace(Content) && Content.StartsWith('#') && Content.EndsWith('#');
    public readonly bool IsSYS => !string.IsNullOrWhiteSpace(Content) && Content.StartsWith('$');

    public static bool operator ==(Segment left, Segment right) => left.Equals(right);
    public static bool operator !=(Segment left, Segment right) => !left.Equals(right);
    public readonly bool Equals(Segment other) => Content == other.Content;
    public override readonly int GetHashCode() => Content.GetHashCode();
    public override bool Equals(object obj) => obj is Segment value && Equals(value);
}
