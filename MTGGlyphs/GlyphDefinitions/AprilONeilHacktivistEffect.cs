namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of April O'Neil, Hacktivist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>April O'Neil, Hacktivist</exampledoc>
[Dependent]
public class AprilONeilHacktivistEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each card type among spells you've cast this turn"];
}
