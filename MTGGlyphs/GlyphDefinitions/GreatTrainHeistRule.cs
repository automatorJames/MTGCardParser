namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Great Train Heist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Great Train Heist</exampledoc>
[TokenizationOrder(-1)]
public class GreatTrainHeistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control deals combat damage to that player this turn, create a tapped treasure token"];
}
