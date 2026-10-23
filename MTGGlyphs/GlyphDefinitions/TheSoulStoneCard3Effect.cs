namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Soul Stone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Soul Stone</exampledoc>
[Dependent]
public class TheSoulStoneCard3Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["harness", Nib.This];
}
