namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Speed Demon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Speed Demon</exampledoc>
[Dependent]
public class TheSpeedDemonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you draw x cards and lose x life, where x is your speed"];
}
