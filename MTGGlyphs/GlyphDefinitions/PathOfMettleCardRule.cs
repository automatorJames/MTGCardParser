namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Path of Mettle // Metzali, Tower of Triumph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Path of Mettle // Metzali, Tower of Triumph</exampledoc>
[TokenizationOrder(-1)]
public class PathOfMettleCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when path of mettle enters, it deals 1 damage to each creature that doesn't have first strike, double strike, vigilance, or haste"];
}
