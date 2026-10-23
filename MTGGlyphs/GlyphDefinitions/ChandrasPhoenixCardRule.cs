namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chandra's Phoenix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra's Phoenix</exampledoc>
[TokenizationOrder(-1)]
public class ChandrasPhoenixCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent is dealt damage by a red instant or sorcery spell you control or by a red planeswalker you control, return this card from your graveyard to your hand"];
}
