namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ashen Ghoul, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashen Ghoul</exampledoc>
[TokenizationOrder(-1)]
public class AshenGhoulRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only during your upkeep and only if three or more creature cards are above this card"];
}
