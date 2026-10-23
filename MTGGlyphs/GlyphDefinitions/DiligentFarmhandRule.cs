namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Diligent Farmhand, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Diligent Farmhand</exampledoc>
[TokenizationOrder(-1)]
public class DiligentFarmhandRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this card is in a graveyard, effects from spells named muscle burst count it as a card named muscle burst"];
}
