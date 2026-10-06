namespace Glyphotype.Attributes;

/// <summary>
/// Binds a <see cref="BackReference"/> property to another property of the same Glyph, rather than leaving it to
/// <see cref="BackReferenceResolver"/>'s search: the Glyph knows what its own pronoun means, e.g. in
/// <c>["destroy", Prop(Target), ".", Prop(Pronoun), "can't be regenerated"]</c>, <c>Pronoun</c> refers to
/// <c>Target</c>. The named property needn't be marked <see cref="ReferentAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RefersToAttribute(string propertyName) : Attribute
{
    /// <summary>The name of the property, on the same Glyph type, whose captured value is the referent.</summary>
    public string PropertyName { get; } = propertyName;
}
