namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ruin Raider, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ruin Raider</exampledoc>
[TokenizationOrder(-1)]
public class RuinRaiderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you lose life equal to the card's mana value"];
}
