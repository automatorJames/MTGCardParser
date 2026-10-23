namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vision Quest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vision Quest</exampledoc>
[TokenizationOrder(-1)]
public class VisionQuestRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 4 or greater, it gains haste until end of turn"];
}
