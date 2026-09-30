namespace Glyphotype.Attributes;

/// <summary>
/// On a <see cref="DynamicGlyph"/> property: when nothing resolves the text it captured, the property keeps that
/// text as unmatched text, rather than failing the glyph's whole match. So a glyph for a construction whose inner
/// part isn't modeled yet - "every {weekday}, {anything}" - still matches, with its unresolved part showing as
/// unmatched text within it (a <see cref="DynamicGlyph"/> whose <see cref="DynamicGlyph.Item"/> is an
/// <see cref="UnmatchedString"/>). Once some glyph resolves that text, it resolves as usual.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AllowUnmatchedAttribute : Attribute
{
}
