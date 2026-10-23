namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Die Young, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Die Young</exampledoc>
[TokenizationOrder(-1)]
public class DieYoungRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the creature gets -1/-1 until end of turn for each {e} paid this way"];
}
