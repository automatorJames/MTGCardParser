namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Critical Hit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Critical Hit</exampledoc>
[TokenizationOrder(-1)]
public class CriticalHitRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you roll a natural 20, return this card from your graveyard to your hand"];
}
