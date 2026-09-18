namespace Glyphotype.Attributes;

/// <summary>
/// Opts a top-level <see cref="Glyph"/> type out of the whole-segment tokenization requirement, letting
/// it match a prefix of a segment rather than having to consume one end to end. A "segment" is everything
/// from the start of the tokenization scope up to but not including the next period, or through the end of
/// the line, whichever comes first - a line may hold several.
/// <para>
/// Only meaningful when <see cref="GlobalSettings.AllowPartialSegmentMatches"/> is false: the attribute
/// overrides that global default for this one type. When the global setting is true every type already
/// matches partially, so the attribute is simply redundant rather than wrong.
/// </para>
/// <para>
/// Mutually exclusive with <see cref="MustMatchWholeLineAttribute"/> (which is strictly stricter than the
/// requirement this opts out of) and with <see cref="DependentAttribute"/> (a dependent is never a
/// top-level tokenization candidate in the first place, so it has no segment of its own to match).
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AllowPartialSegmentMatchAttribute : Attribute
{
}
