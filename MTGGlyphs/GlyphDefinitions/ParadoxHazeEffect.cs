namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Paradox Haze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Paradox Haze</exampledoc>
[Dependent]
public class ParadoxHazeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player gets an additional upkeep step after this step"];
}
