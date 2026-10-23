namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Desert Were-Worm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desert Were-Worm</exampledoc>
[Dependent]
public class DesertWereWormEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap all attacking creatures"];
}
