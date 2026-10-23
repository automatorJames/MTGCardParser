namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gix's Command, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gix's Command</exampledoc>
[TokenizationOrder(-1)]
public class GixsCommandRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• put two +1/+1 counters on up to one creature"];
}
