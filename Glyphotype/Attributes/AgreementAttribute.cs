namespace Glyphotype.Attributes;

/// <summary>
/// What a <see cref="BackReference"/> type's referent has to be: <c>[Agreement(GrammaticalNumber.Plural)]</c> on a
/// "they", <c>[Agreement(GrammaticalNumber.Singular, "creature")]</c> on a "that creature". It agrees with a
/// referent (see <see cref="IntroducesAttribute"/>) that matches on both, either side's unspecified accepting
/// anything. A back-reference without one agrees with every referent.
/// <para>
/// Declared per type rather than worked out from what was captured, so it's grammar a definition can carry: a
/// pronoun whose number varies is a <see cref="OneOf{T1,T2}"/> of one back-reference type per number.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AgreementAttribute(GrammaticalNumber number = GrammaticalNumber.Unspecified, string kind = null) : Attribute
{
    public GrammaticalNumber Number { get; } = number;

    /// <summary>The kind of thing the referent must be (e.g. "creature"), or null for any. Compared ignoring case.</summary>
    public string Kind { get; } = kind;
}
