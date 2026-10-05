namespace Glyphotype.Attributes;

/// <summary>
/// Each member of a vocabulary (an enum) matches singular or plural: "creature" and "creatures". On the enum, wherever
/// it's used; on an enum property, at just that property. (For literal text, see <see cref="GlyphPrimitives.Glyph.Plural"/>.)
/// </summary>
[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property)]
public class OptionalPluralAttribute : Attribute
{
}
