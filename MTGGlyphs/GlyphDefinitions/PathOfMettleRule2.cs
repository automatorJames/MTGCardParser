namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Path of Mettle // Metzali, Tower of Triumph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Path of Mettle // Metzali, Tower of Triumph</exampledoc>
[TokenizationOrder(-1)]
public class PathOfMettleRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you attack with at least two creatures that have first strike, double strike, vigilance, and/or haste, transform path of mettle"];
}
