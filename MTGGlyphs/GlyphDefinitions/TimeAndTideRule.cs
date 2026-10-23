namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Time and Tide, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Time and Tide</exampledoc>
[TokenizationOrder(-1)]
public class TimeAndTideRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["simultaneously, all phased-out creatures phase in and all creatures with phasing phase out"];
}
