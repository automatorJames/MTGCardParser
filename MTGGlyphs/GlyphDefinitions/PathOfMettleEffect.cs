namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Path of Mettle // Metzali, Tower of Triumph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Path of Mettle // Metzali, Tower of Triumph</exampledoc>
[Dependent]
public class PathOfMettleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals 1 damage to each creature that doesn't have first strike, double strike, vigilance, or haste"];
}
