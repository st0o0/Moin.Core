namespace FicktEuchAllee.Core;

/// <summary>
/// </summary>
public readonly struct Segment(string segment) : IEquatable<Segment>
{
    /// <summary>
    /// </summary>
    public readonly string Content { get; } = segment;

    /// <summary>
    /// </summary>
    public readonly bool IsSingleWildcard => !string.IsNullOrWhiteSpace(Content) && Content.StartsWith('+') && Content.EndsWith('+');

    /// <summary>
    /// </summary>
    public readonly bool IsMultiWildcard => !string.IsNullOrWhiteSpace(Content) && Content.StartsWith('#') && Content.EndsWith('#');

    /// <summary>
    /// </summary>
    public readonly bool IsSYS => !string.IsNullOrWhiteSpace(Content) && Content.StartsWith('$');

    /// <summary>
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Segment left, Segment right) => left.Equals(right);

    /// <summary>
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Segment left, Segment right) => !left.Equals(right);

    /// <summary>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public readonly bool Equals(Segment other) => Content == other.Content;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override readonly int GetHashCode() => Content.GetHashCode();

    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => obj is not null && obj is Segment value && Equals(value);
}
