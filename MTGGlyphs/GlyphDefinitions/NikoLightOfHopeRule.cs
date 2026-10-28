namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Niko, Light of Hope, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Niko, Light of Hope</exampledoc>
[TokenizationOrder(-1)]
public class NikoLightOfHopeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shards you control become copies of it until the next end step"];
}
