namespace Glyphotype.Attributes;

/// <summary>
/// Declares that what's captured here introduces a referent - something a later <see cref="BackReference"/> ("it",
/// "they", "that creature") can refer back to. On a property, the property's captured value is the referent (in
/// "destroy all creatures blocking it", the creatures destroyed); on a Glyph type, every match of the type is one
/// (a "target creature" Glyph, wherever it's used).
/// <para>
/// <see cref="Number"/> and <see cref="Kind"/> are what a back-reference has to agree with (see
/// <see cref="AgreementAttribute"/>); left unspecified, any back-reference agrees.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class IntroducesAttribute(GrammaticalNumber number = GrammaticalNumber.Unspecified, string kind = null) : Attribute
{
    /// <summary>The referent's number.</summary>
    public GrammaticalNumber Number { get; } = number;

    /// <summary>The kind of thing the referent is (e.g. "creature"), or null when unspecified.</summary>
    public string Kind { get; } = kind;
}
