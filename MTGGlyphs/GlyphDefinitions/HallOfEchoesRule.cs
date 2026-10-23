namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hall of Echoes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hall of Echoes</exampledoc>
[TokenizationOrder(-1)]
public class HallOfEchoesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the \"legend rule\" doesn't apply to permanents you control this turn"];
}
