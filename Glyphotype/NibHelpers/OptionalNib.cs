namespace Glyphotype.NibHelpers;

/// <summary>A nib that may be absent (see <see cref="GlyphPrimitives.Glyph.Opt"/>): literal text, or a <see cref="PatternNib"/>.</summary>
public record OptionalNib : Nib
{
    /// <summary>The nib made optional - kept so a display-only consumer can reconstruct the original <c>Opt(...)</c> call.</summary>
    public Nib Inner { get; }

    public OptionalNib(Nib inner)
        : base(inner.Text, inner.Regex)
    {
        Inner = inner switch
        {
            PluralNib => throw new ArgumentException("Opt(Plural(...)) - a nib can't be both optional and plural; write its forms out, e.g. Opt(Alt(\"card\", \"cards\"))", nameof(inner)),
            PropertyNib => throw new ArgumentException("Opt(Prop(...)) - make a property optional with [Optional] on it instead", nameof(inner)),
            EmbeddedGlyphNib => throw new ArgumentException($"Opt(Nib.{inner.Text}) - an embedded glyph can't be optional; write the glyph with and without it", nameof(inner)),
            _ => inner,
        };
    }
}
