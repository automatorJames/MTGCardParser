namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Songcrafter Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Songcrafter Mage</exampledoc>
[TokenizationOrder(-1)]
public class SongcrafterMage2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then exile the spell"];
}
