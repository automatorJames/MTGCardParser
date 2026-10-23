namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Scroll Rack, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scroll Rack</exampledoc>
[TokenizationOrder(-1)]
public class ScrollRackRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put that many cards from the top of your library into your hand"];
}
