namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zur's Weirding, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zur's Weirding</exampledoc>
[TokenizationOrder(-1)]
public class ZursWeirdingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then any other player may pay 2 life"];
}
