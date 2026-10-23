namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Master of Lake-town, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Master of Lake-town</exampledoc>
[Dependent]
public class TheMasterOfLakeTownEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each graveyard with seven or more cards in it"];
}
