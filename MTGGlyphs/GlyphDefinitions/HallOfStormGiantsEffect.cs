namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hall of Storm Giants, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hall of Storm Giants</exampledoc>
[Dependent]
public class HallOfStormGiantsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, this land becomes a 7/7 blue giant creature with ward {3}"];
}
