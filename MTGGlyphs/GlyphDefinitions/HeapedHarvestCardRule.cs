namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Heaped Harvest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heaped Harvest</exampledoc>
[TokenizationOrder(-1)]
public class HeapedHarvestCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this artifact enters and when you sacrifice it, you may search your library for a basic land card, put it onto the battlefield tapped, then shuffle"];
}
