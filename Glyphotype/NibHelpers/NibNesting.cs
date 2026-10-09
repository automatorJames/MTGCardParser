namespace Glyphotype.NibHelpers;

/// <summary>
/// What <c>Opt(...)</c> and <c>Plural(...)</c> may wrap, in one place. <see cref="OptionalNib"/> and
/// <see cref="PluralNib"/> read it as a glyph's nibs are built, and whatever reads a glyph definition as data reads it
/// too, so a definition breaking it is refused as it's read - with the same message - rather than when its glyph is
/// first built.
/// </summary>
public static class NibNesting
{
    /// <summary>What a nib is, as far as wrapping it goes.</summary>
    public enum Kind
    {
        /// <summary>Literal text, alternatives or a pattern - anything either wrapper takes.</summary>
        Text,
        Property,
        Optional,
        Plural,

        /// <summary>A glyph matched in place, e.g. <see cref="Nib.This"/>.</summary>
        Embedded,
    }

    /// <summary>Why <c>Opt</c> can't wrap a nib of kind <paramref name="inner"/>, or null if it can.</summary>
    /// <param name="embeddedName">The embedded glyph's name, for an <see cref="Kind.Embedded"/> nib ("This").</param>
    public static string GetOptionalError(Kind inner, string embeddedName = null) => inner switch
    {
        Kind.Plural => "Opt(Plural(...)) - a nib can't be both optional and plural; write its forms out, e.g. Opt(Alt(\"card\", \"cards\"))",
        Kind.Property => "Opt(Prop(...)) - make a property optional with [Optional] on it instead",
        Kind.Embedded => $"Opt(Nib.{embeddedName}) - an embedded glyph can't be optional; write the glyph with and without it",
        _ => null,
    };

    /// <summary>Why <c>Plural</c> can't wrap a nib of kind <paramref name="inner"/>, or null if it can.</summary>
    /// <param name="embeddedName">The embedded glyph's name, for an <see cref="Kind.Embedded"/> nib ("This").</param>
    public static string GetPluralError(Kind inner, string embeddedName = null) => inner switch
    {
        Kind.Plural => "Plural(Plural(...)) - a nib can only be made plural once",
        Kind.Embedded => $"Plural(Nib.{embeddedName}) - an embedded glyph can't be made plural",
        Kind.Optional => "Plural(Opt(...)) - a nib can't be both optional and plural; write its forms out, e.g. Opt(Alt(\"card\", \"cards\"))",
        _ => null,
    };

    /// <summary>What <paramref name="nib"/> is, as far as wrapping it goes.</summary>
    public static Kind KindOf(Nib nib) => nib switch
    {
        PluralNib => Kind.Plural,
        OptionalNib => Kind.Optional,
        PropertyNib => Kind.Property,
        EmbeddedGlyphNib => Kind.Embedded,
        _ => Kind.Text,
    };
}
