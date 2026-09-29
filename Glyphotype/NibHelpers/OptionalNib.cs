namespace Glyphotype.NibHelpers;

/// <summary>A nib that may be absent (see <see cref="GlyphPrimitives.Glyph.Opt"/>): literal text, or a <see cref="PatternNib"/>.</summary>
public record OptionalNib : Nib
{
    /// <summary>The nib made optional - kept so a display-only consumer can reconstruct the original <c>Opt(...)</c> call.</summary>
    public Nib Inner { get; }

    public OptionalNib(Nib inner)
        : base(inner.Text, inner.Regex)
    {
        Inner = inner;
    }
}
