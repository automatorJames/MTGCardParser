namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Path of Mettle // Metzali, Tower of Triumph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Path of Mettle // Metzali, Tower of Triumph</exampledoc>
[Dependent]
public class PathOfMettle2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["double strike, vigilance, and/or haste, transform path of mettle"];
}
