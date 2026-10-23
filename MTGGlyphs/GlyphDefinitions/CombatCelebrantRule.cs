namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Combat Celebrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Combat Celebrant</exampledoc>
[TokenizationOrder(-1)]
public class CombatCelebrantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature hasn't been exerted this turn, you may exert it as it attacks"];
}
