namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Luminesce, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Luminesce</exampledoc>
[TokenizationOrder(-1)]
public class LuminesceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage that black sources and red sources would deal this turn"];
}
