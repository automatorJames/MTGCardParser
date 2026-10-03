namespace Glyphotype.GlyphPrimitives;

/// <summary>
/// A Glyph that refers back to something mentioned earlier - "it", "they", "that creature": an anaphor, in
/// linguistics terms. Not "pronoun", which is both too narrow (most MTG back-references are noun phrases like
/// "that creature") and too broad ("you" and "each" are pronouns that refer back to nothing). It matches like any
/// other Glyph, from its own <see cref="Glyph.Nibs"/>; what it refers to, its <see cref="Antecedent"/>, is
/// resolved afterwards, once the whole line has been tokenized (see <see cref="BackReferenceResolver"/>).
/// <para>
/// Reference is kept out of matching on purpose. A match only ever sees its own text, but an antecedent can sit
/// in an earlier clause, or in another Glyph entirely - so a Glyph spanning two clauses and two Glyphs of one
/// clause each leave their back-references to the same resolution, and how a line happens to be split into
/// Glyphs never changes what its pronouns mean.
/// </para>
/// <para>
/// A referent is anything a Glyph declares with <see cref="IntroducesAttribute"/>. The resolver binds each
/// back-reference to the most recent one in its line that agrees with its <see cref="AgreementAttribute"/>, or
/// leaves it unresolved - never guesses. Where the Glyph containing a back-reference knows better, it binds it
/// outright with <see cref="RefersToAttribute"/>.
/// </para>
/// </summary>
public abstract class BackReference : Glyph
{
    /// <summary>The number the referent must have, from this type's <see cref="AgreementAttribute"/>.</summary>
    public GrammaticalNumber Number => Agreement?.Number ?? GrammaticalNumber.Unspecified;

    /// <summary>The kind of thing the referent must be, from this type's <see cref="AgreementAttribute"/> - null for any.</summary>
    public string Kind => Agreement?.Kind;

    /// <summary>What this back-reference refers to: null until it's resolved, and after if nothing before it agrees with it.</summary>
    public Referent Antecedent { get; internal set; }

    AgreementAttribute Agreement => Type.GetCustomAttribute<AgreementAttribute>();

    /// <summary>Whether <paramref name="referent"/> agrees with this back-reference on <see cref="Number"/> and <see cref="Kind"/>, either side's unspecified accepting anything.</summary>
    public bool AgreesWith(Referent referent) =>
        (Number == GrammaticalNumber.Unspecified || referent.Number == GrammaticalNumber.Unspecified || Number == referent.Number)
        && (Kind is null || referent.Kind is null || string.Equals(Kind, referent.Kind, StringComparison.OrdinalIgnoreCase));
}
