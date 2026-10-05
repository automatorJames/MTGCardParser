namespace Glyphotype.NibHelpers;

/// <summary>
/// A literal word or phrase, singular or plural (see <see cref="GlyphPrimitives.Glyph.Plural"/>): <c>Plural("creature")</c>
/// matches "creature" and "creatures", <c>Plural("berry")</c> "berry" and "berries". The plural is the one
/// <see cref="OptionalPluralAttribute"/> gives a vocabulary's members (see <see cref="Extensions.AddPluralization"/>),
/// formed on the last word.
/// </summary>
public record PluralNib : Nib
{
    /// <summary>The singular, as authored.</summary>
    public string Singular { get; }

    /// <summary>The plural formed from <see cref="Singular"/>.</summary>
    public string PluralForm { get; }

    public PluralNib(string singular)
        : base(RequireTexts("Plural", [singular], minimum: 1)[0], BuildRegex(singular))
    {
        Singular = singular;
        PluralForm = singular.AddPluralization(makeOptional: false);
    }

    public override string Authored => $"Plural({Quote(Singular)})";

    public override IReadOnlyList<string> Literals => [Singular, PluralForm];

    public override IReadOnlyList<string> Branches(Joiner joiner) =>
        PluralForm.StartsWith(Singular, StringComparison.Ordinal) ? [Regex] : [EscapeLiteral(Singular), EscapeLiteral(PluralForm)];

    /// <summary><c>creature(s)?</c> where the plural only adds to the singular; else both whole, <c>(berry|berries)</c>.</summary>
    static string BuildRegex(string singular)
    {
        var plural = singular.AddPluralization(makeOptional: false);

        return plural.StartsWith(singular, StringComparison.Ordinal)
            ? $"{EscapeLiteral(singular)}({EscapeLiteral(plural[singular.Length..])})?"
            : $"({EscapeLiteral(singular)}|{EscapeLiteral(plural)})";
    }
}
