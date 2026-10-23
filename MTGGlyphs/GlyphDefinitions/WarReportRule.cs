namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of War Report, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>War Report</exampledoc>
[TokenizationOrder(-1)]
public class WarReportRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to the number of creatures on the battlefield plus the number of artifacts on the battlefield"];
}
