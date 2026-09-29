namespace Glyphotype.NibHelpers;

/// <summary>Exactly one of several literal texts (see <see cref="GlyphPrimitives.Glyph.Alt"/>).</summary>
public record NibAlternatives : Nib
{
    /// <summary>The individual alternatives as originally passed to <see cref="Glyphotype.GlyphPrimitives.Glyph.Alt"/>, preserved alongside the joined <see cref="Nib.Text"/> so a display-only consumer (e.g. <see cref="Glyphotype.RegexGeneration.Presentation.GlyphClassRenderer"/>) can reconstruct the original <c>Alt(...)</c> call exactly.</summary>
    public string[] Alternatives { get; }

    public NibAlternatives(params string[] alternatives)
        : base("(" + string.Join('|', alternatives) + ")", "(" + string.Join('|', alternatives.Select(EscapeLiteral)) + ")")
    {
        Alternatives = alternatives;
    }
}
