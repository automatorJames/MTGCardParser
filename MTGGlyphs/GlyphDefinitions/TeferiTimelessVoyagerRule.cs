namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Teferi, Timeless Voyager, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi, Timeless Voyager</exampledoc>
[TokenizationOrder(-1)]
public class TeferiTimelessVoyagerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until the end of your next turn, they can't phase in"];
}
