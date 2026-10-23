namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Treasure Keeper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Treasure Keeper</exampledoc>
/// <examplecapture>put all revealed cards not cast this way on the bottom of your library in a random order</examplecapture>
[TokenizationOrder(-1)]
public class TreasureKeeperRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put all revealed cards not cast this way on the bottom of your library in a random order"];
}
