namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Brine Hag, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brine Hag</exampledoc>
[Dependent]
public class BrineHagEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the base power and toughness of each creature that dealt damage to it this turn become 0/2"];
}
