namespace Glyphotype.Attributes;

/// <summary>
/// Marks something a <see cref="BackReference"/> can refer to. On a property, the property's captured value is the
/// referent - in "destroy target creature. it can't be regenerated", the creature targeted; on a glyph class, every
/// match of the class is one.
/// <para>
/// A referent's kind is its type, never declared: a property's is the type of what it captured (an enum such as
/// <c>CardType</c>, or a glyph - for a one-of, whichever alternative matched), a class's is the class. A
/// <see cref="BackReference{T}"/> refers only to referents of kind <c>T</c>; a plain <see cref="BackReference"/>, a
/// pronoun, to any. Give its <see cref="Number"/> - <c>[Referent(GrammaticalNumber.Plural)]</c> - where "it" and "they"
/// need telling apart.
/// </para>
/// <para>
/// Mark only what something later refers back to: every referent is one more thing a back-reference may have to skip.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class ReferentAttribute(GrammaticalNumber number = GrammaticalNumber.Unspecified) : Attribute
{
    /// <summary>Whether the referent is one thing or several - left unspecified, a back-reference of either number can refer to it.</summary>
    public GrammaticalNumber Number { get; } = number;
}
