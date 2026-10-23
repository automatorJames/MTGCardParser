namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tamiyo, Collector of Tales, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tamiyo, Collector of Tales</exampledoc>
[TokenizationOrder(-1)]
public class TamiyoCollectorOfTalesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells and abilities your opponents control can't cause you to discard cards or sacrifice permanents"];
}
