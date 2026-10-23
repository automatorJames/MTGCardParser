namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Heaped Harvest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heaped Harvest</exampledoc>
/// <examplecapture>you may search your library for a basic land card, put it onto the battlefield tapped, then shuffle</examplecapture>
[Dependent]
public class HeapedHarvestEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your library for a basic land card, put it onto the battlefield tapped, then shuffle"];
}
